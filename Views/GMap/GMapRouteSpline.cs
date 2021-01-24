using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace FlightTracker.Views
{
    /// <summary>
    /// A modified GMapRoute for drawing spline routes
    /// </summary>
    public class GMapRouteSpline : GMapRoute
    {

        public GMapRouteSpline(IEnumerable<PointLatLng> points) : base(points)
        {
            Points = new List<PointLatLng>(points);
        }

        /// <summary>
        /// creates path from list of points, for performance set addBlurEffect to false
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="addBlurEffect"></param>
        /// <returns></returns>
        public override Path CreatePath(List<System.Windows.Point> localPath, bool addBlurEffect)
        {
            // Create a StreamGeometry to use to specify myPath.
            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(localPath[0], false, false);

                // Draw a Bezier curve to the next specified point.
                ctx.PolyBezierTo(localPath, true, true);
            }

            // Freeze the geometry (make it unmodifiable)
            // for additional performance benefits.
            geometry.Freeze();

            // Create a path to draw a geometry with.
            Path myPath = new Path();
            {
                // Specify the shape of the Path using the StreamGeometry.
                myPath.Data = geometry;

                if (addBlurEffect)
                {
                    BlurEffect ef = new BlurEffect();
                    {
                        ef.KernelType = KernelType.Gaussian;
                        ef.Radius = 3.0;
                        ef.RenderingBias = RenderingBias.Performance;
                    }

                    myPath.Effect = ef;
                }

                myPath.Stroke = Brushes.Navy;
                myPath.StrokeThickness = 5;
                myPath.StrokeLineJoin = PenLineJoin.Round;
                myPath.StrokeStartLineCap = PenLineCap.Triangle;
                myPath.StrokeEndLineCap = PenLineCap.Square;

                myPath.Opacity = 0.6;
                myPath.IsHitTestVisible = false;
            }
            return myPath;
        }
    }
}
