using System;
using System.IO;
using System.Linq;


namespace LEA
{
    public class Stats
    {
        //TODO:ADD loadPlayerStatistic void
        //TODO:ADD updatePlayerStatistic void


        private const string   StatsDir = "../../../data/statistics";
        private       double   _avgErrors;
        private       int      _avgWpm;
        private       string[] _datapoints;
        private       string   _name;
        private       int      _races;

        #region Properties

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string[] Datapoints
        {
            get => _datapoints;
            set => _datapoints = value;
        }

        public int Races
        {
            get => _races;
            set => _races = value;
        }

        public int AvgWpm
        {
            get => _avgWpm;
            set => _avgWpm = value;
        }

        public double AvgErrors
        {
            get => _avgErrors;
            set => _avgErrors = value;
        }

        #endregion


        /// <summary>
        /// <para>Returns:</para>
        /// True if the player has a non-empty statistics file, false otherwise
        /// </summary>
        /// <param name="player">
        /// Who's statistics file to check for
        /// </param>
        /// <returns>
        /// True if the player has a non-empty statistics file, false otherwise
        /// </returns>
        public static bool HasStatistics(string player)
        {
            string statsFile     = $"{StatsDir}/{player}";
            var    statsFileInfo = new FileInfo(statsFile);

            return File.Exists(statsFile) && statsFileInfo.Length != 0;
        }


        /// <summary>
        /// <para>
        /// Returns:
        /// </para>
        /// A list of player names who have existing and non-empty statistics files
        /// </summary>
        /// <returns>
        /// A list of player names who have existing and non-empty statistics files
        /// </returns>
        public string[] GetPlayerNames()
        {
            return Directory
                  .GetFiles(StatsDir)
                  .Select(Path.GetFileName)
                  .Where(HasStatistics)
                  .ToArray();
        }


        /// <summary>
        /// Form a datapoint with the PostGameStats object's members in the desired format for writing
        /// <para>Returns:</para>
        /// A datapoint in a ready-to-write format
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        private string FormatRaceData(PostGameStats stats)
        {
            int    raceDuration = Convert.ToInt32((stats.EndOfRace - stats.StartOfRace).TotalSeconds);
            double errorRate    = stats.TotalErrors / (double) (stats.TextLength + stats.TotalErrors);

            return ($"{stats.RaceId},,{stats.Wpm},,{Math.Round(errorRate, 2)},,{raceDuration}");
        }


        /// <summary>
        /// Take a stats object, use it to form a datapoint and write the datapoint to a players statistics file
        /// </summary>
        /// <param name="stats">A PostGameStats object to take values from</param>
        public void AddRaceData(PostGameStats stats)
        {
            string data = FormatRaceData(stats);

            StreamWriter file = new StreamWriter($"{StatsDir}/{stats.Name}", true);

            file.WriteLine(data);
        }


        public int GetNumRaces(string[] datapoints)
        {
            return datapoints.Length;
        }


        public int GetAverageWpm(string[] datapoints)
        {
            int wpm = 0;

            foreach (var line in datapoints)
            {
                var items = line.Split(",,");
                wpm += Convert.ToInt32(items[2]);
            }

            return (int) Math.Round(wpm / (double) datapoints.Length, 2);
        }


        public double GetAverageErrorRate(string[] datapoints)
        {
            double sumErrorRate = 0;

            foreach (var line in datapoints)
            {
                var items = line.Split(",,");
                sumErrorRate += Convert.ToDouble(items[3]);
            }

            return Math.Round(sumErrorRate / (double) datapoints.Length, 2);
        }
    }
}