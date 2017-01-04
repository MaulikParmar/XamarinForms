using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace LayoutsDemo.Layouts
{
    public enum WrapOrientation
    {
        HorizontalThenVertical,
        VerticalThenHorizontal
    }

    public abstract class BaseLayout<T> : Layout<T> where T : View
    {
        #region Layout Info
        struct LayoutInfo
        {
            public LayoutInfo(int visibleChildCount, Size cellSize, int rows, int cols) : this()
            {
                VisibleChildCount = visibleChildCount;
                CellSize = cellSize;
                Rows = rows;
                Cols = cols;
            }

            public int VisibleChildCount
            {
                private set;
                get;
            }
            public Size CellSize
            {
                private set;
                get;
            }

            public int Rows
            {
                private set;
                get;
            }
            public int Cols
            {
                private set;
                get;
            }
        }
        #endregion

        #region Propeties

        Dictionary<Size, LayoutInfo> layoutInfoCache = new Dictionary<Size, LayoutInfo>();

        // Orientation
        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create("Orientation",
                typeof(WrapOrientation),
                typeof(BaseLayout<T>),
                default(WrapOrientation),
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((BaseLayout<T>)bindable).InvalidateLayout();
                });

        public WrapOrientation Orientation
        {
            get { return (WrapOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }



        // Column spacing
        public static readonly BindableProperty ColumnSpacingProperty =
           BindableProperty.Create("ColumnSpacing", typeof(double), typeof(BaseLayout<T>), defaultValue: 2.0,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   ((BaseLayout<T>)bindable).InvalidateLayout();
               });

        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        // Row spacing
        public static readonly BindableProperty RowSpacingProperty =
           BindableProperty.Create("RowSpacing", typeof(double), typeof(BaseLayout<T>), defaultValue: 2.0,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((BaseLayout<T>)bindable).InvalidateLayout();
                });

        public double RowSpacing
        {
            get { return (double)GetValue(RowSpacingProperty); }
            set { SetValue(RowSpacingProperty, value); }
        }

        #endregion

        #region GetLayoutInfo
        LayoutInfo GetaLyoutInfo(double width, double height)
        {
            Size size = new Size(width, height);

            // Check if cached information available
            if (layoutInfoCache.ContainsKey(size))
            {
                return layoutInfoCache[size];
            }

            int VisibleChildCount = Children.Count(x => x.IsVisible);
            int rows = 0;
            int cols = 0;
            Size maxChildSize = new Size();
            LayoutInfo layoutInfo = new LayoutInfo();

            foreach (View child in Children.Where(x => x.IsVisible))
            {
                // Get children requested size
                SizeRequest childSizeRequest = child.GetSizeRequest(Double.PositiveInfinity, Double.PositiveInfinity);

                // set max child size
                maxChildSize.Width = Math.Max(maxChildSize.Width, childSizeRequest.Request.Width); // Widht
                maxChildSize.Height = Math.Max(maxChildSize.Height, childSizeRequest.Request.Height); // Height
            }

            if (VisibleChildCount > 0)
            {
                // Calcutlate number of rows and columns
                if (this.Orientation == WrapOrientation.HorizontalThenVertical)
                {
                    if (Double.IsPositiveInfinity(width))
                    {
                        cols = VisibleChildCount;
                        rows = 1;
                    }
                    else
                    {
                        cols = (int)((width + ColumnSpacing) / (maxChildSize.Width + ColumnSpacing));
                        cols = Math.Max(1, cols);
                        rows = (VisibleChildCount + cols - 1) / cols;
                    }
                }
                else
                {
                    if (Double.IsPositiveInfinity(height))
                    {
                        rows = VisibleChildCount;
                        cols = 1;
                    }
                    else
                    {
                        rows = (int)((height + RowSpacing) / (maxChildSize.Height + RowSpacing));
                        rows = Math.Max(1, rows);
                        cols = (VisibleChildCount + rows - 1) / rows;
                    }
                }

                // Now maximize the cell size based on layout size
                Size cellSize = new Size();

                if (Orientation == WrapOrientation.HorizontalThenVertical)
                {
                    cellSize.Width = Double.IsPositiveInfinity(width) ?
                       maxChildSize.Width : (width - ColumnSpacing * (cols - 1)) / cols;

                    cellSize.Height = maxChildSize.Height;
                }
                else
                {
                    cellSize.Width = maxChildSize.Width;

                    cellSize.Height = Double.IsPositiveInfinity(height) ?
                            maxChildSize.Height : (height - RowSpacing * (rows - 1)) / rows;
                }

                layoutInfo = new LayoutInfo(VisibleChildCount, cellSize, rows, cols);

            }

            layoutInfoCache.Add(size, layoutInfo);
            return layoutInfo;
        }
        #endregion

        #region Override Methods
        protected override void InvalidateLayout()
        {
            base.InvalidateLayout();

            // Clear cache values
            layoutInfoCache.Clear();
        }

        protected override void OnChildMeasureInvalidated()
        {
            base.OnChildMeasureInvalidated();

            // Clear cache values
            layoutInfoCache.Clear();
        }

        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
        {
            LayoutInfo layoutInfo = GetaLyoutInfo(widthConstraint, heightConstraint);

            Size totalSize = new Size(layoutInfo.CellSize.Width * layoutInfo.Cols +
                            ColumnSpacing * (layoutInfo.Cols - 1),
                            layoutInfo.CellSize.Height * layoutInfo.Rows +
                            RowSpacing * (layoutInfo.Rows - 1));

            return new SizeRequest(totalSize);
        }
        #endregion

        #region Layout children
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            LayoutInfo layoutInfo = GetaLyoutInfo(width, height);

            // Initialize child position and size.
            double xChild = x;
            double yChild = y;
            int row = 0;
            int col = 0;

            foreach (View child in this.Children.Where(c => c.IsVisible))
            {
                // Get the child's requested size.
                SizeRequest childSizeRequest = child.GetSizeRequest(width, double.PositiveInfinity);

                LayoutChildIntoBoundingRegion(child,
                    new Rectangle(new Point(xChild, yChild), layoutInfo.CellSize));

                if (Orientation == WrapOrientation.HorizontalThenVertical)
                {
                    if (++col == layoutInfo.Cols)
                    {
                        col = 0;
                        row++;
                        xChild = x;
                        yChild = yChild + RowSpacing + layoutInfo.CellSize.Height;
                    }
                    else
                    {
                        xChild = xChild + ColumnSpacing + layoutInfo.CellSize.Width;
                    }
                }
                else
                {
                    if (++row == layoutInfo.Rows)
                    {
                        col++;
                        row = 0;
                        xChild += ColumnSpacing + layoutInfo.CellSize.Width;
                        yChild = y;
                    }
                    else
                    {
                        yChild += RowSpacing + layoutInfo.CellSize.Height;
                    }
                }

            }
        }
        #endregion
    }
}
