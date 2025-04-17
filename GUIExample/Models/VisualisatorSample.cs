using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using MntCuda;
using MntCuda.HardGeometry;
using MntCuda.Visualisation;
using SixLabors.ImageSharp;


namespace GUIExample.Models
{
    internal class VisualisatorSample
    {



        public void WriteToFile(int picsize = 500)
        {
            RBMKCell1 cell = new RBMKCell1();

            TraceVisualizer vis = new TraceVisualizer(TraceVisualizer.AlgorithmEnum.CheckContains);
            var image = vis.GetImage(cell, picsize, picsize,
                TraceVisualizer.DrawRectangle.ByThreePoints(new Vector3(-12.5f, -12.5f, 0),
                    new Vector3(12.5f, -12.5f, 0), new Vector3(-12.5f, 12.5f, 0)), null
                );
            /*vis.DrawCell(cell, 500, 500,
                TraceVisualizer.DrawRectangle.ByThreePoints(new Vector3(-12.5f, -12.5f, 0),
                    new Vector3(12.5f, -12.5f, 0), new Vector3(-12.5f, 12.5f, 0)), "res.png");*/

            var a = 0;

        }
    }
}
