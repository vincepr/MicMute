using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMuteTerminal
{
    // Parse command flags:
    internal class Cmd
    {
        // default Values
        private static string USERNAME = "bob";
        private static string PASSWORD = "123";
        private static string APIKEY   = "noauth";
        private static string URL      = "vprobst.de:5555";
        private static bool   ISHTTPS  = false;
        private static uint   CONNECTION_ATTEMPTS_COUNT = 3;    // how often to retry login->WebSocket process before exiting

        public static ConnData Init(string[] args)
        {
            if (args.Length > 0)
                foreach (string arg in args)
                    Parse(arg);

            return new ConnData(USERNAME, PASSWORD, APIKEY, URL, ISHTTPS, CONNECTION_ATTEMPTS_COUNT);
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


            Console.WriteLine(arg);
        }
    }

    // container to store all our Connection Data. Easier to pass down and change custom-formatting.
    public class ConnData
    {
        public string Username { get; private set; }
        public string Password { get; }
        public string Apikey { get; }
        public string Url { get; }  // without http:// BUT WITH PORT! ex: 'vincepr.de:1234'
        public bool IsHTTPS { get; }

        public uint ConnectionAttempts { get; private set; }

        private string OriginalUsername { get; }
        private string UsernameAppend { get; set; } = "";

        public ConnData(string name, string pw, string apikey, string url, bool isHttps, uint connectionAttempts)
        {
            OriginalUsername = name;
            Username = name;
            Password = pw;
            Apikey = apikey;
            Url = url;
            IsHTTPS = isHttps;
            ConnectionAttempts = connectionAttempts;
        }

        public string ToBaseUrl() => IsHTTPS ? $"https://{Url}" : $"http://{Url}";
        public string ToLoginUrl() => ToBaseUrl() + "/login_receiver";
        public string ToWebSocketUrl() => IsHTTPS ? $"wss://{Url}/receiver" : $"ws://{Url}/receiver";
        public string ToSuccessMessage()
        {
            return $"WebSocket connection successful established. Waiting for controllers to Send Events at \n\n {ToBaseUrl()}\n\n username: {Username}\n password: {Password}";
        }
        public LoginRequest ToLoginRequest() => new(Apikey, Username, Password);

        // in case the username is already taken we try to add 3 random symbols at the end:
        public void RNGUsername(string rng_str)
        {
            UsernameAppend = rng_str.Substring(0, 3);
            Username = OriginalUsername + UsernameAppend;
        }
    }



}
