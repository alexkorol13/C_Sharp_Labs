using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Library
{
    [Serializable]
    public class V5DataCollection : V5Data
    {
        public V5DataCollection()
        {
            Dict = new Dictionary<Vector2, Vector2>();
            Info = "";
            DataItems = new List<DataItem>();
            Date = DateTime.Now;

        }

        public V5DataCollection(string info, DateTime date) : base(info, date) 
        {
            Dict = new Dictionary<Vector2, Vector2>();
        }
        public V5DataCollection (string filename) : base("", new DateTime())
        {

            FileStream filestream = null;
            var lang = new CultureInfo("ru-RU");
            Dict = new Dictionary<Vector2, Vector2>();
            lang.NumberFormat.NumberDecimalSeparator = ".";
            try
            {
                filestream = new FileStream(filename, FileMode.Open);
                var reader = new StreamReader(filestream);

                Info = reader.ReadLine();
                Date = Convert.ToDateTime(reader.ReadLine(), lang);

                while (!reader.EndOfStream)
                {
                    var keyX = (float)Convert.ToDouble(reader.ReadLine(), lang);
                    var keyY = (float)Convert.ToDouble(reader.ReadLine(), lang);
                    var valueX = (float)Convert.ToDouble(reader.ReadLine(), lang);
                    var valueY = (float)Convert.ToDouble(reader.ReadLine(), lang);

                    Dict.Add(new Vector2(keyX, keyY), new Vector2(valueX, valueY));
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
            var rand = new Random();
            for (var k = 0; k < nItems; k++)
            {
                var x = NextFloat(rand) * xmax;
                var y = NextFloat(rand) * ymax;
                var xVal = NextFloat(rand) * (maxValue - minValue) + minValue;
                var yVal = NextFloat(rand) * (maxValue - minValue) + minValue;
                try
                {
                    Dict.Add(new Vector2(x, y), new Vector2(xVal, yVal));
                } catch (System.ArgumentException ex)
                {
                    k--;
                }
            }
        }

        public override Vector2[] NearEqual(float eps)
        {
            Vector2[] vec = { };
            foreach (var pair in Dict)
            {
                if (Math.Abs(pair.Value.X - pair.Value.Y) < eps)
                {
                    vec.Append(pair.Value);
                }
            }
            return vec;
        }
        
        

        //public override IEnumerator<DataItem> GetEnumerator()
        //{
        //    foreach (var pair in Dict)
        //    {
        //        yield return new DataItem(new Vector2(pair.Key.X, pair.Key.Y),
        //            new Vector2(pair.Value.X, pair.Value.Y));
        //    }
        //}
        public override string ToString()
        {
            return "V5DataCollection\n" + Info + " " + Date + " " +  Dict.Count;
        }
        public override string ToLongString()
        {
            var retVal = this.ToString();
            foreach (var p in Dict)
            {
                retVal += "\n" + p.Key.X + "; " + p.Key.Y + " " + p.Value.X + "; " + p.Value.Y;
            }
            return retVal;
        }

        public override string ToLongString(string format)
        {
            var retVal = this.ToString();
            foreach (KeyValuePair<Vector2, Vector2> p in Dict)
            {
                retVal += "\n" + p.Key.X.ToString(format) + "; " + p.Key.Y.ToString(format)
                           + " " + p.Value.X.ToString(format) + "; " + p.Value.Y.ToString(format);
            }
            return retVal;
        }
    }
}