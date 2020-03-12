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

            var nextBlock = block.GetNext();
            var nextBeeHive = beeHive.GetNext();

            Console.WriteLine(block);
            Console.WriteLine(nextBlock);

            // Stable structures should remain
            Assert.Equal(block, nextBlock);
            Assert.Equal(beeHive, nextBeeHive);

            // Arbitrary structures should not be equivalent
            Assert.NotEqual(beeHive, nextBlock);
            Assert.NotEqual(block, nextBeeHive);
        }

    }
}
