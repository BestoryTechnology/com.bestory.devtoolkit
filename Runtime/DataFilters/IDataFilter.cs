using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestory.DataFilters
{
    public interface IDataFilterVector2
    {

        void Reset(float startingPosX, float startingPosY);
        void Reset(PointF toPoint);
        void Push(float startingPosX, float startingPosY);
        PointF Get();

        string ToString();

    }
}
