using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HotelSchedulerControl.ViewPort
{
    class ControlViewport : IViewport
    {
        /// <summary>
        /// Construct a Viewport
        /// </summary>
        /// <param name="view"></param>
        public ControlViewport(Control view)
        {
            _mDevice = view;
            _mhScroll = new HScrollBar();
            _mvScroll = new VScrollBar();
            _mScrollHolePatch = new UserControl();
            WorldWidth = view.Width;
            WorldHeight = view.Height;

            _mDevice.Controls.Add(_mhScroll);
            _mDevice.Controls.Add(_mvScroll);
            _mDevice.Controls.Add(_mScrollHolePatch);

            _mhScroll.Scroll += (s, e) => X = e.NewValue;
            _mvScroll.Scroll += (s, e) => Y = e.NewValue;
            _mDevice.Resize += (s, e) => this.Resize();
            _mDevice.MouseWheel += (s, e) => Y -= e.Delta > 0 ? WheelDelta : -WheelDelta;
            WheelDelta = _mvScroll.LargeChange;

            _RecalculateMatrix();
            _RecalculateRectangle();
        }

        /// <summary>
        /// Identity Matrix
        /// </summary>
        public static readonly Matrix Identity = new Matrix(1, 0, 0, 1, 0, 0);

        /// <summary>
        /// Get or set the number of pixels to scroll on each click of the mouse
        /// </summary>
        public int WheelDelta { get; set; }

        /// <summary>
        /// Get the Rectangle area in world coordinates where the Viewport is currently viewing over
        /// </summary>
        public RectangleF Rectangle
        {
            get
            {
                return _mRectangle;
            }
        }

        /// <summary>
        /// Get the projection transformation matrix required for drawing models in the world projected into view
        /// </summary>
        public Matrix Projection
        {
            get
            {
                return _mMatrix;
            }
        }

        /// <summary>
        /// Resize the Viewport according to the view control and world dimensions, which ever larger and add scrollbars where approperiate
        /// </summary>
        public void Resize()
        {
            _mhScroll.Dock = DockStyle.None;
            _mhScroll.Location = new Point(0, _mDevice.Height - _mhScroll.Height);
            _mhScroll.Width = _mDevice.Width - _mvScroll.Width;

            _mvScroll.Dock = DockStyle.None;
            _mvScroll.Location = new Point(_mDevice.Width - _mvScroll.Width, 0);
            _mvScroll.Height = _mDevice.Height - _mhScroll.Height;

            _mScrollHolePatch.Location = new Point(_mhScroll.Right, _mvScroll.Bottom);
            _mScrollHolePatch.Size = new Size(_mvScroll.Width, _mhScroll.Height);

            if (WorldWidth <= _mDevice.Width)
            {
                _mhScroll.Hide();
            }
            else
            {
                _mhScroll.Maximum = (int)(WorldWidth - _mDevice.Width);
                _mhScroll.Show();

            }

            if (WorldHeight <= _mDevice.Height)
            {
                _mvScroll.Hide();
            }
            else
            {
                _mvScroll.Maximum = (int)(WorldHeight - _mDevice.Height);
                _mvScroll.Show();
            }

            _RecalculateRectangle();
            _RecalculateMatrix();

            _mDevice.Invalidate();
        }
        /// <summary>
        /// Convert view coordinates to world coordinates
        /// </summary>
        /// <param name="screencoord"></param>
        /// <returns></returns>
        public PointF DeviceToWorldCoord(Point screencoord)
        {
            return new PointF(screencoord.X + X, screencoord.Y + Y);
        }

        /// <summary>
        /// Convert view coordinates to world coordinates
        /// </summary>
        /// <param name="screencoord"></param>
        /// <returns></returns>
        public PointF DeviceToWorldCoord(PointF screencoord)
        {
            return new PointF(screencoord.X + X, screencoord.Y + Y);
        }

        /// <summary>
        /// Convert world coordinates to view coordinates
        /// </summary>
        /// <param name="worldcoord"></param>
        /// <returns></returns>
        public PointF WorldToDeviceCoord(PointF worldcoord)
        {
            return new PointF(worldcoord.X - X, worldcoord.Y - Y);
        }

        /// <summary>
        /// Get or set the world width
        /// </summary>
        public float WorldWidth
        {
            get { return _mWorldWidth; }
            set
            {
                if (!value.Equals(_mWorldWidth))
                {
                    if (value < _mDevice.Width) value = _mDevice.Width;
                    _mWorldWidth = value;
                    Resize();
                }
            }
        }

        /// <summary>
        /// Get or set the world height
        /// </summary>
        public float WorldHeight
        {
            get { return _mWorldHeight; }
            set
            {
                if (!value.Equals(_mWorldHeight))
                {
                    if (value < _mDevice.Height) value = _mDevice.Height;
                    _mWorldHeight = value;
                    Resize();
                }
            }
        }

        /// <summary>
        /// Get or set the world X coordinate of the Viewport location, represented by the top left corner of the Viewport Rectangle
        /// </summary>
        public float X
        {
            get { return _mhScroll.Value; }
            set
            {
                if (!((int)value).Equals(_mhScroll.Value))
                {
                    if (value > _mhScroll.Maximum) value = _mhScroll.Maximum;
                    else if (value < 0) value = 0;
                    _mhScroll.Value = (int)value;
                    _RecalculateRectangle();
                    _RecalculateMatrix();
                    _mDevice.Invalidate();
                }
            }
        }

        /// <summary>
        /// Get or set the wordl Y coordinate of the Viewport location, represented by the top left corner of the Viewport Rectangle
        /// </summary>
        public float Y
        {
            get { return _mvScroll.Value; }
            set
            {
                if (!((int)value).Equals(_mvScroll.Value))
                {
                    if (value > _mvScroll.Maximum) value = _mvScroll.Maximum;
                    else if (value < 0) value = 0;
                    _mvScroll.Value = (int)value;
                    _RecalculateRectangle();
                    _RecalculateMatrix();
                    _mDevice.Invalidate();
                }
            }
        }

        private void _RecalculateRectangle()
        {
            _mRectangle = new RectangleF(X, Y, _mDevice.Width, _mDevice.Height);
        }

        private void _RecalculateMatrix()
        {
            _mMatrix = new Matrix();
            _mMatrix.Translate(-X, -Y);
        }

        Control _mDevice;
        HScrollBar _mhScroll;
        VScrollBar _mvScroll;
        UserControl _mScrollHolePatch;
        RectangleF _mRectangle = RectangleF.Empty;
        Matrix _mMatrix = new Matrix();
        float _mWorldHeight, _mWorldWidth;
    }
}
