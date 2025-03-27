using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ActivityMonitorProgram
{
    internal static class CursorPosition
    {
        public static Point previousMouse;
        public static Point currentMouse;

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

        /// <summary>
        /// Vergleich von Mauspositionen
        /// </summary>
        /// <param name="pos1">erste position</param>
        /// <param name="pos2">zweite Position</param>
        /// <returns>Wenn die Positionen nicht gleich sind, dann ist der Rückgabewert true</returns>
        public static bool DifferentMousePosition(Point pos1, Point pos2)
        {
            if (pos1 == pos2)
            {
                return false;
            }
            return true;
        }
    }
}
