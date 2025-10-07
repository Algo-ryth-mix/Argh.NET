using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Argh.NET.Helpers;

namespace Argh.NET
{
    internal static class ParameterProcessor
    {
        public static IEnumerable<SymbolWrapper> Process(IEnumerable<ParameterInfo> parameters)
        {
            foreach (var parameter in parameters)
            {
                var myAttributes = parameter.GetCustomAttributes().GetSubsetWithTypes(new[] { typeof(ArghOptionAttribute), typeof(ArghParameterAttribute) }).ToArray();
                var kind = ClassifyParameter.Classify(myAttributes);

                switch (kind)
                {
                    case ParameterKind.Unknown:
                    {
                        var argument = SymbolFactory.CreateArgument(parameter.ParameterType, parameter.Name, parameter.DefaultValue);
                        yield return new SymbolWrapper(argument, result => ValueAccessors.GetArgumentValue(argument, result));
                        break;
                    }
                    case ParameterKind.Argument:
                    {
                        var paramAttrib = myAttributes.OfType<ArghParameterAttribute>().FirstOrDefault();
                        var argument = SymbolFactory.CreateArgument(parameter.ParameterType, paramAttrib?.Name ?? parameter.Name, parameter.DefaultValue);
                        argument.Description = paramAttrib?.Description ?? string.Empty;
                        yield return new SymbolWrapper(argument, result => ValueAccessors.GetArgumentValue(argument, result));
                        break;
                    }
                    case ParameterKind.Option:
                    {
                        var optionAttrib = myAttributes.OfType<ArghOptionAttribute>().FirstOrDefault();
                        var option = SymbolFactory.CreateOption(parameter.ParameterType, optionAttrib?.Name ?? parameter.Name, parameter.DefaultValue);
                        option.Description = optionAttrib?.Description ?? string.Empty;
                        SymbolFactory.AddAlias(optionAttrib?.Aliases, option.Aliases);
                        yield return new SymbolWrapper(option, result => ValueAccessors.GetOptionValue(option, result));
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
