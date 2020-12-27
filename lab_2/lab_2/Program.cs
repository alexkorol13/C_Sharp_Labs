using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace lab_2
{
    struct DataItem
    {
        public System.Numerics.Vector2 coords { get; set; }
        public System.Numerics.Vector2 values { get; set; }
        public DataItem(System.Numerics.Vector2 coords, System.Numerics.Vector2 values)
        {
            this.coords = coords;
            this.values = values;
        }

        public override string ToString()
        {
            return coords.X.ToString() + " " + coords.Y.ToString() + " " + values.X.ToString() + " " + values.Y.ToString();
        }

        public string ToString(string format) // how to use format variable?
        {
            float cx = coords.X;
            float cy = coords.Y;
            float vx = values.X;
            float vy = values.Y;

            float abs = (float)Math.Sqrt(vx * vx + vy * vy);
            return cx.ToString(format) + " " + cy.ToString(format) + " " + vx.ToString(format) +
                " " + vy.ToString(format) + " " + abs.ToString(format);
        }

    }

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
            return $"x_steps: {x_step.ToString()}, y_steps: {y_step.ToString()}, " +
                $"x_points: {xn_points.ToString()}, y_points: {yn_points.ToString()}";
        }

        public string ToString(string format)
        {
            return $"x_steps: {x_step.ToString(format)}, y_steps: {y_step.ToString(format)}," +
                $" x_points: {xn_points.ToString(format)}, y_points: {yn_points.ToString(format)}";
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
        public abstract string ToLongString(string format);
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

    class V5DataOnGrid : V5Data, IEnumerable<DataItem>
    {
        public Grid2D grid { get; set; }
        public Vector2[,] net_values { get; set; }
        public V5DataOnGrid(string info, DateTime date, Grid2D grid) : base(info, date)
        {
            this.grid = grid;
            net_values = new Vector2[grid.xn_points, grid.yn_points];
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
                    ret_val += "\nValue: " + net_values[i, j].X + "; "+ net_values[i, j].Y;
                }
            }
            return ret_val;
        }

        public IEnumerator<DataItem> GetEnumerator()
        {
            for (int i = 0; i < grid.xn_points; i++)
            {
                for (int j = 0; j < grid.yn_points; j++)
                {
                    yield return new DataItem(new Vector2(grid.x_step * i, grid.y_step * j), 
                        new Vector2(net_values[i, j].X, net_values[i, j].Y));
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToLongString(string format)
        {
            string ret_val = this.ToString();
            for (int i = 0; i < grid.xn_points; i++)
            {
                for (int j = 0; j < grid.yn_points; j++)
                {
                    ret_val += "\n";
                    ret_val += "Point coords: " + (grid.x_step * i).ToString(format) + "; " + (grid.y_step*j).ToString(format);
                    ret_val += "\nValue: " + net_values[i, j].X.ToString(format) + "; " + net_values[i, j].Y.ToString(format);
                }
            }
            return ret_val;
        }
    }

    class V5DataCollection : V5Data, IEnumerable<DataItem>
    {
        public Dictionary<System.Numerics.Vector2, System.Numerics.Vector2> dict { get; set; }
        public V5DataCollection(string info, DateTime date) : base(info, date) { }
        public V5DataCollection (string filename) : base("", new DateTime())
        {
            // FILE FORMAT

            // info field\n
            // DateTime field\n
            // float dict.Key.X\n      # (dict item info)
            // float dict.Key.Y\n      # (dict item info)
            // float dict.Value.X\n    # (dict item info)
            // float dict.Value.Y\n    # (dict item info)

            // the last 4-string sequence repeats till the end of file, 
            // so the file may contain any number of dictipnaty items

            FileStream filestream = null;
            CultureInfo lang = new CultureInfo("ru-RU");

            lang.NumberFormat.NumberDecimalSeparator = ".";
            try
            {
                filestream = new FileStream(filename, FileMode.Open);
                StreamReader reader = new StreamReader(filestream);

                info = reader.ReadLine();
                date = Convert.ToDateTime(reader.ReadLine(), lang);

                while (!reader.EndOfStream)
                {
                    float key_x = (float)Convert.ToDouble(reader.ReadLine(), lang);
                    float key_y = (float)Convert.ToDouble(reader.ReadLine(), lang);
                    float value_x = (float)Convert.ToDouble(reader.ReadLine(), lang);
                    float value_y = (float)Convert.ToDouble(reader.ReadLine(), lang);

                    dict.Add(new Vector2(key_x, key_y), new Vector2(value_x, value_y));
                }
                
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (filestream != null) 
                {
                    filestream.Close(); 
                }
            }
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

        public IEnumerator<DataItem> GetEnumerator()
        {
            foreach (KeyValuePair<Vector2, Vector2> pair in dict)
            {
                yield return new DataItem(new Vector2(pair.Key.X, pair.Key.Y),
                    new Vector2(pair.Value.X, pair.Value.Y));
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return "V5DataCollection\n" + info + " " + date + " " +  dict.Count;
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

        public override string ToLongString(string format)
        {
            string ret_val = this.ToString();
            foreach (KeyValuePair<Vector2, Vector2> p in dict)
            {
                ret_val += "\n" + p.Key.X.ToString(format) + "; " + p.Key.Y.ToString(format)
                    + " " + p.Value.X.ToString(format) + "; " + p.Value.Y.ToString(format);
            }
            return ret_val;
        }
    }

    class V5MainCollection
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
            foreach (V5Data item in list)
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
                V5DataOnGrid dataOnGrid = new V5DataOnGrid("default data on grid" + i.ToString(), DateTime.Now, grid2d);
                dataOnGrid.InitRandom(10, 100);
                list.Add(dataOnGrid);

                V5DataCollection collection = new V5DataCollection("default data collection " + i.ToString(), DateTime.Now);
                collection.InitRandom(10, 20, 20, 0, 100);
                list.Add(collection);
            }
        }

        public override string ToString()
        {
            string str = "";
            foreach (V5Data item in list)
            {
                str += item.ToString();
            }
            return str;
        }

        public float MaxDistance(Vector2 v)
        {
            var q1 = from collection in (from data in list where data is V5DataCollection select (V5DataCollection)data)
                         from item in collection
                     select Vector2.Distance(v, item.coords);
            var q2 = from collection in (from data in list where data is V5DataOnGrid select (V5DataOnGrid)data)
                         from item in collection
                     select Vector2.Distance(v, item.coords);

            float collection_max = (q1 != null) ? q1.Max() : 0;
            float grid_max = (q2 != null) ? q2.Max() : 0;
            return (collection_max > grid_max) ? collection_max : grid_max;
        }

        public IEnumerable<DataItem> MaxDistanceItems(Vector2 v)
        {
            var q1 = from collection in (from data in list where data is V5DataCollection select (V5DataCollection)data)
                         from item in collection
                     where Vector2.Distance(v, item.coords) == MaxDistance(v) select item;

            var q2 = from collection in (from data in list where data is V5DataOnGrid select (V5DataOnGrid)data)
                                from item in collection
                     where Vector2.Distance(v, item.coords) == MaxDistance(v) select item;
            return q1.Union(q2);
        }

        public IEnumerable<DataItem> Iterator
        {
            get
            {
                var q1 = from collection in (from data in list where data is V5DataCollection select (V5DataCollection)data)
                         from item in collection
                         select item;
                var q2 = from collection in (from data in list where data is V5DataOnGrid select (V5DataOnGrid)data)
                         from item in collection
                         select item;
                return from item in q1.Union(q2) orderby item.values.Length() select item;
            }
        }
    }

   
    class Program
    {
        static void Main(string[] args)
        {
            V5DataCollection a = new V5DataCollection("test.txt");
            Console.WriteLine(a.ToLongString("f2"));

            V5MainCollection def = new V5MainCollection();
            def.AddDefaults();
            Console.WriteLine(def.ToString());

            Vector2 p;
            p.X = 3;
            p.Y = 4;

            Console.WriteLine("MaxDistance: " + def.MaxDistance(p).ToString("f4"));

            foreach (DataItem item in def.MaxDistanceItems(p))
            {
                Console.WriteLine("MaxDistanceItem: " + item.ToString("f2"));
            }

            foreach (DataItem item in def.Iterator)
            {
                Console.WriteLine("Iterator item: " + item.ToString());
            }
        }
    }
}

