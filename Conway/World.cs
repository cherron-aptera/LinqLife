using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Conway
{
    public struct Coordinate
    {
        int x;
        int y;
    }

    public struct Cell
    {
        bool value;
    }

    public class World : IEquatable<World>
    {
        private Dictionary<Coordinate, Cell> worldData = new Dictionary<Coordinate, Cell>();

        public World(string[] initData = null)
        {
            // TODO: Populate worldData with initData
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
