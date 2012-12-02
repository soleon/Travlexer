using System;
using System.Collections.Generic;
using System.Device.Location;

namespace Travlexer.WindowsPhone.Controls
{
    internal static class Utility
    {
        /// <summary>
        ///     Uses the Douglas Peucker algorithim to reduce the number of points.
        /// </summary>
        internal static T DouglasPeuckerReduction<T>(T points, Double tolerance) where T : class, IList<GeoCoordinate>, new()
        {
            if (points == null || points.Count < 3)
                return points;

            const int firstPoint = 0;
            var lastPoint = points.Count - 1;
            var pointIndexsToKeep = new List<int> {firstPoint, lastPoint};

            //Add the first and last index to the keepers

            //The first and the last point can not be the same
            while (points[firstPoint].Equals(points[lastPoint]))
                lastPoint--;

            DouglasPeuckerReduction(points, firstPoint, lastPoint, tolerance, ref pointIndexsToKeep);

            var returnPoints = new T();
            pointIndexsToKeep.Sort();
            foreach (var index in pointIndexsToKeep)
                returnPoints.Add(points[index]);

            return returnPoints;
        }

        private static void DouglasPeuckerReduction(IList<GeoCoordinate> points, int firstPoint, int lastPoint, Double tolerance, ref List<int> pointIndexsToKeep)
        {
            Double maxDistance = 0;
            var indexFarthest = 0;

            for (var index = firstPoint; index < lastPoint; index++)
            {
                var distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);
                if (!(distance > maxDistance)) continue;
                maxDistance = distance;
                indexFarthest = index;
            }

            if (!(maxDistance > tolerance) || indexFarthest == 0) return;
            //Add the largest point that exceeds the tolerance
            pointIndexsToKeep.Add(indexFarthest);

            DouglasPeuckerReduction(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
            DouglasPeuckerReduction(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
        }

        /// <summary>
        ///     The distance of a point from a line made from point1 and point2.
        /// </summary>
        private static Double PerpendicularDistance(GeoCoordinate point1, GeoCoordinate point2, GeoCoordinate point)
        {
            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = √((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base

            var area = Math.Abs(.5*(point1.Latitude*point2.Longitude + point2.Latitude*point.Longitude + point.Latitude*point1.Longitude - point2.Latitude*point1.Longitude - point.Latitude*point2.Longitude - point1.Latitude*point.Longitude));
            var bottom = Math.Sqrt(Math.Pow(point1.Latitude - point2.Latitude, 2) + Math.Pow(point1.Longitude - point2.Longitude, 2));
            var height = area/bottom*2;

            return height;
        }
    }
}