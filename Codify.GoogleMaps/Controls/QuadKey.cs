using System;

namespace Codify.GoogleMaps.Controls
{
    public struct QuadKey
    {
        #region Constructors

        public QuadKey(int x, int y, int zoomLevel, Layer layer)
        {
            this = new QuadKey();
            ZoomLevel = zoomLevel;
            X = x;
            Y = y;
            Layer = layer;
        }

        public QuadKey(string quadKey)
        {
            int num;
            int num2;
            int num3;
            Layer layer;
            this = new QuadKey();
            QuadKeyToQuadPixel(quadKey, out num, out num2, out num3, out layer);
            ZoomLevel = num3;
            X = num;
            Y = num2;
            Layer = layer;
        }

        #endregion


        #region Public Properties

        public int ZoomLevel { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Layer Layer { get; set; }

        public string Key
        {
            get { return QuadPixelToQuadKey(X, Y, ZoomLevel, Layer); }
        }

        #endregion


        #region Private Methods

        private static string QuadPixelToQuadKey(int x, int y, int zoomLevel, Layer layer)
        {
            var num2 = (int) Math.Pow(2D, zoomLevel);
            var str = string.Empty;
            if ((y >= 0) && (y < num2))
            {
                while (x < 0)
                {
                    x += num2;
                }
                while (x > num2)
                {
                    x -= num2;
                }
                for (var i = 1; i <= zoomLevel; i++)
                {
                    switch (((2*(y%2)) + (x%2)))
                    {
                        case 0:
                            str = "0" + str;
                            break;

                        case 1:
                            str = "1" + str;
                            break;

                        case 2:
                            str = "2" + str;
                            break;

                        case 3:
                            str = "3" + str;
                            break;
                    }
                    x /= 2;
                    y /= 2;
                }
                str = (byte) layer + str;
                return str;
            }
            return null;
        }

        private static void QuadKeyToQuadPixel(string quadKey, out int x, out int y, out int zoomLevel, out Layer layer)
        {
            x = y = zoomLevel = 0;
            layer = default(Layer);
            if (string.IsNullOrEmpty(quadKey))
            {
                return;
            }
            try
            {
                layer = (Layer) (byte) quadKey[0];
            }
            catch {}
            zoomLevel = quadKey.Length - 1;
            for (var i = 1; i <= zoomLevel; i++)
            {
                switch (quadKey[i])
                {
                    case '0':
                        x *= 2;
                        y *= 2;
                        break;

                    case '1':
                        x = (x*2) + 1;
                        y *= 2;
                        break;

                    case '2':
                        x *= 2;
                        y = (y*2) + 1;
                        break;

                    case '3':
                        x = (x*2) + 1;
                        y = (y*2) + 1;
                        break;
                }
            }
        }

        #endregion


        #region Operators

        public static bool operator ==(QuadKey tile1, QuadKey tile2)
        {
            return (((tile1.X == tile2.X) && (tile1.Y == tile2.Y)) && (tile1.ZoomLevel == tile2.ZoomLevel) && tile1.Layer == tile2.Layer);
        }

        public static bool operator !=(QuadKey tile1, QuadKey tile2)
        {
            return !(tile1 == tile2);
        }

        #endregion


        #region Public Properties

        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is QuadKey))
            {
                return false;
            }
            var key = (QuadKey) obj;
            return (this == key);
        }

        public override int GetHashCode()
        {
            return ((X.GetHashCode() ^ Y.GetHashCode()) ^ ZoomLevel.GetHashCode() ^ Layer.GetHashCode());
        }

        #endregion
    }
}