// See https://aka.ms/new-console-template for more information


using MicroMuteTerminal;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;

namespace MicroMuteTerminal // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        const string APIKEY = "noauth";
        const string URL = "vprobst.de:5555";
        private static uint CONNECTION_ATTEMPTS_COUNT = 3;    // how often to retry login->WebSocket process before exiting
        const string USERNAME = "bob";
        const string PASSWORD = "123";
        const bool ISHTTPS = false; 

        static async Task Main(string[] args)
        {
            Console.WriteLine("Running MicroMute in the Terminal");

            ConnData data = new ConnData(USERNAME, PASSWORD ,APIKEY, URL, ISHTTPS);
            await Login.StartLoginLoop(data, CONNECTION_ATTEMPTS_COUNT);
        }
    }

    // container to store all our Connection Data. Easier to pass down and change custom-formatting.
    public class ConnData
    {
        public string Username { get; private set; }
        public string Password { get; }
        public string Apikey { get; }
        public string Url   { get; }  // without http:// BUT WITH PORT! ex: 'vincepr.de:1234'
        public bool IsHTTPS { get; }

        private string OriginalUsername { get; }
        private string UsernameAppend { get; set; } = "";

        public ConnData(string name, string pw, string apikey, string url, bool isHttps)
        {
            OriginalUsername = name;
            Username = name;
            Password = pw;
            Apikey = apikey;
            Url = url;
            IsHTTPS = isHttps;
        }

        public string ToBaseUrl() => IsHTTPS ? $"https://{Url}" : $"http://{Url}";
        public string ToLoginUrl() => ToBaseUrl() + "/login_receiver";
        public string ToWebSocketUrl() => IsHTTPS ? $"wss://{Url}/receiver" : $"ws://{Url}/receiver";
        public string ToSuccessMessage() {
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

