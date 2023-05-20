using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMuteTerminal
{
    // Parse command flags, not pretty but i really dont want another library for it.
    internal class Cmd
    {
        // default Values
        private static string USERNAME = "bob";
        private static string PASSWORD = "123";
        private static string APIKEY   = "noauth";
        private static string URL      = "vprobst.de:5555";
        private static bool   ISHTTPS  = false;
        private static uint   CONNECTION_ATTEMPTS_COUNT = 3;    // how often to retry login->WebSocket process before exiting

        public static ConnectionData Init(string[] args)
        {
            if (args.Length > 0)
                foreach (string arg in args)
                    Parse(arg);

            return new ConnectionData(USERNAME, PASSWORD, APIKEY, URL, ISHTTPS, CONNECTION_ATTEMPTS_COUNT);
        }

        private static void Parse(string arg)
        {
            var helptxt = @"
MicroMute Options

-help or --help         for this help

user=jim                to set username to 'jim'
pw=22                   to set password to 22
count=3                 number of attempts to reconnect. 0 for infinite

api=key                 if you selfhost
url=mic.vincepr.de:123  to point to your own ipaddr
https=false             to change to http-only-mode
";

            if (arg.StartsWith("name=")) USERNAME = arg["name=".Length..];
            else if (arg.StartsWith("pw=")) PASSWORD = arg["pw=".Length..];
            else if (arg.StartsWith("api=")) APIKEY = arg["api=".Length..];
            else if (arg.StartsWith("url=")) URL = arg["url=".Length..];
            else if (arg.StartsWith("https="))
            {
                var isValid = bool.TryParse(arg["https=".Length..], out bool result);
                if (isValid) ISHTTPS = result;
                else throw new Exception("Failed parsing https=... ONLY true or false allowed");
            }
            else if (arg.StartsWith("count="))
            {
                var isValid = uint.TryParse(arg["count=".Length..], out uint result);
                if (isValid) CONNECTION_ATTEMPTS_COUNT = result;
                else throw new Exception("Failed parsing count=... must be uint");
            }
            else if (arg.StartsWith("-help") || arg.StartsWith("--help"))
                throw new Exception(helptxt);
            else throw new Exception("\nInvalid Options, try:"+helptxt);
        }
    }
}
