using System;

namespace Conway
{
    class Program
    {
        const string Osc1 =
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
            "..###..\n";

        const string Acorn =
            ".#.....\n" +
            "...#...\n" +
            "##..###\n";

        const string RPentomino =
            ".##\n" +
            "##.\n" +
            ".#.\n";

        const string Diehard =
            "......#.\n" +
            "##......\n" +
            ".#...###\n";

        const string Gosper =
                "........................#............\n" +
                "......................#.#.............\n" +
                "............##......##............##.....\n" +
                "...........#...#....##............##......\n" +
                "##........#.....#...##.................\n" +
                "##........#...#.##....#.#................\n" +
                "..........#.....#.......#.............\n" +
                "...........#...#......................\n" +
                "............##.........................\n";

        static void Main(string[] args)
        {
            bool running = true;

            var world = new World(Osc1);

            while (running)
            {
                // Check for exit
                if (Console.KeyAvailable)
                {
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.Escape:
                        case ConsoleKey.Q:
                            running = false;
                            break;
                        case ConsoleKey.D1:
                            world = new World(Osc1);
                            break;
                        case ConsoleKey.D2:
                            world = new World(Acorn);
                            break;
                        case ConsoleKey.D3:
                            world = new World(RPentomino);
                            break;
                        case ConsoleKey.D4:
                            world = new World(Diehard);
                            break;
                        case ConsoleKey.D5:
                            world = new World(Gosper);
                            break;
                    }
                }

                Console.Clear();
                Console.WriteLine(world);
                System.Threading.Thread.Sleep(50);
                world = world.GetNext();
            }


        }
    }
}
