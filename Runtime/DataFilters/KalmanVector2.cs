using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestory.DataFilters
{
    public class KalmanVector2 : IDataFilterVector2
    {

        KalmanFilter kalmanX;
        KalmanFilter kalmanY;

        public KalmanVector2(float startingPosX, float startingPosY, float smoothing)
        {
            kalmanX = new KalmanFilter(startingPosX, smoothing);
            kalmanY = new KalmanFilter(startingPosY, smoothing);
        }

        public void Reset(float startingPosX, float startingPosY)
        {
            kalmanX.Reset(startingPosX);
            kalmanY.Reset(startingPosY);
        }

        public void Push(float startingPosX, float startingPosY)
        {
            kalmanX.Push(startingPosX);
            kalmanY.Push(startingPosY);
        }

        public PointF Get()
        {
            return new PointF(kalmanX.Get(), kalmanY.Get());
        }

        public void Reset(PointF toPoint)
        {
            Reset(toPoint.X, toPoint.Y);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})",
                kalmanX.Get().ToString("N2"), 
                kalmanY.Get().ToString("N2"));
        }
    }
}
