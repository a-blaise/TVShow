using Constellation;
using Constellation.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace Quiz
{
    public class Program : PackageBase
    { 
        static int length = 0;
        static int pos = -1;
        static List<string> players = new List<string>();
        static List<Player> finalPlayers = new List<Player>();

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("QCM.js"));

            // Computes the number of questions
            if (data != null)
            {
                foreach (var q in data.Data)
                {
                    length++;
                }
            }

            // Starts a 10-second timer for initialization : players have 10 seconds to click on one button
            PackageHost.WriteInfo("Beginning of the init phase");
            PackageHost.
                 CreateMessageProxy(MessageScope.ScopeType.Group, "ClientQCM").
                 InitPlayers();

            Timer initTimer = new Timer(10000);
            // à la fin du temps, il envoie une nouvelle question
            initTimer.Elapsed += (sender, e) => EndOfInit(sender, data);
            initTimer.Start();
        }

        [MessageCallback]
        public void NewPlayer(int response)
        {
            players.Add(MessageContext.Current.Sender.ConnectionId);
        }

        [MessageCallback]
        public void SendResponse(int response)
        {
            foreach (Player p in finalPlayers)
            {
                if (p.ConnectionId.Equals(MessageContext.Current.Sender.ConnectionId))
                {
                    p.CurrentReponse = response;
                }
            }
        }

        // Called when qTimer is finished
        static void EndOfQuestion(object sender, dynamic data)
        {
            // Retrieve responses
            if (pos != -1)
            {
                foreach (Player p in finalPlayers) {
                    if (p.CurrentReponse != - 1)
                    {
                        if ((bool)data.Data[pos].Reponses[p.CurrentReponse].BonneReponse)
                        {
                            PackageHost.WriteInfo("{0} a donné la bonne réponse {1}", p.ConnectionId, data.Data[pos].Reponses[p.CurrentReponse].Reponse);
                            p.Score++;
                        }
                        else
                        {
                            PackageHost.WriteInfo("{0} a donné la mauvaise réponse {1}", p.ConnectionId, data.Data[pos].Reponses[p.CurrentReponse].Reponse);
                        }
                    }
                    else
                    {
                        PackageHost.WriteInfo("{0} n'a pas répondu", p.ConnectionId);
                    }
                }
            }

            // Sends a new question
            pos++;
            if (pos < length)
            {
                // Reinitialization of all responses
                foreach (Player p in finalPlayers)
                {
                    p.CurrentReponse = -1;
                }

                PackageHost.WriteInfo(data.Data[pos].Label);
                PackageHost.
                    CreateMessageProxy(MessageScope.ScopeType.Group, "ClientQCM").
                    SendQuestion(data.Data[pos]);
            } else
            {
                Timer aTimer = (Timer)sender;
                aTimer.Stop();
            }
        }

        // 
        static void EndOfInit(object sender2, dynamic data)
        {
            PackageHost.WriteInfo("End of the init phase");
            PackageHost.WriteInfo("The current players are : ");
            foreach (String p in players)
            {
                PackageHost.WriteInfo(p);
                Player player = new Player(p);
                finalPlayers.Add(player);
            }

            Timer initTimer = (Timer)sender2;
            initTimer.Stop();
            
            // Start a new timer for each question
            Timer qTimer = new Timer(10000);
            // At the end of the time, he retrieves the responses and sends a new question
            qTimer.Elapsed += (sender, e) => EndOfQuestion(sender, data);
            qTimer.Start();
        }
    }

    internal class Player
    {
        public string ConnectionId { get; set; }
        public int CurrentReponse { get; set; }
        public int Score { get; set; }

        public Player(string connectionId)
        {
            this.ConnectionId = connectionId;
            this.CurrentReponse = -1;
            this.Score = 0;
        }
    }
}