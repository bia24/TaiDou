using System;
using System.Collections.Generic;
using System.Text;

namespace TaidouCommon.Model
{
    public class SynPlayer
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public Euler Euler { get; set; }
        public bool isConnect { get; set; }
    }

    public struct Position
    {
        public double x;
        public double y;
        public double z;
        public Position(double px,double py,double pz)
        {
            x = px;
            y = py;
            z = pz;
        }
    }

    public struct Euler
    {
        public double x;
        public double y;
        public double z;
        public Euler(double ex,double ey,double ez)
        {
            x = ex;
            y = ey;
            z = ez;
        }
    }
}
