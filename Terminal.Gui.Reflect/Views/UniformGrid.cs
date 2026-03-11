using System.Drawing;
using System.Reflection;
using Terminal.Gui.ViewBase;

namespace Terminal.Gui.Reflect.Views;

/// <summary>
///     A layout container that arranges children in a grid
///     - Rows / Columns: fixed count (1..N)
///     - Use -1 to let the grid auto-compute that dimension from child count
///     - Both -1: grid grows as a single row (like a horizontal StackPanel)
///     - Height is determined by children's natural heights (per-row tallest).
/// </summary>
public class UniformGrid : View
{
    private readonly List<View> _cachedChildren = new();
    private int _columns;
    private int _rows;
    private MethodInfo? _layoutSubViewsCallback;
    
    public string Name { get; set; }

    /// <param name="rows">Number of rows. Use -1 for auto.</param>
    /// <param name="columns">Number of columns. Use -1 for auto.</param>
    public UniformGrid(int rows = -1, int columns = -1)
    {
        _rows = rows;
        _columns = columns;
        CanFocus = true;
        TabStop = TabBehavior.TabGroup;
        // ViewportSettings = ViewportSettingsFlags.AllowNegativeLocation | ViewportSettingsFlags.ClipContentOnly;
    }

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

    private (int rows, int cols, int count) ComputeGrid()
    {
        var visibleChildren = SubViews.Count(s => s.Visible);
        if (visibleChildren == 0) return (0, 0, 0);

        int cols, rows;

        if (_columns <= 0 && _rows <= 0)
        {
            cols = visibleChildren;
            rows = 1;
        }
        else if (_columns <= 0)
        {
            rows = _rows;
            cols = (int)Math.Ceiling((double)visibleChildren / rows);
        }
        else
        {
            cols = Math.Min(visibleChildren, _columns);
            rows = (int)Math.Ceiling((double)visibleChildren / cols);
        }

        return (rows, cols, visibleChildren);
    }

    protected override void OnSubViewsLaidOut(LayoutEventArgs args)
    {
        base.OnSubViewsLaidOut(args);
        ArrangeChildren();
    }
    private bool _autoHeight = true;

    public new Dim Height
    {
        get => base.Height;
        set
        {
            _autoHeight = value is DimAuto;
            base.Height = value;
        }
    }
    private void ArrangeChildren()
    {
        var totalWidth = Viewport.Width;

        _cachedChildren.Clear();
        foreach (var sub in SubViews)
            if (sub.Visible)
                _cachedChildren.Add(sub);

        var (rows, cols, count) = ComputeGrid();
        if (count == 0) return;

        var totalHSpacing = HorizontalSpacing * (cols - 1);
        var cellWidth = cols > 0 ? Math.Max(1, (totalWidth - totalHSpacing) / cols) : 1;

        // First pass: assign widths and measure each row's tallest child
        var rowHeights = new int[rows];

        for (var i = 0; i < _cachedChildren.Count; i++)
        {
            var row = i / cols;
            var child = _cachedChildren[i];

            // Give the child its cell width so it can compute its auto height
            child.Width = Dim.Absolute(cellWidth);

            // Force the child to layout so its Viewport reflects its natural height
            child.SetNeedsLayout();

            _layoutSubViewsCallback ??= child.GetType().GetMethod("LayoutSubViews", BindingFlags.Instance | BindingFlags.NonPublic);
            
            _layoutSubViewsCallback?.Invoke(child, []);

            var childHeight = child.Frame.Height;

            // If the child hasn't resolved a height yet, check Frame as fallback
            if (childHeight <= 0)
                childHeight = child.Frame.Height;

            if (childHeight > rowHeights[row])
                rowHeights[row] = childHeight;
        }

        // Second pass: position children using computed row heights
        // Build cumulative Y offsets per row
        var rowY = new int[rows];
        for (var r = 1; r < rows; r++)
            rowY[r] = rowY[r - 1] + rowHeights[r - 1] + VerticalSpacing;

        for (var i = 0; i < _cachedChildren.Count; i++)
        {
            var col = i % cols;
            var row = i / cols;

            var x = col * (cellWidth + HorizontalSpacing);
            var y = rowY[row];

            var child = _cachedChildren[i];

            child.X = Pos.Absolute(x);
            child.Y = Pos.Absolute(y);
            child.Width = Dim.Absolute(cellWidth);
            child.Height = Dim.Absolute(rowHeights[row]);
        }

        // Set the grid's own height to the total content height
        var neededHeight = rows > 0
            ? rowY[rows - 1] + rowHeights[rows - 1]
            : 0;
        
        if (_autoHeight)
        {
            base.Height = neededHeight;
        }
        else
        {
            SetContentSize(new Size(Viewport.Width, neededHeight));
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