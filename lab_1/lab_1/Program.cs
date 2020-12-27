using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace lab_1
{
    struct Grid2D
    {
        public float y_step { get; set; }
        public float x_step { get; set; }
        public int xn_points { get; set; }
        public int yn_points { get; set; }

        public Grid2D(float x_step, float y_step, int xn_points, int yn_points)
        {
            this.x_step = x_step;
            this.y_step = y_step;
            this.xn_points = xn_points;
            this.yn_points = yn_points;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    abstract class V5Data
    {
        public string info { get; set; }
        public DateTime date { get; set; }

        public V5Data(string init_info, DateTime init_date)
        {
            this.info = init_info;
            this.date = init_date;
        }

        abstract public Vector2[] NearEqual(float eps);
        abstract public string ToLongString();
        public override string ToString()
        {
            return base.ToString();
        }

        public static float NextFloat(Random random)
        {
            var buffer = new byte[4];
            random.NextBytes(buffer);
            float res = BitConverter.ToSingle(buffer, 0);
            return Math.Abs(res / float.MaxValue);
        }
    }

    class V5DataOnGrid : V5Data
    {
        public Grid2D grid { get; set; }
        public Vector2[,] net_values { get; set; }
        public V5DataOnGrid(string info, DateTime date, Grid2D grid) : base(info, date)
        {
            this.grid = grid;
            net_values = new Vector2[grid.xn_points, grid.yn_points]; // ok вроде
        }
        public void InitRandom(float minValue, float maxValue)
        {
            Random rand = new Random();
            for (int i = 0; i < grid.xn_points; i++)
            {
                for (int j = 0; j < grid.yn_points; j++)
                {
                    net_values[i, j].X = NextFloat(rand) * (maxValue - minValue) + minValue;
                    net_values[i, j].Y = NextFloat(rand) * (maxValue - minValue) + minValue;
                }
            }
        }

        override public Vector2[] NearEqual(float eps)
        {
            Vector2[] vec = { };
            for (int i = 0; i < grid.xn_points; i++)
            {
                for (int j = 0; j < grid.yn_points; j++)
                {
                    if (Math.Abs(net_values[i, j].X - net_values[i, j].Y) < eps);
                        vec.Append(net_values[i, j]);
                }
            }
            return vec;
        }
        
        public override string ToString()
        {
            return "V5DataOnGrid\n" + info + "\n" + date;
        }

        public override string ToLongString()
        {
            string ret_val = this.ToString();
            for (int i = 0; i < grid.xn_points; i++)
            {
                for (int j = 0; j < grid.yn_points; j++)
                {
                    ret_val += "\n";
                    ret_val += "Point coords: " + grid.x_step * i + "; " + grid.y_step + j;
                    ret_val += "\nValue: " + net_values[i, j];
                }
            }
            return ret_val;
        }
    }

    class V5DataCollection : V5Data
    {
        public Dictionary<System.Numerics.Vector2, System.Numerics.Vector2> dict { get; set; }
        public V5DataCollection(string info, DateTime date) : base(info, date) {}
        public static explicit operator V5DataCollection(V5DataOnGrid dg)
        {
            V5DataCollection collection = new V5DataCollection("from grid2d", DateTime.Now);
            for (int i = 0; i < dg.grid.xn_points; i++)
            {
                for (int j = 0; j < dg.grid.yn_points; j++)
                {
                    collection.dict.Add(new Vector2(dg.grid.x_step*i, dg.grid.y_step * j), 
                        new Vector2(dg.net_values[i,j].X, dg.net_values[i, j].Y));
                }
            }
            return collection;
        }
        public void InitRandom(int nItems, float xmax, float ymax, float minValue, float maxValue)
        {
            Random rand = new Random();
            for (int k = 0; k < nItems; k++)
            {
                float x = NextFloat(rand) * xmax;
                float y = NextFloat(rand) * ymax;
                float x_val = NextFloat(rand) * (maxValue - minValue) + minValue;
                float y_val = NextFloat(rand) * (maxValue - minValue) + minValue;
                dict.Add(new Vector2(x, y), new Vector2(x_val, y_val));
            }
        }

        override public Vector2[] NearEqual(float eps)
        {
            Vector2[] vec = { };
            foreach (KeyValuePair<System.Numerics.Vector2, System.Numerics.Vector2> pair in dict)
            {
                if (Math.Abs(pair.Value.X - pair.Value.Y) < eps)
                {
                    vec.Append(pair.Value);
                }
            }
            return vec;
        }

        public override string ToString()
        {
            return "V5DataCollection\n" + info + " " + date + dict.Count;
        }
        public override string ToLongString()
        {
            string ret_val = this.ToString();
            foreach (KeyValuePair<Vector2, Vector2> p in dict)
            {
                ret_val += "\n" + p.Key.X + "; " + p.Key.Y + " " + p.Value.X + "; " + p.Value.Y;
            }
            return ret_val;
        }
    }

    class V5MainCollection : IEnumerable<V5Data>
    {
        private List<V5Data> list;
        public int Count()
        {
            return list.Count;
        }

        public void Add(V5Data item)
        {
            list.Add(item);
        }

        bool Remove(string id, DateTime date)
        {
            bool ret_val = false;
            foreach(V5Data item in list)
            {
                if (item.date == date && item.info == id) { list.Remove(item); ret_val = true; }
            }
            return ret_val;
        }
        public void AddDefaults()
        {
            for (int i = 0; i < 3; i++)
            {
                Grid2D grid2d = new Grid2D(0.1f, 0.2f, 5, 10);
                V5DataOnGrid dataOnGrid = new V5DataOnGrid("default data on grid" + i.ToString(), new DateTime(), grid2d);
                dataOnGrid.InitRandom(10, 100);
                list.Add(dataOnGrid);

                V5DataCollection collection = new V5DataCollection("default data collection " + i.ToString(), new DateTime());
                collection.InitRandom(10, 20, 20, 0, 100);
                list.Add(collection);
            }
        }

        public override string ToString()
        {
            string str = "";
            foreach(V5Data item in list)
            {
                str += item.ToString();
            }
            return str;
        }

        public IEnumerator<V5Data> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Grid2D point = new Grid2D(10, 10, 25, 25);
            V5DataOnGrid dg = new V5DataOnGrid("point 1 info", DateTime.Now, point);
            dg.InitRandom(0, 100);
            Console.WriteLine(dg.ToLongString());
            V5DataCollection dc = (V5DataCollection)dg;
            Console.WriteLine(dc.ToLongString());

            V5MainCollection collection = new V5MainCollection();
            collection.AddDefaults();
            Console.WriteLine(collection.ToString());

            Vector2[] lst;
            foreach (V5Data item in collection)
            {
                lst = item.NearEqual(10.0f);
                for (int i = 0; i < lst.Length; i++)
                {
                    Console.WriteLine(lst[i].X + "; " + lst[i].Y);
                }
            }
            Console.ReadKey();
        }
    }
}
