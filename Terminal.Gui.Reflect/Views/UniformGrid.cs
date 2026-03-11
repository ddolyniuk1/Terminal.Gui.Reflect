using Terminal.Gui.ViewBase;

namespace Terminal.Gui.Reflect.Views;

public class UniformGrid : View
{
    private int _rows;
    private int _columns;
    private int _firstColumn;

    /// <summary>
    /// Number of rows. If 0, it is automatically calculated based on column count and child count.
    /// </summary>
    public int Rows
    {
        get => _rows;
        set { _rows = value; SetNeedsLayout(); }
    }

    /// <summary>
    /// Number of columns. If 0, it is automatically calculated based on row count and child count.
    /// </summary>
    public int Columns
    {
        get => _columns;
        set { _columns = value; SetNeedsLayout(); }
    }

    /// <summary>
    /// Offset the first cell by this many empty cells (WPF FirstColumn equivalent).
    /// </summary>
    public int FirstColumn
    {
        get => _firstColumn;
        set { _firstColumn = Math.Max(0, value); SetNeedsLayout(); }
    }

    public int CellWidth { get; private set; }
    public int CellHeight { get; private set; }

    public UniformGrid() { }

    public UniformGrid(int rows, int columns)
    {
        _rows = rows;
        _columns = columns;
    }

    protected override void OnSubViewsLaidOut(LayoutEventArgs args)
    {
        base.OnSubViewsLaidOut(args);
        PerformLayout();
    }

    private void PerformLayout()
    {
        var visibleChildren = SubViews
            .Where(v => v.Visible)
            .ToList();

        if (visibleChildren.Count == 0)
            return;

        var (resolvedColumns, resolvedRows) = ResolveGrid(visibleChildren.Count);

        CellWidth = resolvedColumns > 0 ? Viewport.Width / resolvedColumns : Viewport.Width;
        
        CellHeight = _rows > 0
            ? Viewport.Height / resolvedRows
            : CellWidth;

        if (_rows == 0)
        {
            Height = resolvedRows * CellHeight;
        }

        var cellIndex = _firstColumn;

        foreach (var child in visibleChildren)
        {
            var col = cellIndex % resolvedColumns;
            var row = cellIndex / resolvedColumns;

            child.X = col * CellWidth;
            child.Y = row * CellHeight;
            child.Width  = CellWidth;
            child.Height = CellHeight;

            cellIndex++;
        }
    }
    
    /// <summary>
    /// Mirrors WPF's internal grid-resolution logic.
    /// </summary>
    private (int columns, int rows) ResolveGrid(int childCount)
    {
        int cols = _columns;
        int rows = _rows;

        // Total slots needed (children + first-column offset)
        int totalSlots = childCount + _firstColumn;

        if (cols == 0 && rows == 0)
        {
            // Square-ish grid: find smallest n where n*n >= totalSlots
            cols = (int)Math.Ceiling(Math.Sqrt(totalSlots));
            cols = Math.Max(1, cols);
            rows = (int)Math.Ceiling((double)totalSlots / cols);
            rows = Math.Max(1, rows);
        }
        else if (cols == 0)
        {
            cols = Math.Max(1, (int)Math.Ceiling((double)totalSlots / rows));
        }
        else if (rows == 0)
        {
            rows = Math.Max(1, (int)Math.Ceiling((double)totalSlots / cols));
        }

        return (cols, rows);
    }
}