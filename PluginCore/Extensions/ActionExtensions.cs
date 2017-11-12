using System;

namespace PluginCore.Extensions
{
    public static class ActionExtensions
    {
        public static Action Create(Action action)
        {
            return action;
        }
        public static Action<T1> Create<T1>(Action<T1> action)
        {
            return action;
        }
    }
}
