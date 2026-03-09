namespace Terminal.Gui.Reflect
{
    namespace TerminalGuiComponents
    {
        /// <summary>
        ///     A layout container that arranges children in a uniform grid,
        ///     similar to WPF's UniformGrid.
        ///     - Rows / Columns: fixed count (1..N)
        ///     - Use -1 to let the grid auto-compute that dimension from child count
        ///     - Both -1: grid grows as a single row (like a horizontal StackPanel)
        /// </summary>
        public class UniformGrid : View
        {
            private readonly List<View> _cachedChildren = new();
            private          int        _columns;

            private int _lastChildCount = -1;
            private int _lastWidth      = -1, _lastHeight = -1;
            private int _rows;

            /// <param name="rows">Number of rows. Use -1 for auto.</param>
            /// <param name="columns">Number of columns. Use -1 for auto.</param>
            public UniformGrid(int rows = -1, int columns = -1)
            {
                _rows    = rows;
                _columns = columns;
                CanFocus = true;
                TabStop  = TabBehavior.TabGroup;
            }

            // -1 means "auto"
            public int Rows
            {
                get => _rows;
                set
                {
                    _rows = value;
                    SetNeedsLayout();
                }
            }

            public int Columns
            {
                get => _columns;
                set
                {
                    _columns = value;
                    SetNeedsLayout();
                }
            }

            /// <summary>Gap (in characters) between columns.</summary>
            public int HorizontalSpacing { get; set; } = 0;

            /// <summary>Gap (in rows) between rows.</summary>
            public int VerticalSpacing { get; set; } = 0;

            protected override void OnSubviewsLaidOut(LayoutEventArgs args)
            {
                base.OnSubviewsLaidOut(args);
                ArrangeChildren();
            }

            private void ArrangeChildren()
            {
                var totalWidth  = Viewport.Width;
                var totalHeight = Viewport.Height;

                var subviewCount = Subviews.Count;
                if (subviewCount != _lastChildCount)
                {
                    _cachedChildren.Clear();
                    foreach (var sub in Subviews)
                    {
                        if (sub.Visible)
                        {
                            _cachedChildren.Add(sub);
                        }
                    }

                    _lastChildCount = subviewCount;
                }

                var count = _cachedChildren.Count;
                if (count == 0)
                {
                    return;
                }

                if (totalWidth == _lastWidth && totalHeight == _lastHeight)
                {
                    return;
                }

                _lastWidth  = totalWidth;
                _lastHeight = totalHeight;

                var cols = _columns;
                var rows = _rows;

                if (cols <= 0 && rows <= 0)
                {
                    cols = count;
                    rows = 1;
                }
                else if (cols <= 0)
                {
                    cols = (int)Math.Ceiling((double)count / rows);
                }
                else if (rows <= 0)
                {
                    rows = (int)Math.Ceiling((double)count / cols);
                }

                var totalHSpacing = HorizontalSpacing * (cols - 1);
                var totalVSpacing = VerticalSpacing   * (rows - 1);

                var cellWidth  = cols > 0 ? Math.Max(1, (totalWidth  - totalHSpacing) / cols) : 1;
                var cellHeight = rows > 0 ? Math.Max(1, (totalHeight - totalVSpacing) / rows) : 1;

                for (var i = 0; i < _cachedChildren.Count; i++)
                {
                    var col = i % cols;
                    var row = i / cols;

                    var x = col * (cellWidth  + HorizontalSpacing);
                    var y = row * (cellHeight + VerticalSpacing);

                    var child = _cachedChildren[i];

                    var newX = Pos.Absolute(x);
                    var newY = Pos.Absolute(y);
                    var newW = Dim.Absolute(cellWidth);

                    if (child.X != newX)
                    {
                        child.X = newX;
                    }

                    if (child.Y != newY)
                    {
                        child.Y = newY;
                    }

                    if (child.Width != newW)
                    {
                        child.Width = newW;
                    }
                }
            }

            /// <summary>
            ///     Convenience helper — add a view and queue a re-layout.
            /// </summary>
            public void AddCell(View view)
            {
                Add(view);
                SetNeedsLayout();
            }
        }
    }
}