using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


   public struct GridPosition : IEquatable<GridPosition>
    {
        public int x;
        public int z;
        public int floor;

        public GridPosition(int x, int z, int floor)
        {
            this.x = x;
            this.z = z;
            this.floor = floor;
        }

    // will test object type then test  if values match
    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               z == position.z &&
               floor == position.floor;
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z, floor);
    }

    public override string ToString()
    {
        return "x: " + x + "; z: " + z + "; floor: " + floor;
    }

    // comparison for grid positions 
    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.z == b.z && a.floor == b.floor;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z, a.floor + b.floor);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z, a.floor - b.floor);
    }
}

