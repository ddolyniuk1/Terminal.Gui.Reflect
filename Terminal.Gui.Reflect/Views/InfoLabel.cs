using System.ComponentModel;
using System.Drawing;

namespace Terminal.Gui.Reflect.Views;

/// <summary>
/// A focusable ℹ label that shows a tooltip popup on:
///   - Mouse hover  (auto-dismiss on mouse leave)
///   - Enter / Space key when focused (toggle)
///   - Mouse click (toggle)
///
/// Usage:
///   var info = new InfoLabel("Must be at least 8 characters.")
///   {
///       X = Pos.Right(myField) + 1,
///       Y = Pos.Top(myField)
///   };
///   window.Add(info);
/// </summary>
public sealed class InfoLabel : Label
{
    /// <summary>Text displayed inside the tooltip popup.</summary>
    public string TooltipText
    {
        get => _tooltipText;
        set
        {
            _tooltipText = value;
            if (_popup is not null)
                UpdatePopupContent();
        }
    }

    /// <summary>
    /// Maximum width (in columns) of the tooltip popup.
    /// The popup will word-wrap and grow vertically to fit.
    /// Default: 40.
    /// </summary>
    public int MaxTooltipWidth { get; set; } = 40;

    /// <summary>Glyph shown as the info icon. Default: ℹ</summary>
    public string Icon { get; set; } = PickIcon();

    private static string PickIcon()
    {
        // Windows Terminal, iTerm2, modern terminals generally support U+24D8
        // Old Windows console (conhost) often doesn't
        if (OperatingSystem.IsWindows())
        {
            return Console.OutputEncoding.CodePage == 65001  // UTF-8
                ? "\u24d8"
                : "\u2139";
        }
        return "\u24d8";
    }
    
    private string _tooltipText;
    private Window? _popup;
    private bool _pinnedOpen;
    private bool _hoverOpen;

    public InfoLabel(string tooltipText)
    {
        _tooltipText = tooltipText;

        CanFocus  = true;
        Width     = 1;
        Height    = 1;

        MouseEnter  += OnMouseEnter;
        MouseLeave  += OnMouseLeave;
        MouseClick  += OnMouseClick;

        KeyDown += OnKeyDown;

        Removed += (_, _) => DestroyPopup();
        Text = Icon;
    }

    protected override bool OnDrawingText()
    {
        var attr = HasFocus ? GetFocusColor() : GetNormalColor();
        Driver?.SetAttribute(attr);
        Move(0, 0);
        Driver?.AddStr(Icon);
        return true;
    }

    protected override bool OnDrawingContent()
    {
        return base.OnDrawingContent();
    }

    private void OnMouseEnter(object? sender, CancelEventArgs e)
    {
        if (_pinnedOpen) return;
        _hoverOpen = true;
        ShowPopup();
    }

    private void OnMouseLeave(object? sender, EventArgs? e)
    {
        _hoverOpen = false;
        if (!_pinnedOpen)
            HidePopup();
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        TogglePin();
        e.Handled = true;
    }


    private void OnKeyDown(object? sender, Key e)
    {
        if (e == Key.Enter || e == Key.Space)
        {
            TogglePin();
            e.Handled = true;
        }
        else if (e == Key.Esc && _pinnedOpen)
        {
            _pinnedOpen = false;
            HidePopup();
            e.Handled = true;
        }
    }

    private void TogglePin()
    {
        _pinnedOpen = !_pinnedOpen;
        if (_pinnedOpen)
            ShowPopup();
        else
            HidePopup();
    }

    private void ShowPopup()
    {
        if (_popup is not null) return;

        var lines = WordWrap(TooltipText, MaxTooltipWidth);
        var popupW = Math.Min(MaxTooltipWidth, lines.Max(l => l.Length)) + 4; // +4 for borders
        var popupH = lines.Count + 2;                                          // +2 for borders

        // Position: prefer right of this view, fall back to left if off-screen
        var screenRect = FrameToScreen();
        var screenW = Driver?.Cols ?? 80;
        var screenH = Driver?.Rows ?? 24;

        var popX = screenRect.X + screenRect.Width;
        var popY = screenRect.Y;

        // Clamp horizontally
        if (popX + popupW > screenW)
        {
            popX = Math.Max(0, screenRect.X - popupW);
        }

        // Clamp vertically
        if (popY + popupH > screenH)
        {
            popY = Math.Max(0, screenH - popupH);
        }

        _popup = new Window
        {
            X           = popX,
            Y           = popY,
            Width       = popupW,
            Height      = popupH,
            Title       = string.Empty,
            BorderStyle = LineStyle.Rounded,
            CanFocus    = false,
        };

        for (var i = 0; i < lines.Count; i++)
        {
            _popup.Add(new Label
            {
                Text = lines[i],
                X    = 0,
                Y    = i,
            });
        }

        if (_pinnedOpen)
        {
            _popup.Title = " ℹ ";
        }

        Application.Top?.Add(_popup);
        Application.Top?.SetNeedsDraw();
    }

    private void HidePopup()
    {
        DestroyPopup();
        Application.Top?.SetNeedsDraw();
    }

    private void DestroyPopup()
    {
        if (_popup is null) return;
        Application.Top?.Remove(_popup);
        _popup.Dispose();
        _popup = null;
    }

    private void UpdatePopupContent()
    {
        if (_popup is null) return;
        var wasPinned = _pinnedOpen;
        var wasHover  = _hoverOpen;
        DestroyPopup();
        if (wasPinned || wasHover)
            ShowPopup();
    }

    /// <summary>Simple greedy word-wrap. Returns lines of at most <paramref name="maxWidth"/> chars.</summary>
    private static List<string> WordWrap(string text, int maxWidth)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(text))
        {
            result.Add(string.Empty);
            return result;
        }

        foreach (var paragraph in text.Split('\n'))
        {
            var words = paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var line  = new System.Text.StringBuilder();

            foreach (var word in words)
            {
                if (line.Length == 0)
                {
                    line.Append(word);
                }
                else if (line.Length + 1 + word.Length <= maxWidth)
                {
                    line.Append(' ').Append(word);
                }
                else
                {
                    result.Add(line.ToString());
                    line.Clear().Append(word);
                }
            }

            if (line.Length > 0 || words.Length == 0)
                result.Add(line.ToString());
        }

        return result;
    }
    
    /// <summary>
    /// Returns screen-absolute (X, Y) of this view's top-left corner.
    /// </summary>
    public override Rectangle FrameToScreen()
    {
        var x = Frame.X;
        var y = Frame.Y;
        var parent = SuperView;
        while (parent is not null)
        {
            var offset = parent.GetViewportOffsetFromFrame();
            x += parent.Frame.X + offset.X;
            y += parent.Frame.Y + offset.Y;
            parent = parent.SuperView;
        }
        return new Rectangle(x, y, Frame.Width, Frame.Height);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            DestroyPopup();
        base.Dispose(disposing);
    }
}