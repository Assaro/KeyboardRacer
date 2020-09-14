using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace LEA
{
    public class Race
    {
        // TODO: Add reference to players in race
        // TODO: Add way to detect a player ending the race
        // TODO: Add scoreboard composition (first place, second place, etc)

        #region Properties

        public string Text { get; private set; }

        public DateTime StartOfRace { get; set; }

        public List<Player> Participants { get; set; }

        public List<Player> CompletionOrder { get; set; }

        public int CompletionTime { get; set; }

        public bool RaceCompleted { get; set; }

        #endregion

        #region Constructors

        public Race(ref string text)
        {
            Text            = text;
            Participants    = new List<Player>();
            CompletionOrder = new List<Player>();
            Text            = text;
            StartOfRace     = DateTime.Now;
            CompletionTime  = DateTime.Now.AddSeconds(Text.Length * 10.0).Second;
        }

        #endregion


        private void EndRace()
        {
            if (RaceCompleted)
            {
                Console.WriteLine("Already completed");
            }
            else
            {
                RaceCompleted = true;
                Console.WriteLine("completed");
            }
        }


        private async Task CountdownCompletionTime()
        {
            await Task.Delay(CompletionTime   * 1000);
            EndRace();
        }


        public void StartRace()
        {
            var tasks = new List<Task>();

            foreach (var player in Participants)
            {
                tasks.Add(player.TypeText());
            }
            Task.WaitAny(CountdownCompletionTime(), Task.WhenAll(tasks));

            EndRace();
        }
    }
}