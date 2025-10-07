using System;
using System.Collections.Generic;
using System.Linq;

namespace Argh.NET.Helpers
{
    public static class ClassifyParameter
    {
        public static ParameterKind Classify(IEnumerable<Attribute> myAttributes)
        {
            var attributes = myAttributes as Attribute[] ?? myAttributes.ToArray();
            if(attributes.Any(x => x.GetType() == typeof(ArghOptionAttribute)))
            {
                return ParameterKind.Option;
            }
            else if(attributes.Any(x => x.GetType() == typeof(ArghParameterAttribute)))
            {
                return ParameterKind.Argument;
            }
            return ParameterKind.Unknown;
        }
    }

    public enum ParameterKind
    {
        Unknown,
        Argument,
        Option,
    }
}
