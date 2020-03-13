using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Conway
{
    public class Coordinate : IEquatable<Coordinate>
    {
        public int x { get; }
        public int y { get; }

        #region Constructors and interfaces
        public Coordinate(int X, int Y) 
        {
            x = X;
            y = Y;
        }

        public bool Equals([AllowNull] Coordinate other) =>
                x == other.x
             && y == other.y;

        public override string ToString() =>
            $"{x},{y}";

        public override int GetHashCode() =>
            x + y;

        public Coordinate Plus(Coordinate other) =>
            new Coordinate(x + other.x, y + other.y);
        #endregion

        public IEnumerable<Coordinate> NeighborsAndSelf =>
            Enumerable.Range(x - 1, 3).Select(xNeighbor =>  // Iterate xNeighbor over x-1 to x+1
            Enumerable.Range(y - 1, 3).Select(yNeighbor =>  // Iterate yNeighbor over y-1 to y+1
                    new Coordinate(xNeighbor, yNeighbor)    // Create a coordinate for xNeighbor and yNeighbor
                )).SelectMany(s => s);                      // Flatten the arrays

        public IEnumerable<Coordinate> Neighbors =>
            NeighborsAndSelf.Where(c=>c.x != x || c.y != y);   // Exclude the center

    }

    public struct Cell : IEquatable<Cell>
    {
        public bool value { get; }
        public Coordinate coord { get; }

        #region Constructors and interfaces
        public Cell(Coordinate Coord, bool Value)
        {
            coord = Coord;
            value = Value;
        }

        public Cell(int X, int Y, bool Value) : this(new Coordinate(X, Y), Value)
        { }

        public bool Equals([AllowNull] Cell other) =>
               value == other.value
            && coord.Equals(other.coord);

        public override int GetHashCode() =>
            coord.GetHashCode();

        public override string ToString() =>
             $"{(value ? '#' : '.')} ({coord})";
        #endregion

        public Cell GetNext(int neighborCount) =>
            // Any live cell with two or three neighbors survives.
            // Any dead cell with three live neighbors becomes a live cell.
            // All other live cells die in the next generation. Similarly, all other dead cells stay dead.
            new Cell(coord,
                    (neighborCount == 3)
                 || (neighborCount == 2 && value));
    }

    public class World : IEquatable<World>
    {
        private Dictionary<Coordinate, Cell> worldData = new Dictionary<Coordinate, Cell>();
        public int Age { get; }

        #region Constructors
        public World(IEnumerable<Cell> cells, int age = 0)
        {
            worldData = cells.ToDictionary(x => x.coord, x => x);
            Age = age;
        }

        public World(World copy, int age = 0) : this(copy.worldData.Values, age)
        { }

        public World(string initData = "") : this(
            initData.Split('\n').Select((row, y) =>
                row.Select((cellChar, x) =>
                    new Cell(x, y, cellChar.Equals('#')
                        ))).SelectMany(s => s))
        { }
        #endregion


        #region World Generation

        // Returns the next generation of a world
        public World GetNext() =>
            new World( // Return a new world
                LiveCells.Select( // that focuses on every live cell,
                    c => NeighborsAndSelf(c)) // and each of its neighbors.
                        .SelectMany(s => s).Distinct() // (Flatten enumerable of enumerables)
                            .Select(focusedCell => // Then for each focused cell
                                focusedCell.GetNext( // get the next generation of that cell
                                    LiveNeighborCount(focusedCell.coord) // Based on that cell's current value and its live neighbor count
                                    )), Age+1); // That is one generation older

        // Returns the same world, but shifted by a coordinate delta
        public World GetShifted(Coordinate delta) =>
            new World(
                LiveCells.Select(c =>
                    new Cell(c.coord.Plus(delta), c.value)),
                Age);
        #endregion

        #region Cell Utilities

        public Cell CellAt(Coordinate coord) =>
            worldData.TryGetValue(coord, out Cell cell) ? cell :
                new Cell(coord, false);

        public IEnumerable<Cell> NeighborsAndSelf(Cell cell) =>
            cell.coord.NeighborsAndSelf.Select(c => CellAt(c));

        public IEnumerable<Cell> LiveCells =>
            worldData.Values.Where(c => c.value);

        public int LiveNeighborCount(Coordinate coord) =>
            coord.Neighbors.Select(c => CellAt(c)).Count(c=>c.value);

        #endregion

        #region Interfaces
        public bool Equals([AllowNull] World other) =>
            // Ensure we have the same number of live cells,
            (LiveCells.Count() == other.LiveCells.Count())
                // And that for every live cell we have, it equals the value of a live cell in the other world.
                && LiveCells.Select(cell => cell.value == other.CellAt(cell.coord).value).All(v => v == true);

        public override string ToString()
        {
            var live = LiveCells.ToList();

            int minX = 0, maxX = 0, minY = 0, maxY = 0;

            if (live.Count > 0)
            {
                minX = live.Min(c => c.coord.x) - 1;
                maxX = live.Max(c => c.coord.x) + 1;
                minY = live.Min(c => c.coord.y) - 1;
                maxY = live.Max(c => c.coord.y) + 1;
            }

            return $"Age: {Age}\n" +
                   $"Live Cells: {live.Count()}\n" +
                   $"({minX}, {minY}) - ({maxX}, {maxY})\n" +
                    String.Join("\n", Enumerable.Range(minY, maxY - minY + 1).Select(y_ =>
                    String.Join(" ", Enumerable.Range(minX, maxX - minX + 1).Select(x_ =>
                       CellAt(new Coordinate(x_, y_))
                           .value == true ? "#" : ".")
                  .ToArray())));
        }
        #endregion
    }
}
