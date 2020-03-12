using System;
using System.Collections.Generic;

namespace Conway
{
    class Program
    {
        public struct WorldSeed
        {
            public string title;
            public string seed;
        }

        public static Dictionary<ConsoleKey, WorldSeed> Seeds = new Dictionary<ConsoleKey, WorldSeed>()
        {
            {
                ConsoleKey.D1,
                new WorldSeed()
                {
                title = "Penta-decathlon Oscillator",
                seed =
                "..###..\n" +
                ".#...#.\n" +
                ".#...#.\n" +
                "..###..\n" +
                ".......\n" +
                ".......\n" +
                ".......\n" +
                ".......\n" +
                "..###..\n" +
                ".#...#.\n" +
                ".#...#.\n" +
                "..###..\n"
                }
            },
            {
                ConsoleKey.D2,
                new WorldSeed()
                {
                title = "Acorn",
                seed =
                ".....#.\n" +
                "...#...\n" +
                "###..##\n"
                }
            },
            {
                ConsoleKey.D3,
                new WorldSeed()
                {
                title = "R-Pentomino",
                seed =
                ".##\n" +
                "##.\n" +
                ".#.\n"
                }
            },
            {
                ConsoleKey.D4,
                new WorldSeed()
                {
                title = "Diehard",
                seed =
                "......#.\n" +
                "##......\n" +
                ".#...###\n"
                }
            },
            {
                ConsoleKey.D5,
                new WorldSeed()
                {
                title = "Gosper Glider-Gun",
                seed =
                "........................#............\n" +
                "......................#.#.............\n" +
                "............##......##............##.....\n" +
                "...........#...#....##............##......\n" +
                "##........#.....#...##.................\n" +
                "##........#...#.##....#.#................\n" +
                "..........#.....#.......#.............\n" +
                "...........#...#......................\n" +
                "............##.........................\n"
                }
            }
        };

        static void Main(string[] args)
        {
            bool running = true;
            string title = "Empty World";

            var world = new World();

            while (running)
            {
                // Check for exit
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

                    if (Seeds.ContainsKey(key))
                    {
                        var seed = Seeds[key];
                        title = seed.title;
                        world = new World(seed.seed);
                    }
                    else if (key == ConsoleKey.Escape ||
                        key == ConsoleKey.Q)
                    {
                        running = false;
                    }
                }

                Console.Clear();
                Console.WriteLine("Press 1-5 to load a seed world, or Q to quit");
                Console.WriteLine(title);
                Console.WriteLine(world);
                System.Threading.Thread.Sleep(50);
                world = world.GetNext();
            }


        }
    }
}
