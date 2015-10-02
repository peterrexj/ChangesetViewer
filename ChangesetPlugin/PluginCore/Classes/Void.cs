using System;

namespace PluginCore.Classes
{
    public class Void
    {
        private Void() { }
        public static Void Value = new Void();
        public static Tuple<Void> TupleValue = Tuple.Create(Value);
    }
}
