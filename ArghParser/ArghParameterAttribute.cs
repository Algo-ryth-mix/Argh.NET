using System;
using JetBrains.Annotations;

namespace Argh.NET
{
    [AttributeUsage(AttributeTargets.Parameter),MeansImplicitUse]
    public class ArghParameterAttribute : Attribute
    {
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
