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
        dynamic data;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            data = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("QCM.js"));

            // Computes the number of questions
            if (data != null)
            {
                foreach (var q in data.Data)
                {
                    length++;
                }
            }

            // Starts a 10-second timer for initialization : players have 10 seconds to click on one button
            PackageHost.WriteInfo("Click on a button to play");
            PackageHost.
                 CreateMessageProxy(MessageScope.ScopeType.Group, "ClientQCM").
                 InitPlayers();

            Timer initTimer = new Timer(10000);
            // à la fin du temps, il envoie une nouvelle question
            initTimer.Elapsed += (sender, e) => EndOfInit(sender, data);
            initTimer.Start();
        }

        [MessageCallback]
        public void NewPlayer(int answer)
        {
            players.Add(MessageContext.Current.Sender.ConnectionId);
            PackageHost.WriteInfo("{0} is playing", MessageContext.Current.Sender.ConnectionId);
        }

        [MessageCallback]
        public void SendAnswer(int answer)
        {
            foreach (Player p in finalPlayers)
            {
                if (p.ConnectionId.Equals(MessageContext.Current.Sender.ConnectionId))
                {
                    PackageHost.WriteInfo("Answer {0} from {1}", data.Data[pos].Answers[answer].Answer, p.ConnectionId);
                    p.CurrentAnswer = answer;
                }
            }
        }

        // Called when qTimer is finished
        static void EndOfQuestion(object sender, dynamic data)
        {
            // Retrieve answers
            if (pos != -1)
            {
                foreach (Player p in finalPlayers) {
                    if (p.CurrentAnswer != - 1)
                    {
                        if ((bool)data.Data[pos].Answers[p.CurrentAnswer].GoodAnswer)
                        {
                            PackageHost.WriteInfo("{0} gave a good answer", p.ConnectionId);
                            p.Score++;
                        }
                        else
                        {
                            PackageHost.WriteInfo("{0} gave a wrong answer", p.ConnectionId);
                        }
                    }
                    else
                    {
                        PackageHost.WriteInfo("{0} does not answered", p.ConnectionId);
                    }
                }
            }

            // Sends a new question
            pos++;
            if (pos < length)
            {
                // Reinitialization of all answers
                foreach (Player p in finalPlayers)
                {
                    p.CurrentAnswer = -1;
                }

                PackageHost.WriteInfo(data.Data[pos].Label);
                PackageHost.
                    CreateMessageProxy(MessageScope.ScopeType.Group, "ClientQCM").
                    SendQuestion(data.Data[pos]);
            } else
            {
                PackageHost.WriteInfo("The scores are : ");
                foreach (Player p in finalPlayers)
                {
                    PackageHost.WriteInfo("{0} out of {1} for {2}", p.Score, length, p.ConnectionId);
                }
                Timer aTimer = (Timer)sender;
                aTimer.Stop();
            }
        }

        // 
        static void EndOfInit(object sender2, dynamic data)
        {
            PackageHost.WriteInfo("The game begins !");
            PackageHost.WriteInfo("The players are : ");
            foreach (String p in players)
            {
                PackageHost.WriteInfo(p);
                Player player = new Player(p);
                finalPlayers.Add(player);
            }

            Timer initTimer = (Timer)sender2;
            initTimer.Stop();

            if (finalPlayers.Count > 0)
            {
                // Start a new timer for each question
                Timer qTimer = new Timer(10000);
                // At the end of the time, he retrieves the answers and sends a new question
                qTimer.Elapsed += (sender, e) => EndOfQuestion(sender, data);
                qTimer.Start();
            }
            else PackageHost.WriteInfo("No players");
        }
    }

    internal class Player
    {
        public string ConnectionId { get; set; }
        public int CurrentAnswer { get; set; }
        public int Score { get; set; }

        public Player(string connectionId)
        {
            this.ConnectionId = connectionId;
            this.CurrentAnswer = -1;
            this.Score = 0;
        }
    }
}