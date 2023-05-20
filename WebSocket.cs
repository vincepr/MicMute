using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MicroMuteTerminal
{

    // https://thecodegarden.net/websocket-client-dotnet

    internal class WebSocket
    {
        private const Int32 maxMsgSize = 512;       // max-size in bytes one Message can be. Server wont accept bigger.
        private static UTF8Encoding encoding = new UTF8Encoding();

        // Tries 2 times to Upgrade to a WebSocket connection on the supplied uri
        // - uri like "ws://vincepr.de:5555/login_receiver?otp=asdf12-12jfsadfp123kjasdf#g4k3i1" must include the one-time-password
        public static async Task StartWebSocketLoop(ConnectionData data, string one_time_password)
        {
            int count = 0;
            do
            {
                using (var socket = new ClientWebSocket())
                    try
                    {
                        //await ConnectToWebSocket(socket, data, one_time_password);
                        await socket.ConnectAsync(new Uri(data.ToWebSocketUrl()+"?otp="+one_time_password), CancellationToken.None);
                        // await Send(socket, "data");      // no need to send data to server at the moment
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(data.ToSuccessMessage());
                        Console.ResetColor();
                        await Receive(socket);
                    }
                    catch (Exception ex)
                    {
                        ChangeUsernameIfAlreadyTaken(ex, data, one_time_password);
                        Console.WriteLine($"WebSocket-ERROR - {ex.Message}");
                    }
                    finally
                    {
                        count++;
                        if (socket != null) socket.Dispose();
                    }

            } while (count < 2);
        }

        /// <summary>
        /// this throws if conditions are met. Also side-effect changes the Username.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="data"></param>
        /// <param name="otp"></param>
        /// <exception cref="Exception"></exception>
        public static void ChangeUsernameIfAlreadyTaken(Exception ex, ConnectionData data, string otp)
        {
            if (ex is WebSocketException
                    && ((WebSocketException)ex).WebSocketErrorCode == WebSocketError.NotAWebSocket
                    && ex.Message.Contains("409")       // Status: 409 - Body "username already in use"   -  expected
                    )
            {
                data.RNGUsername(otp);
                throw new Exception("Username already taken, trying new username: " + data.Username);
            }
        }

        // Sends the string to the server using the WS
        private static async Task Send(ClientWebSocket socket, string data)
        {
            await socket.SendAsync(
                encoding.GetBytes(data), 
                WebSocketMessageType.Text, 
                true, 
                CancellationToken.None
            );
        }

        // handler that keeps loop active to receive Events/Messages from the open WS-Connection to the Server.
        private static async Task Receive(ClientWebSocket socket)
        {
            var buf = new ArraySegment<byte>(new byte[maxMsgSize]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    // Receive (big messages) in pices:
                    do
                    {
                        result = await socket.ReceiveAsync(buf, CancellationToken.None);
                            #pragma warning disable CS8604 // Mögliches Nullverweisargument. Were in a try catch anyways
                        ms.Write(buf.Array, buf.Offset, result.Count);
                            #pragma warning restore CS8604 // Mögliches Nullverweisargument.
                    } while (!result.EndOfMessage);


                    // If we receive the Close Message we know Server is shutting down:
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    // We Print out our message for now:
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, encoding))
                    {
                        var txt = await reader.ReadToEndAsync();
                        Event.Handle(txt);

                    }
                }
            } while (true);
        }
    }
}
