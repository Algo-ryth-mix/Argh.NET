using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Argh.NET
{
    public sealed class CommandSpec
    {
        public ArghCommandAttribute Attribute { get; }
        public MethodInfo Method { get; }

        public CommandSpec(ArghCommandAttribute attribute, MethodInfo method)
        {
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            Method = method ?? throw new ArgumentNullException(nameof(method));
        }
    }

    public static class CommandDiscovery
    {
        public static IEnumerable<CommandSpec> DiscoverCommands(Type classType)
        {
            if (classType == null) throw new ArgumentNullException(nameof(classType));

            var methods = classType.GetMethods(BindingFlags.Static | BindingFlags.Public);

            return methods
                .Select(m => new { Attribute = m.GetCustomAttribute<ArghCommandAttribute>(), Method = m })
                .Where(x => x.Attribute != null && x.Method.ReturnType == typeof(int))
                .Select(x => new CommandSpec(x.Attribute, x.Method));
        }
    }
}
