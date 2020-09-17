using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;


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
            string statsFile = $"{StatsDir}/{player}";
            var statsFileInfo = new FileInfo(statsFile);

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


        public string FormatRaceData(int raceID, int textLength, int WPM, int errors, DateTime startOfRace,
            DateTime endOfRace)
        {
            int time = Convert.ToInt32((endOfRace - startOfRace).TotalSeconds);
            double error = (errors + textLength) / Convert.ToDouble(errors);
            
            return ($"{raceID},,{WPM},,{Math.Round(error,2)},,{time}");
        }

        public void AddRaceData(string player, int raceID, int textLength, int WPM, int errors, DateTime startOfRace,
            DateTime endOfRace)
        {
            string data = FormatRaceData(raceID, textLength, WPM, errors, startOfRace, endOfRace);
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter($"{StatsDir}/{player}", true))
            {
                file.WriteLine(data);
            }
        }

        public int GetNumRaces(string[] datapoints)
        {
            return datapoints.Length;
        }


        public int GetAverageWpm(string[] datapoints)
        {
            int _wpm = 0;
            string[] cache;
            foreach (var line in datapoints)
            {
                cache = line.Split(",,");
                _wpm += Convert.ToInt32(cache[2]);
            }

            return (_wpm/datapoints.Length);
        }

        public double GetAverageErrors(string[] datapoints)
        {
            double allErrors = 0;
            string[] cache;
            foreach (var line in datapoints)
            {
                cache = line.Split(",,");
                allErrors += Convert.ToDouble(cache[3]);
            }

            return Math.Round((allErrors / datapoints.Length),2);
        }
        
    }
}