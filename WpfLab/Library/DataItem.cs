using System;

namespace Library
{
    public struct DataItem
    {
        public System.Numerics.Vector2 Coords { get; set; }
        public System.Numerics.Vector2 Values { get; set; }
        public DataItem(System.Numerics.Vector2 coords, System.Numerics.Vector2 values)
        {
            this.Coords = coords;
            this.Values = values;
        }


        //public override string ToString()
        //{
        //    return Coords.X.ToString() + " " + Coords.Y.ToString() + " " + Values.X.ToString() + " " + Values.Y.ToString();
        //}

        //public string ToString(string format) // how to use format variable?
        //{
        //    float cx = Coords.X;
        //    float cy = Coords.Y;
        //    float vx = Values.X;
        //    float vy = Values.Y;
        //
        //    float abs = (float)Math.Sqrt(vx * vx + vy * vy);
        //    return cx.ToString(format) + " " + cy.ToString(format) + " " + vx.ToString(format) +
        //           " " + vy.ToString(format) + " " + abs.ToString(format);
        //}
    }
}