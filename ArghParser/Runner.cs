using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Argh
{
    public static class Runner
    {
        public static void RunClass<T>(string[] args = null, string rootCommandDescription = "")
            where T : class => RunClass(typeof(T), args, rootCommandDescription);


        public static int RunClass(Type @classType, string[] args, string rootCommandDescription = "")
        {
            args = args ?? new string[] { };

            var methods = @classType.GetMethods(BindingFlags.Static | BindingFlags.Public);

            var methodsWithAttributes = methods
                .Select(x => (x.GetCustomAttribute<ArghCommandAttribute>(), x))
                .Where(x => x.Item1 != null && x.Item2.ReturnType == typeof(int))
                .ToList();

            RootCommand root = new RootCommand(rootCommandDescription);

            foreach (var (attribute, method) in methodsWithAttributes)
            {
                Command command = new Command(attribute.Verb, attribute.Description);
                foreach (var alias in (attribute.Aliases ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    command.Aliases.Add(alias.Trim());
                }

                foreach (var parameter in method.GetParameters())
                {
                    object defaultValue = null;
                    if (parameter.HasDefaultValue)
                    {
                        defaultValue = parameter.DefaultValue;
                    }
                    var paramAttrib = parameter.GetCustomAttribute<ArghParameterAttribute>();
                    var argument = CreateArgument(parameter.ParameterType, paramAttrib?.Name ?? parameter.Name, defaultValue);
                    argument.Description = paramAttrib?.Description ?? string.Empty;
                    command.Arguments.Add(argument);

                }
                command.SetAction(result =>
                {
                    var values = method.GetParameters().Select(x => GetParameterValue(x, result)).ToArray();
                    return (int)method.Invoke(null, values);
                });
                root.Add(command);
            }

            return CommandLineParser.Parse(root, args).Invoke();
        }

        private static object GetParameterValue(ParameterInfo parameter, ParseResult result)
        {
            var paramAttrib = parameter.GetCustomAttribute<ArghParameterAttribute>();
            var argumentName = paramAttrib?.Name ?? parameter.Name;
            var helperType = typeof(GetValueBinder<>)?
                .MakeGenericType(parameter.ParameterType);
            var helperMethod = helperType.GetMethod(nameof(GetValueBinder<int>.GetValue));

            var value = helperMethod?.Invoke(null, new object[] { result, argumentName });
            return value;
        }


        private static Argument CreateArgument(Type argumentType, string name, object defaultValue)
        {
            var type = typeof(Argument<>).MakeGenericType(argumentType);
            var inst = Activator.CreateInstance(type, name) as Argument;

            if (inst == null || defaultValue == null)
                return inst;

            var argumentResultParam = Expression.Parameter(typeof(ArgumentResult), "_");
            var valueConstant = Expression.Constant(defaultValue, defaultValue.GetType());
            var castValue = Expression.Convert(valueConstant, argumentType);
            var lambda = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(typeof(ArgumentResult), argumentType),
                castValue,
                argumentResultParam
            );
            inst.GetType().GetProperty("DefaultValueFactory")?.SetValue(inst, lambda.Compile());
            return inst;
        }
    }
}

public static class GetValueBinder<T>
{
    public static T GetValue(ParseResult result, string name)
    {
        return result.GetValue<T>(name);
    }
}