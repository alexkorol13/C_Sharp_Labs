using System;

namespace Library
{
    [Serializable]
    public struct Grid2D
    {
        public float YStep { get; set; }
        public float XStep { get; set; }
        public int XnPoints { get; set; }
        public int YnPoints { get; set; }

        public Grid2D(float xStep, float yStep, int xnPoints, int ynPoints)
        {
            XStep = xStep;
            YStep = yStep;
            XnPoints = xnPoints;
            YnPoints = ynPoints;
        }

        public override string ToString()
        {
            return $"x_steps: {XStep.ToString()}, y_steps: {YStep.ToString()}, " +
                   $"x_points: {XnPoints.ToString()}, y_points: {YnPoints.ToString()}";
        }

        public string ToString(string format)
        {
            return $"x_steps: {XStep.ToString(format)}, y_steps: {YStep.ToString(format)}," +
                   $" x_points: {XnPoints.ToString(format)}, y_points: {YnPoints.ToString(format)}";
        }
    }
}