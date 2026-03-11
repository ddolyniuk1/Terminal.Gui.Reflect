using System.Drawing;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Terminal.Gui.Reflect.Views;

public class UniformGridA : View
{
    public UniformGridA(int columns)
    {
        Columns = columns;
        Width = Dim.Fill();
        Height = Dim.Fill();

        VerticalScrollBar.VisibilityMode = ScrollBarVisibilityMode.Auto;
    }
    
    public int? ForcedHeight { get; set; }

    public int Columns { get; set; }

    protected override void OnSubViewAdded(View view)
    {
        base.OnSubViewAdded(view);
        RedrawLayout();
    }

    protected override void OnSubViewRemoved(View view)
    {
        base.OnSubViewRemoved(view);
        RedrawLayout();
    }

    protected override void OnSubViewsLaidOut(LayoutEventArgs args)
    {
        base.OnSubViewsLaidOut(args);
        RedrawLayout();
    }

    protected override void OnContentSizeChanged(ValueChangedEventArgs<Size?> args)
    {
        base.OnContentSizeChanged(args);
        RedrawLayout();
    }

    private void RedrawLayout()
    {
        if (Frame.Width == 0)
        {
            return;
        }
        
        var cellWidth = Frame.Width / Columns;
        var maxChildHeight = SubViews.Max(t => t.Frame.Height); 
        
        var rowHeightCache = new Dictionary<int, int>();
        
        for (var i = 0; i < SubViews.Count; i++)
        {
            var prevChild = SubViews.ElementAtOrDefault(i - 1);
            var child = SubViews.ElementAt(i);

            var prevRow = (int)((float)(i - 1) / Columns);
            var row = (int)((float)i / Columns);
            
            child.Width = new DimAbsolute(cellWidth);
            child.Height = maxChildHeight;

            if (!rowHeightCache.ContainsKey(row) || rowHeightCache[row] < maxChildHeight)
            {
                rowHeightCache[row] = maxChildHeight;
            }
            
            if (prevChild == null)
            {
                child.X = 0;
                child.Y = 0;
            }
            else if(prevRow < row)
            {
                child.X = 0;
                child.Y = Pos.Bottom(prevChild);
            }
            else
            {
                child.X = Pos.Right(prevChild);
                child.Y = Pos.Y(prevChild);
            }
        }
        
        SetContentSize(new Size(Frame.Width, ForcedHeight ?? rowHeightCache.Values.Sum()));
    }
}