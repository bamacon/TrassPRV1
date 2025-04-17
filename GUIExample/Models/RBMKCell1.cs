using MntCuda.HardGeometry;
using MntCuda.Visualisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GUIExample.Models
{
    internal class RBMKCell1 : Cell
    {
        public InterfaceFunction Source { get; private set; }

        public RBMKCell1() : base(6)
        {
            float latticePitch = 25;
            float height = 50;

            int nTvelInnerCircle = 6;
            int nTvelOuterCircle = 12;

            float rInnerGraphiteSleeve = 4.475f;
            float rOuterGraphiteSleeve = 5.625f;
            float rGraphite = 5.7f;

            float rInnerChannelTube = 4.0f;
            float rOuterChannelTube = 4.4f;

            float rCentralTube = 0.75f;
            float rInnerCircle = 1.6f;
            float rOuterCircle = 3.1f;
            float rFuelCladding = 0.6815f;
            float rTvel = 0.5850f;

            Zone centralTube = AddZone(2);
            Zone water = AddZone(10);
            AddSurface(new CylinderOzSurface(0, 0, rCentralTube), centralTube, water);

            for (int tvel = nTvelInnerCircle - 1; tvel >= 0; tvel--)
            {
                float x = rInnerCircle * (float)Math.Cos(tvel * Math.PI / 3 - Math.PI / 3);
                float y = rInnerCircle * (float)Math.Sin(tvel * Math.PI / 3 - Math.PI / 3);

                Zone fuelCladdingTVElInnerCircle = AddZone(0);
                AddSurface(new CylinderOzSurface(x, y, rFuelCladding, false), water, fuelCladdingTVElInnerCircle);

                Zone fuelInnerCircle = AddZone(3);
                AddSurface(new CylinderOzSurface(x, y, rTvel, false), fuelCladdingTVElInnerCircle, fuelInnerCircle);
            }

            for (int tvel = nTvelOuterCircle - 1; tvel >= 0; tvel--)
            {
                float x = rOuterCircle * (float)Math.Cos(tvel * Math.PI / 6 + Math.PI / 12);
                float y = rOuterCircle * (float)Math.Sin(tvel * Math.PI / 6 + Math.PI / 12);

                Zone fuelCladdingTVELOuterCircle = AddZone(1);

                AddSurface(new CylinderOzSurface(x, y, rFuelCladding, false), water, fuelCladdingTVELOuterCircle);

                Zone fuelOuterCircle = AddZone(4);
                AddSurface(new CylinderOzSurface(x, y, rTvel, false), fuelCladdingTVELOuterCircle, fuelOuterCircle);
            }


            Zone channelTube = AddZone(9);
            AddSurface(new CylinderOzSurface(0, 0, rInnerChannelTube), water, channelTube);

            Zone gasGapChannelTubeGraphiteSleeve = AddZone(7);
            AddSurface(new CylinderOzSurface(0, 0, rOuterChannelTube), channelTube, gasGapChannelTubeGraphiteSleeve);

            Zone graphiteSleeve = AddZone(5);
            AddSurface(new CylinderOzSurface(0, 0, rInnerGraphiteSleeve), gasGapChannelTubeGraphiteSleeve, graphiteSleeve);

            Zone gasGapGraphiteSleeveGraphite = AddZone(8);
            AddSurface(new CylinderOzSurface(0, 0, rOuterGraphiteSleeve), graphiteSleeve, gasGapGraphiteSleeveGraphite);

            Zone graphite = AddZone(6);
            AddSurface(new CylinderOzSurface(0, 0, rGraphite), gasGapGraphiteSleeveGraphite, graphite);

            float d = latticePitch / 2;
            this.AddSurface(new PlaneOzSurface(d, d, d, -d), graphite, this.Interface[0]);
            this.AddSurface(new PlaneOzSurface(d, -d, -d, -d), graphite, this.Interface[1]);
            this.AddSurface(new PlaneOzSurface(-d, -d, -d, d), graphite, this.Interface[2]);
            this.AddSurface(new PlaneOzSurface(-d, d, d, d), graphite, this.Interface[3]);

            var if_trace = InterfaceFunction.TraceToPoint(graphite, -0.4999f * d, 0);
            this.AddSharedSurface(new ZConstPlaneSurface(0, -1), this.Zones, this.Interface[4], if_trace);
            this.AddSharedSurface(new ZConstPlaneSurface(height, 1), this.Zones, this.Interface[5], if_trace);

            TraceVisualizer vis = new MntCuda.Visualisation.TraceVisualizer();
            //vis.GetImage(this, 2000, 2000, TraceVisualizer.DrawRectangle.ByThreePoints(new Vector3(-d, -d, height / 2), new Vector3(d, -d, height / 2), new Vector3(-d, d, height / 2))).Save("RBMKCell.png");

            this.InternalPoint = (new Vector3(0, 0, 0), centralTube);
            //Source = Set(Register.x, 0) & Set(Register.y, 0) & Set(Register.z, height / 2) & SetZone(centralTube) & Set(Register.ng, 10) & SetRandomDirectionSphere();
        }
    }
}
