using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Conway
{
    public class Coordinate : IEquatable<Coordinate>
    {
        public int x;
        public int y;

        public Coordinate(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public bool Equals([AllowNull] Coordinate other)
        {
            return x == other.x
                && y == other.y;
        }

        public IEnumerable<Coordinate> Neighbors =>
            Enumerable.Range(x - 1, x + 1).Select(x_ =>     // Get X neighbors
            Enumerable.Range(y - 1, y + 1).Select(y_ =>     // Get Y neighbors
                new Coordinate(x_, y_))).SelectMany(s=>s)   // Flatten the arrays
                .Where(c=>c.x != x || c.y != y);            // Exclude the center
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
                                coord = new Coordinate(x, y)
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

        public World(IEnumerable<Cell> cells)
        {
            foreach (var c in cells)
                worldData.Add(c.coord, c);
        }

        public World GetNext() => new World(liveCells.Select(c => GetNeighbors(c)).SelectMany(s => s).Distinct()
                .Select(a =>
                {
                    int neighborCount = GetNeighbors(a).Count(n => n.value == true);
                    return new Cell()
                    {
                        coord = a.coord,
                        // Any live cell with two or three neighbors survives.
                        // Any dead cell with three live neighbors becomes a live cell.
                        // All other live cells die in the next generation. Similarly, all other dead cells stay dead.
                        value = neighborCount == 3 || neighborCount == 2 && a.value
                    };
                }));


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

        public IEnumerable<Cell> GetNeighbors(Cell cell) =>
            cell.coord.Neighbors.Select(c => GetCell(c));

        public bool Equals([AllowNull] World other) => 
            // Ensure we have the same number of live cells,
            // And that for every live cell we have, it equals the value of a live cell in the other world.
            (liveCells.Count() == other.liveCells.Count())
                && liveCells.Select(cell => cell.value == other.GetCell(cell.coord).value).All(comparison => comparison == true);
    }
}
