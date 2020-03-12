using System;
using Conway;
using Xunit;

namespace Conway.Test
{
    public class TheWorld
    {
        [Fact]
        public void WorldComparison()
        {
            World world1 = new World(new string[]
            {
                "#"
            });
            World world2 = new World(new string[]
            {
                "."
            });

            var world1_ = new World(world1);
            var world2_ = new World(world2);

            Assert.Equal(world1, world1_);
            Assert.Equal(world2, world2_);
            Assert.NotEqual(world1, world2_);
            Assert.NotEqual(world2, world1_);

            Assert.True(world1.GetCell(new Coordinate(0, 0)).value);
            Assert.False(world2.GetCell(new Coordinate(0, 0)).value);
        }

        [Fact]
        public void EmptyWorldDoesNotHaveSpontaneousGeneration()
        {
            World blankWorld = new World(new string[]
                {
                    "...",
                    "...",
                    "..."
                }
            );

            var next = blankWorld.GetNext();

            // Empty worlds should stay empty
            Assert.Equal(blankWorld, next);

            // However, we should not return the same instance
            Assert.NotSame(blankWorld, next);
        }

        [Fact]
        public void SingleCellDiesAlone()
        {
            World world1 = new World(new string[]
                {
                    "...",
                    ".#.",
                    "..."
                }
            );

            World world2 = new World(new string[]
                {
                    "...",
                    "...",
                    "..."
                }
            );

            var next = world1.GetNext();

            // Single nodes should die off
            Assert.Equal(world2, next);

            // We should actually progress
            Assert.NotEqual(world2, world1);
        }

        [Theory]
        [InlineData(
            "Block",

            "....\n" +
            ".##.\n" +
            ".##.\n" +
            "....\n"
            )]
        [InlineData(
            "Bee Hive",

            "......\n" +
            "..##..\n" +
            ".#..#.\n" +
            "..##..\n" +
            "......\n"
            )]
        public void StillLife(string structureName, string worldData)
        {
            World stillLife = new World(worldData.Split('\n'));

            var nextStillLife = stillLife.GetNext();

            // Stable structures should remain
            Assert.Equal(stillLife, nextStillLife);

            Console.WriteLine("\nStill Life:");
            Console.WriteLine($" {structureName} (1): {stillLife.ToString()}");
            Console.WriteLine($" {structureName} (2): {nextStillLife.ToString()}");
        }

        [Theory]
        [InlineData(
            "Blinker",

            "...\n" +
            "###\n" +
            "...\n",

            ".#.\n" +
            ".#.\n" +
            ".#.\n"
            )]
        [InlineData(
            "Toad",

            "....\n" +
            ".###\n" +
            "###.\n" +
            "....\n",

            "..#.\n" +
            "#..#\n" +
            "#..#\n" +
            ".#..\n"
            )]
        public void OscillatorsPeriod2(string structureName, string worldDataA, string worldDataB)
        {
            World oscA = new World(worldDataA.Split('\n'));
            World oscB = new World(worldDataB.Split('\n'));

            Console.WriteLine("\nOscillation (Period 2):");

            Console.WriteLine($"{structureName} (1): {oscA.ToString()}");

            // Assert that A goes to B
            var osc2 = oscA.GetNext();
            Console.WriteLine($"{structureName} (2): {osc2.ToString()}");

            Assert.Equal(oscB, osc2);
            Assert.NotEqual(oscA, osc2);

            // That B goes back to A
            var osc3 = osc2.GetNext();            Console.WriteLine($"{structureName} (3): {osc3.ToString()}");
            Assert.Equal(oscA, osc3);            Assert.NotEqual(oscB, osc3);

            // And that A goes back to B again (for good measure)
            var osc4 = osc3.GetNext();            Console.WriteLine($"{structureName} (4): {osc4.ToString()}");
            Assert.Equal(oscB, osc4);            Assert.NotEqual(oscA, osc4);
        }

        [Theory]
        [InlineData(
            "Glider",

            ".#.\n" +
            "..#\n" +
            "###\n" +
            "...\n",
            4,
            1,
            1
            )]
        public void Spaceships(string structureName, string worldData, int steps, int deltaX, int deltaY)
        {
            World shipBase = new World(worldData.Split('\n'));

            Console.WriteLine($"{structureName} (1): {shipBase.ToString()}");

            World ship = new World(shipBase);

            Console.WriteLine("\nSpaceship:");

            for (int cnt = 1; cnt < steps; cnt++)
            {
                ship = ship.GetNext();
                Console.WriteLine($"{structureName} ({cnt + 1}): {ship.ToString()}");
            }

            World shipShifted = shipBase.GetShifted(new Coordinate(deltaX, deltaY));

            Console.WriteLine($"{structureName} (Original): {shipBase.ToString()}");

            // Ensure we have moved to the new location
            Assert.Equal(shipShifted, ship);
            // Ensure we haven't stayed where we are
            Assert.NotEqual(shipBase, ship);
        }
    }
}
