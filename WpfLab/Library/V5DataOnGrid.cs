using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Library
{
    [Serializable]
    public class V5DataOnGrid : V5Data
    {
        

        public V5DataOnGrid(string info, DateTime date)
        {
            Date = date;
            Info = info;
        }
        public V5DataOnGrid(string info, DateTime date, Grid2D grid) : base(info, date)
        {
            Date = DateTime.Now;
            Grid = grid;
            NetValues = new Vector2[grid.XnPoints, grid.YnPoints];
        }

        public V5DataOnGrid(string filename)
        {
            StreamReader sr = null;

            try
            {
                sr = new StreamReader(filename);
                Info = sr.ReadLine();
                Date = DateTime.Parse(sr?.ReadLine() ?? string.Empty);

                var grid = new Grid2D
                {
                    XnPoints = int.Parse(sr.ReadLine() ?? string.Empty),
                    XStep = float.Parse(sr.ReadLine() ?? string.Empty),
                    YnPoints = int.Parse(sr.ReadLine() ?? string.Empty),
                    YStep = float.Parse(sr.ReadLine() ?? string.Empty),
                };
                Grid = grid;
                NetValues = new Vector2[Grid.XnPoints, Grid.YnPoints];

                for (var i = 0; i < Grid.XnPoints; i++)
                for (var j = 0; j < Grid.YnPoints; j++)
                {
                    string[] data = sr.ReadLine()?.Split(' ');
                    NetValues[i, j] = new Vector2(
                        (float) Convert.ToDouble(data[0]),
                        (float) Convert.ToDouble(data[1]));
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Filename is empty string");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File is not found");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory is not found");
            }
            catch (IOException)
            {
                Console.WriteLine("Unacceptable filename");
            }
            catch (FormatException)
            {
                Console.WriteLine("String could not be parsed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sr != null) sr.Dispose();
            }
        }

        public void InitRandom(float minValue, float maxValue)
        {
            var rand = new Random();
            for (var i = 0; i < Grid.XnPoints; i++)
            {
                for (var j = 0; j < Grid.YnPoints; j++)
                {
                    NetValues[i, j].X = NextFloat(rand) * (maxValue - minValue) + minValue;
                    NetValues[i, j].Y = NextFloat(rand) * (maxValue - minValue) + minValue;
                }
            }
        }

        public override Vector2[] NearEqual(float eps)
        {
            Vector2[] vec = { };
            for (var i = 0; i < Grid.XnPoints; i++)
            {
                for (var j = 0; j < Grid.YnPoints; j++)
                {
                    if (Math.Abs(NetValues[i, j].X - NetValues[i, j].Y) < eps) ;
                    vec.Append(NetValues[i, j]);
                }
            }

            return vec;
        }

        public override string ToString()
        {
            return "V5DataOnGrid\n" + Info + "\n" + Date;
        }

        public override string ToLongString()
        {
            var retVal = this.ToString();
            for (var i = 0; i < Grid.XnPoints; i++)
            {
                for (var j = 0; j < Grid.YnPoints; j++)
                {
                    retVal += "\n";
                    retVal += "Point coords: " + Grid.XStep * i + "; " + Grid.YStep + j;
                    retVal += "\nValue: " + NetValues[i, j].X + "; " + NetValues[i, j].Y;
                }
            }

            return retVal;
        }


        public override string ToLongString(string format)
        {
            var retVal = this.ToString();
            for (var i = 0; i < Grid.XnPoints; i++)
            {
                for (var j = 0; j < Grid.YnPoints; j++)
                {
                    retVal += "\n";
                    retVal += "Point coords: " + (Grid.XStep * i).ToString(format) + "; " +
                              (Grid.YStep * j).ToString(format);
                    retVal += "\nValue: " + NetValues[i, j].X.ToString(format) + "; " +
                              NetValues[i, j].Y.ToString(format);
                }
            }

            return retVal;
        }
    }
}