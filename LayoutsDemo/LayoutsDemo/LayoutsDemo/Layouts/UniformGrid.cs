using System;
using System.Linq;
using Xamarin.Forms;

namespace LayoutsDemo.Layouts
{
    public class UniformGrid : BaseLayout<View>
    {
        #region Chidren layout
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            //base.LayoutChildren(x, y, width, height);

            LayoutInfo layoutInfo = GetaLyoutInfo(width, height);

            // Initialize child position and size.
            double xChild = x;
            double yChild = y;
            int row = 0;
            int col = 0;

            // Get max size from height and width
            double maxSize = Math.Max(layoutInfo.CellSize.Height, layoutInfo.CellSize.Width);

            foreach (View child in this.Children.Where(c => c.IsVisible))
            {
                LayoutChildIntoBoundingRegion(child,
                    new Rectangle(new Point(xChild, yChild), new Size(maxSize, maxSize)));

                if (Orientation == WrapOrientation.HorizontalThenVertical)
                {
                    if (++col == layoutInfo.Cols)
                    {
                        col = 0;
                        row++;
                        xChild = x;
                        yChild = yChild + RowSpacing + maxSize;
                    }
                    else
                    {
                        xChild = xChild + ColumnSpacing + maxSize;
                    }
                }
                else
                {
                    if (++row == layoutInfo.Rows)
                    {
                        col++;
                        row = 0;
                        xChild += ColumnSpacing + maxSize;
                        yChild = y;
                    }
                    else
                    {
                        yChild += RowSpacing + maxSize;
                    }
                }

            }
        }
        #endregion

        #region Size
        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
        {
            // Get layout info
            LayoutInfo layoutInfo = GetaLyoutInfo(widthConstraint, heightConstraint);

            // Get MaxSize
            double maxSize = Math.Max(layoutInfo.CellSize.Width, layoutInfo.CellSize.Height);

            // Total size
            Size totalSize = new Size(maxSize * layoutInfo.Cols +
                            ColumnSpacing * (layoutInfo.Cols - 1),
                            maxSize * layoutInfo.Rows +
                            RowSpacing * (layoutInfo.Rows - 1));

            return new SizeRequest(totalSize);
        }
        #endregion
    }
}
