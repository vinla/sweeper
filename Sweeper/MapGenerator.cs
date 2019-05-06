using System;
using Sweeper.GameObjects;

namespace Sweeper
{
    public static class MapGenerator
    {
        public static Map Generate(int width, int height, int difficulty, MainScene scene)
        {
            var rng = new Random();
            var map = new Map(width, height, scene);

            for(int i = 0; i < Math.Min(difficulty + 5, width); i++)
            {
                for(int j = 0; j < Math.Min(difficulty + 5, height); j++)
                {
                    map.GetTileAt(i, j).Modifier = new Empty();
                }
            }


            for(int i = 0; i < difficulty;)
            {
                var x = rng.Next(0, width);
                var y = rng.Next(0, height);

                if( x > 2 || y > 2 )
                {
                    var rt = map.GetTileAt(x, y);
                    if (rt.Modifier is Empty)
                    {
                        rt.Modifier = new Node();
                        i++;
                    }
                }
            }

            var uplinkCount = 0;
            var bitCoinCount = 0;
            var decryptorCount = 0;

            for (int i = 0; i < map.Width; i++)
                for (int j = 0; j < map.Height; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    var diceRoll = rng.NextDouble();
                    var target = map.GetTileAt(i, j);
                    var encDiff = 1 - (MainScene.Difficulty / 250f);
                    if(target.Modifier is Empty && target.DiscoveredNodes > 0 && diceRoll > encDiff)
                    {
                        target.Modifier = new Encrypted();
                    }
                    else if(target.Modifier is Empty && target.DiscoveredNodes == 0)
                    {                        
                        if (diceRoll < 0.025 && uplinkCount < 3)
                        {
                            target.Modifier = new Uplink();
                            uplinkCount++;
                        }
                        else if (diceRoll < 0.015 && decryptorCount < 1)
                        {
                            target.Modifier = new Decryptor();
                            decryptorCount++;
                        }

                        if (diceRoll > 0.9 && uplinkCount < 10)
                        {
                            target.Modifier = new BitCoin();
                            bitCoinCount++;
                        }
                    }
                }            
            
            return map;
        }
    }

    public class GameContext
    {
        public int CurrentLevel { get; set; }
    }
}
