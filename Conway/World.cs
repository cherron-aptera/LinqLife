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
            NeighborsAndSelf.Where(c=>c.x != x || c.y != y);   // Exclude the center

        public IEnumerable<Coordinate> NeighborsAndSelf =>
            Enumerable.Range(x - 1, x + 1).Select(x_ =>        // Get X neighbors
            Enumerable.Range(y - 1, y + 1).Select(y_ =>        // Get Y neighbors
                new Coordinate(x_, y_))).SelectMany(s => s);   // Flatten the arrays
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

        private IEnumerable<Cell> liveCells => worldData.Values.Where(c => c.value == true);

        public override string ToString()
        {
            var minX = worldData.Values.Min(c => c.coord.x) - 1;
            var maxX = worldData.Values.Max(c => c.coord.x) + 1;
            var minY = worldData.Values.Min(c => c.coord.y) - 1;
            var maxY = worldData.Values.Max(c => c.coord.y) + 1;

            var dX = maxX - minX;
            var dY = maxY - minY;

            var live = liveCells.ToList();

            return $"Live Cells: {live.Count()}\n" +
                   $"({minX}, {minY}) - ({maxX}, {maxY})\n" +
                String.Join("\n", Enumerable.Range(minY, dY+1).Select(y_ =>
                String.Join(" ", Enumerable.Range(minX, dX+1).Select(x_ =>
                    GetCell(new Coordinate(x_, y_))
                        .value == true ? "#" : ".")
                .ToArray())));
        }

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

        public World(World copy) : this(copy.worldData.Values)
        {

        }

        public World(IEnumerable<Cell> cells)
        {
            foreach (var c in cells)
                worldData.Add(c.coord, c);
        }

        public World GetNext() =>
            new World(
                liveCells.Select(c => GetNeighborsAndSelf(c)).SelectMany(s => s).Distinct() // A flattened array of all distinct "interesting" nodes
                    .Select(a =>
                    {
                        // For each cell that has a live neighbor
                        int neighborCount = GetLiveNeighborCount(a.coord);
                        return new Cell()
                        {
                            coord = a.coord,
                            // Any live cell with two or three neighbors survives.
                            // Any dead cell with three live neighbors becomes a live cell.
                            // All other live cells die in the next generation. Similarly, all other dead cells stay dead.
                            value = (neighborCount == 3) || (neighborCount == 2 && a.value)
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

        public int GetLiveNeighborCount(Coordinate coord)
        {
            return GetNeighbors(coord).Count(c => c.value == true);
        }

        public IEnumerable<Cell> GetNeighbors(Coordinate coord) =>
            coord.Neighbors.Select(c => GetCell(c));

        public IEnumerable<Cell> GetNeighborsAndSelf(Cell cell) =>
            cell.coord.NeighborsAndSelf.Select(c => GetCell(c));

        public bool Equals([AllowNull] World other) => 
            // Ensure we have the same number of live cells,
            // And that for every live cell we have, it equals the value of a live cell in the other world.
            (liveCells.Count() == other.liveCells.Count())
                && liveCells.Select(cell => cell.value == other.GetCell(cell.coord).value).All(v => v == true);
    }
}
