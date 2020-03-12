using System;
using Conway;
using Xunit;

namespace Conway.Test
{
    public class TheWorld
    {
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

            var nextBlock = block.GetNext();
            var nextBeeHive = beeHive.GetNext();

            // Stable structures should remain
            Assert.Equal(nextBlock, block);
            Assert.Equal(nextBeeHive, beeHive);

            // Arbitrary structures should not be equivalent
            Assert.NotEqual(nextBlock, beeHive);
            Assert.NotEqual(nextBeeHive, block);
        }

    }
}
