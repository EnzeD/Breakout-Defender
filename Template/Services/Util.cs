﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainSpace
{
    public class Util
    {
        static Random RandomGen = new Random();

        public static void SetRandomSeed(int seed)
        {
            RandomGen = new Random(seed);
        }

        public static int GetInt(int pMin, int pMax)
        {
            return RandomGen.Next(pMin, pMax + 1);
        }

        public static bool CollideByBox(IActor p1, IActor p2)
        {
            return p1.BoundingBox.Intersects(p2.BoundingBox);
        }
        public static bool CollideBottomWithTop(IActor p1, IActor p2)
        {
            if (p1.BoundingBox.Intersects(p2.BoundingBox))
            {
                int bottomOfP1 = p1.BoundingBox.Bottom;
                int topOfP2 = p2.BoundingBox.Top;

                if (bottomOfP1 >= topOfP2 && p1.BoundingBox.Top < topOfP2)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
