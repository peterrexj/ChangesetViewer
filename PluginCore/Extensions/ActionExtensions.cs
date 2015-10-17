﻿using System;

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
        public static Action<T1, T2> Create<T1, T2>(Action<T1, T2> action)
        {
            return action;
        }
        public static Action<T1, T2, T3> Create<T1, T2, T3>(Action<T1, T2, T3> action)
        {
            return action;
        }
        public static Action<T1, T2, T3, T4> Create<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
        {
            return action;
        }
        public static void Invoke(Action action)
        {
            action();
        }
        public static Func<Classes.Void> AsFunc(this Action action)
        {
            return () =>
            {
                action();
                return Classes.Void.Value;
            };
        }
    }
}