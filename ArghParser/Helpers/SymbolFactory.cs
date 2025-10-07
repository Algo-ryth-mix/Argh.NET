using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq.Expressions;

namespace Argh.NET
{
    internal static class SymbolFactory
    {
        public static Option CreateOption(Type parameterType, string parameterName, object parameterDefaultValue)
        {
            var type = typeof(Option<>).MakeGenericType(parameterType);
            var inst = Activator.CreateInstance(type, parameterName) as Option;
            ImbueDefaultValueFactory(parameterType, parameterDefaultValue, inst);
            return inst;
        }

        public static Argument CreateArgument(Type parameterType, string name, object parameterDefaultValue)
        {
            var type = typeof(Argument<>).MakeGenericType(parameterType);
            var inst = Activator.CreateInstance(type, name) as Argument;
            ImbueDefaultValueFactory(parameterType, parameterDefaultValue, inst);
            return inst;
        }

        public static void AddAlias(string aliases, ICollection<string> target)
        {
            if (!string.IsNullOrEmpty(aliases))
            {
                foreach (var alias in aliases.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    target.Add(alias.Trim());
                }
            }
        }

        private static void ImbueDefaultValueFactory(Type parameterType, object parameterDefaultValue, object inst)
        {
            if (inst == null || parameterDefaultValue == null || parameterDefaultValue is DBNull)
                return;
            var func = MakeValueFactory(parameterType, parameterDefaultValue);
            var prop = inst.GetType().GetProperty("DefaultValueFactory");
            if (prop != null)
            {
                prop.SetValue(inst, func);
            }
        }

        private static Delegate MakeValueFactory(Type argumentType, object defaultValue)
        {
            var argumentResultParam = Expression.Parameter(typeof(ArgumentResult), "_");
            var valueConstant = Expression.Constant(defaultValue, defaultValue.GetType());
            var castValue = Expression.Convert(valueConstant, argumentType);
            var lambda = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(typeof(ArgumentResult), argumentType),
                castValue,
                argumentResultParam
            );
            return lambda.Compile();
        }
    }
}
