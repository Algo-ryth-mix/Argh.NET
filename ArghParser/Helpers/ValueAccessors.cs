using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;

namespace Argh.NET
{
    internal static class ValueAccessors
    {
        public static object GetOptionValue(Option option, ParseResult result)
        {
            var helperType = typeof(GetValueBinder<>)
                .MakeGenericType(option.ValueType);
            var helperMethod = helperType.GetMethods().First(x => x.Name == nameof(GetValueBinder<int>.GetValue) && x.GetParameters().Any(p => typeof(Option).IsAssignableFrom(p.ParameterType)));

            var value = helperMethod?.Invoke(null, new object[] { result, option });
            return value;
        }

        public static object GetArgumentValue(Argument argument, ParseResult result)
        {
            var helperType = typeof(GetValueBinder<>)
                .MakeGenericType(argument.ValueType);
            var helperMethod = helperType.GetMethods().First(x => x.Name == nameof(GetValueBinder<int>.GetValue) && x.GetParameters().Any(p => typeof(Argument).IsAssignableFrom(p.ParameterType)));

            var value = helperMethod?.Invoke(null, new object[] { result, argument });
            return value;
        }
    }
}
