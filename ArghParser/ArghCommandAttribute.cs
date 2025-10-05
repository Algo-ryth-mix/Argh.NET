using System;
using JetBrains.Annotations;

namespace Argh
{

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false), MeansImplicitUse]
    public class ArghCommandAttribute : Attribute
    {
        public string Verb { get; set; }
        public string Description { get; set; }
        public string Aliases { get; set; }
    }
}
