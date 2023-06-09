﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMuteTerminal
{
    // json-blueprint for the Request we send to the Server
    public class LoginRequest
    {
        public string apikey;
        public string username;
        public string password;
        public LoginRequest(string apikey, string username, string password)
        {
            this.apikey = apikey;
            this.username = username;
            this.password = password;
        }
    }
    
    // json-blueprint for the Response we get From the Server
    public class LoginResponse
    {
        // otp is the One-Time-Password get from server -> use as credential for upgrading to Websocket.
        public string otp { get; set; }
        public LoginResponse(string otp) { this.otp = otp; }
    }

    public class Login
    {

        /// <summary>
        /// starts specified ammount of login attempts. If successful it will open the Websocket-Loop
        /// - if max_attempts==0 it will repeat infinite
        /// </summary>

        public static async Task StartLoginLoop(ConnectionData data)
        {
            uint count = 0;
            uint max_attempts = data.ConnectionAttempts;
            do
            {
                try
                {
                    string otp = await Make_Request(data);
                    await WebSocket.StartWebSocketLoop(data, otp);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Mainloop-ERROR: " + ex.Message);
                }
                finally
                {
                    count++;
                }
            } while (max_attempts == 0 || count < max_attempts);
            // C_A_T==0 -> infinite retries, otherwise stop after x ammount of reconnect attempts:

        }

        /// <summary>
        /// Make a request to the server with credentials provided. Server validates them and sends back a One-Time-Password (otp).
        /// </summary>
        private static async Task<string> Make_Request(ConnectionData data)
        {
            HttpClient client = new HttpClient();
            string json = JsonConvert.SerializeObject(data.ToLoginRequest());
            HttpContent content = new StringContent(json);
            HttpResponseMessage? response = await client.PostAsync(data.ToLoginUrl(), content);
            if (response == null) { throw new Exception("Error - No Response received!"); }
            if (response.IsSuccessStatusCode)
            {
                string resp_json = await response.Content.ReadAsStringAsync();
                var login = JsonConvert.DeserializeObject<LoginResponse>(resp_json);
                if (login is null || login.otp == null)
                    throw (new Exception("Error - failed to parse json: " + response));
                return login.otp;
            }
            throw new Exception("Error - Bad response. \n\tCode: " +response.StatusCode+"\n\tContent: "  + response.Content);
        }
    }
    
}
