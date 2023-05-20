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
        static async Task Main(string[] args)
        {
            // Try Parse Terminal flags or failback to defaults for data we run on:
            ConnData data;
            try {
                data = Cmd.Init(args);
            }
            catch (Exception ex) { 
                Console.WriteLine(ex);
                return;
            }
            Console.WriteLine("Running MicroMute in the Terminal");
            // start the main loop:
            await Login.StartLoginLoop(data);
        }
    }

    
}

