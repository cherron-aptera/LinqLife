using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Conway
{
    public struct Coordinate : IEquatable<Coordinate>
    {
        public int x;
        public int y;

        public bool Equals([AllowNull] Coordinate other)
        {
            return x == other.x
                && y == other.y;
        }
    }

    public struct Cell : IEquatable<Cell>
    {
        public bool value;
        public Coordinate coord;

        public bool Equals([AllowNull] Cell other)
        {
            return value == other.value
                && coord.Equals(other.coord);
        }
    }

    public class World : IEquatable<World>
    {
        private Dictionary<Coordinate, Cell> worldData = new Dictionary<Coordinate, Cell>();

        public World(string[] initData = null)
        {
            int x, y = 0;

            if (initData != null)
            {
                foreach (string row in initData)
                {
                    x = 0;
                    foreach (char cellChar in row)
                    {
                        if (cellChar.Equals('#'))
                        {
                            var cell = new Cell()
                            {
                                value = true,
                                coord = new Coordinate()
                                {
                                    x = x,
                                    y = y
                                },
                            };
                            worldData.Add(cell.coord,
                                cell);
                        }
                        x++;
                    }
                    y++;
                }
            }
        }

        public World GetNext()
        {
            return new World();
        }

        public bool Equals([AllowNull] World other)
        {
            // Look at all live cells
            var hots = worldData.Values.Where(c => c.value);
            var otherHots = other.worldData.Values.Where(c => c.value);

            // Ensure that we have the same number of live cells
            if (hots.Count() != otherHots.Count())
                return false;

            // Ensure that for each live cell, there is a corresponding live cell in the other list
            foreach (Cell c in hots)
            {
                Cell oc;
                if (!other.worldData.TryGetValue(c.coord, out oc))
                    return false;

                if (c.value != oc.value)
                    return false;
            }

            return true;
        }
    }
}
