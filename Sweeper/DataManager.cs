using System;
using System.IO;

namespace Sweeper
{
    public static class DataManager
    {
        private static string UserFile(string gameType) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "BitShifter", $"{gameType}.txt");
        public static int ReadHighScore(string gameType)
        {
            var file = UserFile(gameType);

            if (File.Exists(file))
                return int.Parse(File.ReadAllText(file));
            else
                return 0;
        }

        public static void SaveHighScore(int score, string gameType)
        {
            try
            {
                var file = UserFile(gameType);
                if (Directory.Exists(Path.GetDirectoryName(file)) == false)
                    Directory.CreateDirectory(Path.GetDirectoryName(file));
                File.WriteAllText(file, score.ToString());
            }
            catch(Exception)
            {
                // Don't blow up because we can't write the score
            }
        }
    }
}
