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

        [Fact]
        public void StillLife()
        {
            World block = new World(new string[]
                {
                    "....",
                    ".##.",
                    ".##.",
                    "....",
                }
            );

            World beeHive = new World(new string[]
                {
                    "......",
                    "..##..",
                    ".#..#.",
                    "..##..",
                    "......",
                }
            );

            Assert.Equal(1, block.GetLiveNeighborCount(new Coordinate(0, 0)));
            Assert.Equal(3, block.GetLiveNeighborCount(new Coordinate(1, 1)));

            var nextBlock = block.GetNext();
            var nextBeeHive = beeHive.GetNext();

            // Stable structures should remain
            Assert.Equal(block, nextBlock);
            Assert.Equal(beeHive, nextBeeHive);

            // Arbitrary structures should not be equivalent
            Assert.NotEqual(beeHive, nextBlock);
            Assert.NotEqual(block, nextBeeHive);
        }

        [Fact]
        public void OscillatorsPeriod2()
        {
            World blinkerA = new World(new string[]
                {
                    "...",
                    "###",
                    "...",
                }
            );
            World blinkerB = new World(new string[]
                {
                    ".#.",
                    ".#.",
                    ".#.",
                }
            );

            // Assert that A goes to B
            var blinker2 = blinkerA.GetNext();

            Assert.Equal(blinkerB, blinker2);
            Assert.NotEqual(blinkerA, blinker2);

            // That B goes back to A
            var blinker3 = blinker2.GetNext();

            Assert.Equal(blinkerA, blinker3);
            Assert.NotEqual(blinkerB, blinker3);

            // And that A goes back to B again (for good measure)
            var blinker4 = blinker3.GetNext();

            Assert.Equal(blinkerB, blinker4);
            Assert.NotEqual(blinkerA, blinker4);
        }
    }
}
