using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCore.Classes
{
    public class Void
    {
        private Void() { }
        public static Void Value = new Void();
        public static Tuple<Void> TupleValue = Tuple.Create(Value);
    }
}
