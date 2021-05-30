using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Transactions;
using Newtonsoft.Json;

namespace Library
{
    [Serializable]
    public class V5Data
    {
        public Grid2D Grid { get; set; }
        public Vector2[,] NetValues { get; set; }
        [JsonIgnore]
        public Dictionary<Vector2, Vector2> Dict { get; set; } =
            new Dictionary<Vector2, Vector2>();
        public IEnumerable<DataItem> DataItems { get; set; } 
        public string Info { get; set; }
        public DateTime Date { get; set; }

        public V5Data()
        {
            
        }
        public V5Data(string initInfo, DateTime initDate)
        {
            this.Info = initInfo;
            this.Date = initDate;
        }

        public virtual Vector2[] NearEqual(float eps)
        {
            return null;
        }

        public override string ToString()
        {
            var str = string.Empty;
            if (Info.ToLower().Contains("grid"))
                str = "V5DataGrid";
            if (Info.ToLower().Contains("collection"))
                str = "V5DataCollection";
            
            return $"{str} {Info}\n{Date}";
        }

        public virtual string ToLongString()
        {
            return "V5Data" + Info + "\n" + Date;

        }

        public virtual string ToLongString(string format)
        {
            return null;
        }

        public static float NextFloat(Random random)
        {
            var buffer = new byte[4];
            random.NextBytes(buffer);
            var res = BitConverter.ToSingle(buffer, 0);
            return Math.Abs(res / float.MaxValue);
        }

    }
}