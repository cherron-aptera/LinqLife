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

        private IEnumerable<Cell> liveCells => worldData.Values.Where(c => c.value);

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
            // TODO: Return something other than an empty world
            return new World();
        }

        public Cell GetCell(Coordinate coord)
        {
            Cell cell;
            if (!worldData.TryGetValue(coord, out cell))
            {
                return new Cell()
                {
                    coord = coord,
                    value = false
                };
            }
            return cell;
        }

        public bool Equals([AllowNull] World other)
        {
            // Ensure we have the same number of live cells,
            // And that for every live cell we have, it equals the value of a live cell in the other world.
            return (liveCells.Count() == other.liveCells.Count())
                && liveCells.Select(cell => cell.value == other.GetCell(cell.coord).value).All(comparison => comparison == true);
        }
    }
}
