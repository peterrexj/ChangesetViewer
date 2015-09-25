using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCore
{
    public static class Consts
    {
        public const string EmailAddressRegPattern = @"^[a-zA-Z0-9!#$%&'`‘*+/=?^_{|}~–-]+(?:\.[a-zA-Z0-9!#$%&'`‘*+/=?^_{|}~–-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?$";
        public const string IdentiferRegPattern = @"^[a-zA-Z0-9-_]+$";
        public const string ScriptInjectionRegPattern = @"^[^<]*$|^.*<\s.*$";
    }
}
