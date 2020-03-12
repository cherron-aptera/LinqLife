using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Conway
{
    public struct Coordinate
    {
        public int x;
        public int y;
    }

    public struct Cell
    {
        public bool value;
        public Coordinate coord;
    }

    public class World : IEquatable<World>
    {
        private Dictionary<Coordinate, Cell> worldData = new Dictionary<Coordinate, Cell>();

        public World(string[] initData = null)
        {
            int x, y = 0;

            foreach (string row in initData)
            {
                x = 0;
                foreach (char cellChar in row)
                {
                    if(cellChar.Equals('#'))
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

        public World GetNext()
        {
            return new World();
        }

        public bool Equals([AllowNull] World other)
        {
            // TODO: Compare worldData
            return true;
        }
    }
}
