using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMuteTerminal
{
    internal class Event
    {
        // this is the format the Server API sends us via websocket:
        internal class EventResponse
        {
            public string type { get; }
            public SignalToReceiver payload { get; }
            public EventResponse(string type, SignalToReceiver payload)
            {
                this.type = type;
                this.payload = payload;
            }
        }

        // this is wrapped into the Payload of the EventType class
        internal class SignalToReceiver
        {
            // the id contains the actual Signal we 
            public string signal { get; }
            public SignalToReceiver(string signal)
            {
                this.signal = signal;
            }
        }

        // logic on what to do once we receive a msg over the Websocket.
        public static void Handle(string response)
        {
            var ev = JsonConvert.DeserializeObject<EventResponse>(response);
            if (ev is null || ev.payload is null || ev.payload.signal is null)
            {
                Console.WriteLine("Bad Payload in Respose: " + response);
                return;
            }
            trySignal(ev.payload.signal);
        }

        private static void trySignal(string signal)
        {
            switch (signal)
            {
                case "vol_down":
                    Audio.Volume_Down(); break;
                case "vol_up":
                    Audio.Volume_Up(); break;
                case "vol_toggle":
                    Audio.Volume_Toggle(); break;
                case "mic_down":
                    Audio.Mic_Volume_Down(); break;
                case "mic_up":
                    Audio.Mic_Volume_Up(); break;
                case "mic_toggle":
                    Audio.Mic_Mute(); break;
                default: 
                    Console.WriteLine("Received unsupported Signal:" + signal); return;
            }
            Console.WriteLine(">" + signal);
        }
    }

}
