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
        static int responseA = 0;
        static List<String> players;
        static List<String> finalPlayers;

        static void Main(string[] args)
        {
            PackageHost.Start<Program>(args);
        }

        public override void OnStart()
        {
            PackageHost.WriteInfo("Package starting - IsRunning: {0} - IsConnected: {1}", PackageHost.IsRunning, PackageHost.IsConnected);

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("QCM.js"));

            // calcul du nombre de questions
            if (data != null)
            {
                foreach (var q in data.Data)
                {
                    length++;
                }
            }

            // démarre un timer de 10 secondes pour l'initialisation
            PackageHost.WriteInfo("Beginning of the init phase");
            Timer initTimer = new Timer(10000);
            // à la fin du temps, il envoie une nouvelle question
            initTimer.Elapsed += (sender, e) => EndOfInit(data);
            initTimer.Start();

            PackageHost.
                  CreateMessageProxy(MessageScope.ScopeType.Group, "ClientQCM").
                  InitPlayers();
        }

        [MessageCallback]
        public void NewPlayer(int response)
        {
            PackageHost.WriteInfo(MessageContext.Current.Sender.ConnectionId);
            players.Add(MessageContext.Current.Sender.ConnectionId);
        }

        [MessageCallback]
        public void SendResponse(int response)
        {
            responseA = response;
        }

        // Called when aTimer is finished
        static void EndOfQuestion(object sender, dynamic data)
        {
            // je récupère les réponses
            if (pos != -1)
            {
                PackageHost.WriteInfo("Reponse : {0}", data.Data[pos].Reponses[responseA].Reponse);
                if ((bool) data.Data[pos].Reponses[responseA].BonneReponse)
                {
                    PackageHost.WriteInfo("A a donné la bonne réponse");
                } else {
                    PackageHost.WriteInfo("A a donné la mauvaise réponse");
                }
            }

            // j'envoie une nouvelle question
            pos++;
            if (pos < length)
            {
                responseA = 0; // on réinitialise sa réponse
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
        static void EndOfInit(dynamic data)
        {
            PackageHost.WriteInfo("End of the init phase");
            finalPlayers = players;
            foreach (String p in finalPlayers)
            {
                PackageHost.WriteInfo(p);
            }

            // à démarrer après la phase d'initialisation..
            // démarre un nouveau timer pour chaque question
            Timer qTimer = new Timer(10000);
            // à la fin du temps, il envoie une nouvelle question
            qTimer.Elapsed += (sender, e) => EndOfQuestion(sender, data);
            qTimer.Start();
        }

    }
}

