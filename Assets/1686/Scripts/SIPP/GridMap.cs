using System.Collections.Generic;
using UnityEngine;

namespace SIPP
{
    internal class GridUnit
    {
        public float hValue;
        public float gValue;
        public bool visited = false;
    }

    public class GridMap
    {

        internal int rows;
        internal int cols;
        internal GridUnit[][][] map;

        public GridMap(List<Interval>[][] safeIntervals)
        {
            rows = safeIntervals.Length;
            cols = safeIntervals[0].Length;
            map = new GridUnit[rows][][];
            for (int r = 0; r < rows; r++)
            {
                map[r] = new GridUnit[cols][];
                for (int c = 0; c < cols; c++)
                {
                    map[r][c] = new GridUnit[safeIntervals[r][c].Count];
                    for (int i = 0; i < map[r][c].Length; i++) map[r][c][i] = new GridUnit();
                }
            }

            GeneralClass.Log("Map Size: (" + rows + ", " + cols + ")", GeneralClass.debugLevel.low);
        }
    }
}