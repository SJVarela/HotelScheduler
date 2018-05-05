using System.Drawing;
using System.Drawing.Drawing2D;

namespace HotelSchedulerControl.ViewPort
{
    /// <summary>
    /// IViewport moves in world coordinate and projects models to device coordinate space
    /// </summary>
    public interface IViewport
    {
        /// <summary>
        /// Get the projection matrix to transform world coordinates to device coordinates
        /// </summary>
        Matrix Projection { get; }
        /// <summary>
        /// Get the rectangle boundary in world coordinates to be captured and projected onto viewport
        /// </summary>
        RectangleF Rectangle { get; }
        /// <summary>
        /// Request viewport to recalculate its get properties.
        /// </summary>
        void Resize();
        /// <summary>
        /// Convert device coordinates to world coordinates
        /// </summary>
        /// <param name="screencoord"></param>
        /// <returns></returns>
        PointF DeviceToWorldCoord(Point screencoord);
        /// <summary>
        /// Convert device coordinates to world coordinates
        /// </summary>
        /// <param name="screencoord"></param>
        /// <returns></returns>
        PointF DeviceToWorldCoord(PointF screencoord);
        /// <summary>
        /// Convert world coordinates to device coordinates
        /// </summary>
        /// <param name="worldcoord"></param>
        /// <returns></returns>
        PointF WorldToDeviceCoord(PointF worldcoord);
        /// <summary>
        /// Get or set the world height
        /// </summary>
        float WorldHeight { get; set; }
        /// <summary>
        /// Get or set the world width
        /// </summary>
        float WorldWidth { get; set; }
        /// <summary>
        /// Get or set the X world-coordinate of the position of the viewport.
        /// This is also the same as the IViewport.Rectangle.Left.
        /// </summary>
        float X { get; set; }
        /// <summary>
        /// Get or set the Y world-coordinate of the position of the viewport.
        /// This is also the same as the IViewport.Rectangle.Top.
        /// </summary>
        float Y { get; set; }
    }
}
