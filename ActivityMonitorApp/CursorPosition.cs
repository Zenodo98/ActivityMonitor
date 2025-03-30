using System.Drawing;
using System.Runtime.InteropServices;


namespace ActivityMonitorProgram
{
    class CursorPosition
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;
            public static explicit operator Point(PointInter point) => new Point(point.X, point.Y);
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter lpPoint);

        /// <summary>
        /// Die Methode liest die Mausposition.
        /// </summary>
        /// <returns>Mauskoordinaten X und Y</returns>
        public static Point GetCursorPosition()
        {
            PointInter lpPoint;
            GetCursorPos(out lpPoint);
            return (Point)lpPoint;
        }
    }
}
