namespace Terminal.Gui.Reflect.Services;

/// <summary>
///     Compatibility tiers for terminal icon rendering.
/// </summary>
public enum IconTier
{
    /// <summary>
    ///     CP437 + basic Unicode (U+2500–U+25FF, U+2600–U+266F).
    ///     Works in virtually every terminal and font, including legacy
    ///     Windows Console, PuTTY, and minimal SSH environments.
    /// </summary>
    Safe,

    /// <summary>
    ///     Extended Unicode: Miscellaneous Technical (U+2300–U+23FF),
    ///     Dingbats (U+2700–U+27BF), Supplemental Arrows, and others.
    ///     Requires a modern terminal (Windows Terminal, iTerm2, GNOME Terminal)
    ///     and a font with broad coverage (Cascadia Code, Fira Code, Nerd Fonts).
    /// </summary>
    Extended
}