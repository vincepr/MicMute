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
        private static uint CONNECTION_ATTEMPTS_COUNT = 0;    // how often to retry login->WebSocket process before exiting
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
    public struct ConnData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Apikey { get; set; }
        public string Url { get; set; }  // without http:// BUT WITH PORT! ex: 'vincepr.de:1234'
        public bool IsHTTPS { get; set; }

        public ConnData(string name, string pw, string apikey, string url, bool isHttps)
        {
            Username = name;
            Password = pw;
            Apikey = apikey;
            Url = url;
            IsHTTPS = isHttps;
        }

        public LoginRequest GetLoginRequest() => new(Apikey, Username, Password);
        public string GetBaseUrl() => IsHTTPS ? $"https://{Url}" : $"http://{Url}";
        public string GetLoginUrl() => GetBaseUrl() + "/login_receiver";
        public string GetWebSocketUrl() => IsHTTPS ? $"wss://{Url}/receiver" : $"ws://{Url}/receiver";
        public string GetSuccessMessage() {
            return $"WebSocket connection successful established. Waiting for controllers to Send Events at \n\n {GetBaseUrl()}\n\n username: {Username}\n password: {Password}";
        }
    }
}

