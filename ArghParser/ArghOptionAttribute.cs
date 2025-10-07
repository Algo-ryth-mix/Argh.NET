using JetBrains.Annotations;
using System;

namespace Argh.NET
{
    [AttributeUsage(AttributeTargets.Parameter), MeansImplicitUse]
    public class ArghOptionAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Aliases { get; set; }
    }
}
