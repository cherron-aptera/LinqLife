﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Conway
{
    public class Coordinate : IEquatable<Coordinate>
    {
        public int x { get; }
        public int y { get; }

        public Coordinate(int X, int Y) 
        {
            x = X;
            y = Y;
        }

        public IEnumerable<Coordinate> NeighborsAndSelf =>
            Enumerable.Range(x - 1, 3).Select(xNeighbor =>  // Iterate xNeighbor over x-1 to x+1
            Enumerable.Range(y - 1, 3).Select(yNeighbor =>  // Iterate yNeighbor over y-1 to y+1
                    new Coordinate(xNeighbor, yNeighbor)    // Create a coordinate for xNeighbor and yNeighbor
                )).SelectMany(s => s);                      // Flatten the arrays

        public IEnumerable<Coordinate> Neighbors =>
            NeighborsAndSelf.Where(c=>c.x != x || c.y != y);   // Exclude the center

        public bool Equals([AllowNull] Coordinate other)
        {
            return x == other.x
                && y == other.y;
        }

        public override string ToString()
        {
            return $"{x},{y}";
        }

        public override int GetHashCode()
        {
            return x + y;
        }

        public Coordinate Plus(Coordinate other) => new Coordinate(x + other.x, y + other.y);
    }

    public struct Cell : IEquatable<Cell>
    {
        public bool value { get; }
        public Coordinate coord { get; }

        public Cell(Coordinate Coord, bool Value)
        {
            coord = Coord;
            value = Value;
        }

        public Cell(int X, int Y, bool Value) : this(new Coordinate(X, Y), Value)
        {
        }

        public bool Equals([AllowNull] Cell other)
        {
            return value == other.value
                && coord.Equals(other.coord);
        }

        public override string ToString() =>
             $"{(value ? '#' : '.')} ({coord})";
        
    }

    public class World : IEquatable<World>
    {
        private Dictionary<Coordinate, Cell> worldData = new Dictionary<Coordinate, Cell>();

        #region Boilerplate
        public World(string initData = "") : this(
            initData.Split('\n').Select((row, y) =>
                row.Select((cellChar, x) =>
                    new Cell(x, y, cellChar.Equals('#')
                        ))).SelectMany(s=>s))
        {
        }

        public World(World copy) : this(copy.worldData.Values)
        {
        }

        public World(IEnumerable<Cell> cells)
        {
            foreach (var c in cells)
                worldData.Add(c.coord, c);
        }

        public override string ToString()
        {
            var live = liveCells.ToList();

            int minX = 0, maxX = 0, minY = 0, maxY = 0;

            if (live.Count > 0)
            {
                minX = live.Min(c => c.coord.x) - 1;
                maxX = live.Max(c => c.coord.x) + 1;
                minY = live.Min(c => c.coord.y) - 1;
                maxY = live.Max(c => c.coord.y) + 1;
            }

            var dX = maxX - minX;
            var dY = maxY - minY;

            return $"Live Cells: {live.Count()}\n" +
                   $"({minX}, {minY}) - ({maxX}, {maxY})\n" +
                    String.Join("\n", Enumerable.Range(minY, dY + 1).Select(y_ =>
                    String.Join(" ",  Enumerable.Range(minX, dX + 1).Select(x_ =>
                        GetCell(new Coordinate(x_, y_))
                            .value == true ? "#" : ".")
                  .ToArray())));
        }
        #endregion

        private IEnumerable<Cell> liveCells => worldData.Values.Where(c => c.value);

        public World GetNext() =>
            new World(
                // Look at all live cells and their neighbors
                liveCells.Select(c => GetNeighborsAndSelf(c)).SelectMany(s => s).Distinct() // This is an enumerable of enumerables, so we must flatten the array
                    .Select(focusedCell =>
                    {
                        var neighborCount = GetLiveNeighborCount(focusedCell.coord); // NOTE: Because we don't want to query this twice, cache the value here -- otherwise this whole section could be declarative
                        return new Cell(
                            focusedCell.coord,
                            // Any live cell with two or three neighbors survives.
                            // Any dead cell with three live neighbors becomes a live cell.
                            // All other live cells die in the next generation. Similarly, all other dead cells stay dead.
                            (neighborCount == 3) || (neighborCount == 2 && focusedCell.value)
                        );
                    }));

        public World GetShifted(Coordinate delta) =>
            new World(
                liveCells.Select(c =>
                    new Cell(c.coord.Plus(delta), c.value)));

        public Cell GetCell(Coordinate coord) =>
            worldData.TryGetValue(coord, out Cell cell) ? cell :
                new Cell(coord, false);

        public int GetLiveNeighborCount(Coordinate coord) =>
            GetNeighbors(coord).Count(c => c.value == true);

        public IEnumerable<Cell> GetNeighbors(Coordinate coord) =>
            coord.Neighbors.Select(c => GetCell(c));

        public IEnumerable<Cell> GetNeighborsAndSelf(Cell cell) =>
            cell.coord.NeighborsAndSelf.Select(c => GetCell(c));

        public bool Equals([AllowNull] World other) => 
            // Ensure we have the same number of live cells,
            (liveCells.Count() == other.liveCells.Count())
                // And that for every live cell we have, it equals the value of a live cell in the other world.
                && liveCells.Select(cell => cell.value == other.GetCell(cell.coord).value).All(v => v == true);
    }
}
