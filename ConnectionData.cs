using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMuteTerminal
{
    // container to store all our Connection Data. Easier to pass down and change custom-formatting.
    public class ConnectionData
    {
        public string Username { get; private set; }
        public string Password { get; }
        public string Apikey { get; }
        public string Url { get; }  // without http:// BUT WITH PORT! ex: 'vincepr.de:1234'
        public bool IsHTTPS { get; }

        public uint ConnectionAttempts { get; private set; }

        private string OriginalUsername { get; }
        private string UsernameAppend { get; set; } = "";

        public ConnectionData(string name, string pw, string apikey, string url, bool isHttps, uint connectionAttempts)
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
