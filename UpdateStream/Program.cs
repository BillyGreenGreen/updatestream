using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api;
using System.IO;
using System.Diagnostics;
using System.Net.Http;

namespace UpdateStream
{
    class Program
    {
        private static TwitchAPI api;
        private static Dictionary<string, string> gameDict = new Dictionary<string, string>();
        private static List<Process[]> processes = new List<Process[]>();
        
        static async Task Main(string[] args)
        {
            api = new TwitchAPI();
            api.Settings.ClientId = "";
            api.Settings.AccessToken = "";
            var gamesFile = File.ReadAllLines("games.txt");
            List<string> games = new List<string>(gamesFile);
            var channel = await api.V5.Channels.GetChannelAsync();
            string title = channel.Status;
            int gameCount = 0;
            var dictFile = File.ReadAllLines("dict.txt");            

            for (int i = 0; i < dictFile.Length; i += 2)
            {
                gameDict.Add(dictFile[i], dictFile[i + 1]);
            }

            for (int i = 0; i < dictFile.Length; i += 2)
            {
                processes.Add(Process.GetProcessesByName(dictFile[i]));
            }

            Console.WriteLine("List of compatible games");
            Console.WriteLine("------------------------");
            for (int i = 0; i < dictFile.Length; i += 2)
            {
                Console.WriteLine(dictFile[i + 1]);
            }

            while (true)
            {
                
                Process[] gamePlaying = Process.GetProcessesByName("chrome");
                List<Process[]> processes2 = processes;
                System.Threading.Thread.Sleep(1000);
                foreach (Process[] game in processes)
                {
                    if (game.Length > 0)
                    {
                        gameCount += 1;
                        gamePlaying = game;
                        break;
                    }

                    if (game.Length == 0)
                    {
                        gameCount = 0;
                    }
                }

                if (gameCount == 0)
                {
                    var newChannel = await api.V5.Channels.GetChannelAsync();
                    if (newChannel.Game != "Just Chatting")
                    {
                        await Task.Delay(10000);
                        await api.V5.Channels.UpdateChannelAsync("56302578", title, "Just Chatting", "10", false);
                    }
                }
                else
                {
                    string game1 = gamePlaying[0].ToString().Substring(28);
                    string gameID = game1.Substring(0, game1.Length - 1);
                    await Task.Delay(10000);
                    await api.V5.Channels.UpdateChannelAsync("56302578", title, gameDict[gameID], "10", false);
                }
                
                processes.Clear();
                for (int i = 0; i < dictFile.Length; i += 2)
                {
                    processes.Add(Process.GetProcessesByName(dictFile[i]));
                }
            }
        }
    }
}
