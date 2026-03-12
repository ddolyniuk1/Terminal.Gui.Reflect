using Terminal.Gui.Reflect.Interfaces;

namespace Terminal.Gui.Reflect.Services;

public class IconService : IIconService
{
    public IconService()
    {
        Tier = DetectTier();
    }

    public IconTier Tier { get; set; } = IconTier.Safe;

    // ── Arrows ───────────────────────────────────────────────────────────────
    // All cardinal arrows are CP437 — no fallback needed.
    public char ArrowLeft         => '\u2190';              // ← safe
    public char ArrowRight        => '\u2192';              // → safe
    public char ArrowUp           => '\u2191';              // ↑ safe
    public char ArrowDown         => '\u2193';              // ↓ safe
    public char ArrowLeftRight    => '\u2194';              // ↔ safe
    public char ArrowUpDown       => '\u2195';              // ↕ safe
    public char ArrowUpDownBase   => '\u21A8';              // ↨ safe
    public char ArrowLeftFilled   => '\u25C4';              // ◄ safe
    public char ArrowRightFilled  => '\u25BA';              // ► safe
    public char ArrowUpFilled     => '\u25B2';              // ▲ safe
    public char ArrowDownFilled   => '\u25BC';              // ▼ safe
    public char ArrowLeftSmall    => '\u25C2';              // ◂ safe
    public char ArrowRightSmall   => '\u25B8';              // ▸ safe
    public char ArrowUpSmall      => '\u25B4';              // ▴ safe
    public char ArrowDownSmall    => '\u25BE';              // ▾ safe
    public char ArrowLeftOutline  => '\u25C1';              // ◁ safe
    public char ArrowRightOutline => '\u25B7';              // ▷ safe
    public char ArrowUpOutline    => '\u25B3';              // △ safe
    public char ArrowDownOutline  => '\u25BD';              // ▽ safe
    public char ArrowDoubleLeft   => '\u00AB';              // « safe
    public char ArrowDoubleRight  => '\u00BB';              // » safe
    public char ArrowDoubleUp     => R('\u2191', '\u21D1'); // ↑  / ⇑
    public char ArrowDoubleDown   => R('\u2193', '\u21D3'); // ↓  / ⇓
    public char ArrowNorthEast    => R('+',      '\u2197'); // +  / ↗
    public char ArrowNorthWest    => R('+',      '\u2196'); // +  / ↖
    public char ArrowSouthEast    => R('+',      '\u2198'); // +  / ↘
    public char ArrowSouthWest    => R('+',      '\u2199'); // +  / ↙
    public char ArrowUndo         => R('\u2190', '\u21A9'); // ←  / ↩
    public char ArrowRedo         => R('\u2192', '\u21AA'); // →  / ↪
    public char ArrowRefresh      => R('~',      '\u21BA'); // ~  / ↺

    // ── Box Drawing – Single (all CP437, always safe) ─────────────────────────
    public char BoxHorizontal  => '\u2500'; // ─
    public char BoxVertical    => '\u2502'; // │
    public char BoxTopLeft     => '\u250C'; // ┌
    public char BoxTopRight    => '\u2510'; // ┐
    public char BoxBottomLeft  => '\u2514'; // └
    public char BoxBottomRight => '\u2518'; // ┘
    public char BoxCross       => '\u253C'; // ┼
    public char BoxTeeLeft     => '\u2524'; // ┤
    public char BoxTeeRight    => '\u251C'; // ├
    public char BoxTeeTop      => '\u2534'; // ┴
    public char BoxTeeBottom   => '\u252C'; // ┬

    // ── Box Drawing – Double (all CP437, always safe) ─────────────────────────
    public char BoxDoubleHorizontal  => '\u2550'; // ═
    public char BoxDoubleVertical    => '\u2551'; // ║
    public char BoxDoubleTopLeft     => '\u2554'; // ╔
    public char BoxDoubleTopRight    => '\u2557'; // ╗
    public char BoxDoubleBottomLeft  => '\u255A'; // ╚
    public char BoxDoubleBottomRight => '\u255D'; // ╝
    public char BoxDoubleCross       => '\u256C'; // ╬
    public char BoxDoubleTeeLeft     => '\u2563'; // ╣
    public char BoxDoubleTeeRight    => '\u2560'; // ╠
    public char BoxDoubleTeeTop      => '\u2569'; // ╩
    public char BoxDoubleTeeBottom   => '\u2566'; // ╦

    // ── Box Drawing – Heavy (Unicode 3.2, very widely supported) ─────────────
    public char BoxHeavyHorizontal  => R('\u2500', '\u2501'); // ─ / ━
    public char BoxHeavyVertical    => R('\u2502', '\u2503'); // │ / ┃
    public char BoxHeavyTopLeft     => R('\u250C', '\u250F'); // ┌ / ┏
    public char BoxHeavyTopRight    => R('\u2510', '\u2513'); // ┐ / ┓
    public char BoxHeavyBottomLeft  => R('\u2514', '\u2517'); // └ / ┗
    public char BoxHeavyBottomRight => R('\u2518', '\u251B'); // ┘ / ┛
    public char BoxHeavyCross       => R('\u253C', '\u254B'); // ┼ / ╋
    public char BoxHeavyTeeLeft     => R('\u2524', '\u252B'); // ┤ / ┫
    public char BoxHeavyTeeRight    => R('\u251C', '\u2523'); // ├ / ┣
    public char BoxHeavyTeeTop      => R('\u2534', '\u253B'); // ┴ / ┻
    public char BoxHeavyTeeBottom   => R('\u252C', '\u2533'); // ┬ / ┳

    // ── Box Drawing – Rounded ────────────────────────────────────────────────
    public char BoxRoundedTopLeft     => R('\u250C', '\u256D'); // ┌ / ╭
    public char BoxRoundedTopRight    => R('\u2510', '\u256E'); // ┐ / ╮
    public char BoxRoundedBottomLeft  => R('\u2514', '\u2570'); // └ / ╰
    public char BoxRoundedBottomRight => R('\u2518', '\u256F'); // ┘ / ╯

    // ── Box Drawing – Dashed ─────────────────────────────────────────────────
    public char BoxDashHorizontal2 => R('\u2500', '\u254C'); // ─ / ╌
    public char BoxDashHorizontal3 => R('\u2500', '\u2504'); // ─ / ┄
    public char BoxDashHorizontal4 => R('\u2500', '\u2508'); // ─ / ┈
    public char BoxDashVertical2   => R('\u2502', '\u254E'); // │ / ╎
    public char BoxDashVertical3   => R('\u2502', '\u2506'); // │ / ┆
    public char BoxDashVertical4   => R('\u2502', '\u250A'); // │ / ┊

    // ── Block Elements (all CP437, always safe) ───────────────────────────────
    public char BlockFull       => '\u2588'; // █
    public char BlockLight      => '\u2591'; // ░
    public char BlockMedium     => '\u2592'; // ▒
    public char BlockDark       => '\u2593'; // ▓
    public char BlockTop        => '\u2580'; // ▀
    public char BlockBottom     => '\u2584'; // ▄
    public char BlockLeft       => '\u258C'; // ▌
    public char BlockRight      => '\u2590'; // ▐
    public char BlockUpperLeft  => '\u2598'; // ▘
    public char BlockUpperRight => '\u259D'; // ▝
    public char BlockLowerLeft  => '\u2596'; // ▖
    public char BlockLowerRight => '\u2597'; // ▗
    public char BlockEighth1    => '\u2581'; // ▁
    public char BlockEighth2    => '\u2582'; // ▂
    public char BlockEighth3    => '\u2583'; // ▃
    public char BlockEighth4    => '\u2584'; // ▄
    public char BlockEighth5    => '\u2585'; // ▅
    public char BlockEighth6    => '\u2586'; // ▆
    public char BlockEighth7    => '\u2587'; // ▇
    public char BlockTopLine    => '\u2594'; // ▔
    public char BlockRightLine  => '\u2595'; // ▕

    // ── Geometric Shapes (Unicode 1.1 / CP437, always safe) ──────────────────
    public char SquareFilled         => '\u25A0'; // ■
    public char SquareOutline        => '\u25A1'; // □
    public char SquareSmallFilled    => '\u25AA'; // ▪
    public char SquareSmallOutline   => '\u25AB'; // ▫
    public char RectangleHorizontal  => '\u25AC'; // ▬
    public char RectangleVertical    => '\u25AE'; // ▮
    public char TriangleUpFilled     => '\u25B2'; // ▲
    public char TriangleUpOutline    => '\u25B3'; // △
    public char TriangleDownFilled   => '\u25BC'; // ▼
    public char TriangleDownOutline  => '\u25BD'; // ▽
    public char TriangleRightFilled  => '\u25B6'; // ▶
    public char TriangleRightOutline => '\u25B7'; // ▷
    public char TriangleLeftFilled   => '\u25C0'; // ◀
    public char TriangleLeftOutline  => '\u25C1'; // ◁
    public char TriangleUpSmall      => '\u25B4'; // ▴
    public char TriangleDownSmall    => '\u25BE'; // ▾
    public char TriangleRightSmall   => '\u25B8'; // ▸
    public char TriangleLeftSmall    => '\u25C2'; // ◂
    public char DiamondFilled        => '\u25C6'; // ◆
    public char DiamondOutline       => '\u25C7'; // ◇
    public char DiamondSmall         => '\u2666'; // ♦
    public char CircleFilled         => '\u25CF'; // ●
    public char CircleOutline        => '\u25CB'; // ○
    public char CircleBullseye       => '\u25CE'; // ◎
    public char CircleInverse        => '\u25D8'; // ◘
    public char CircleDotted         => '\u25CC'; // ◌
    public char Lozenge              => '\u25CA'; // ◊
    public char LozengeOutline       => '\u25C8'; // ◈

    // ── Symbols & Status ─────────────────────────────────────────────────────
    public char Bullet            => '\u2022';              // • safe
    public char BulletOutline     => '\u25E6';              // ◦ safe
    public char Ellipsis          => R('.', '\u2026');      // .  / …
    public char MiddleDot         => '\u00B7';              // · safe
    public char Dash              => '\u2013';              // – safe
    public char HorizontalBar     => R('\u2500', '\u2015'); // ─ / ―
    public char VerticalEllipsis  => R(':',      '\u22EE'); // :  / ⋮
    public char Star              => '\u2605';              // ★ safe
    public char StarOutline       => '\u2606';              // ☆ safe
    public char Heart             => '\u2665';              // ♥ safe
    public char HeartOutline      => R('\u2665', '\u2661'); // ♥ / ♡
    public char Diamond           => '\u2666';              // ♦ safe
    public char Club              => '\u2663';              // ♣ safe
    public char Spade             => '\u2660';              // ♠ safe
    public char Note              => '\u266A';              // ♪ safe
    public char Notes             => '\u266B';              // ♫ safe
    public char Female            => '\u2640';              // ♀ safe
    public char Male              => '\u2642';              // ♂ safe
    public char SmileyFilled      => '\u263B';              // ☻ safe
    public char SmileyOutline     => '\u263A';              // ☺ safe
    public char Sun               => '\u263C';              // ☼ safe
    public char Moon              => R('\u263C', '\u263D'); // ☼ / ☽
    public char Check             => R('\u221A', '\u2713'); // √ / ✓
    public char CheckBold         => R('\u221A', '\u2714'); // √ / ✔
    public char Cross             => R('x',      '\u2717'); // x / ✗
    public char CrossDiagonal     => '\u2573';              // ╳ safe
    public char Plus              => '+';                   // + ASCII
    public char Minus             => '\u2212';              // − safe
    public char PlusMinus         => '\u00B1';              // ± safe
    public char Multiply          => '\u00D7';              // × safe
    public char Divide            => '\u00F7';              // ÷ safe
    public char Infinity          => '\u221E';              // ∞ safe
    public char Degree            => '\u00B0';              // ° safe
    public char Paragraph         => '\u00B6';              // ¶ safe
    public char Section           => '\u00A7';              // § safe
    public char Copyright         => '\u00A9';              // © safe
    public char Registered        => '\u00AE';              // ® safe
    public char Trademark         => R('(', '\u2122');      // (  / ™
    public char Sigma             => '\u03A3';              // Σ safe
    public char Pi                => '\u03C0';              // π safe
    public char Omega             => '\u03A9';              // Ω safe
    public char Micro             => '\u00B5';              // µ safe
    public char AngleLeft         => '\u00AB';              // « safe
    public char AngleRight        => '\u00BB';              // » safe
    public char Superscript2      => '\u00B2';              // ² safe
    public char Superscript3      => '\u00B3';              // ³ safe
    public char DoubleExclamation => '\u203C';              // ‼ safe
    public char Intersection      => '\u2229';              // ∩ safe
    public char ApproxEqual       => '\u2248';              // ≈ safe
    public char LessEqual         => '\u2264';              // ≤ safe
    public char GreaterEqual      => '\u2265';              // ≥ safe
    public char NotEqual          => '\u2260';              // ≠ safe
    public char RightAngle        => '\u221F';              // ∟ safe

    // ── UI Actions ───────────────────────────────────────────────────────────
    public char Play        => '\u25B6';              // ▶ safe
    public char Pause       => R('\u2016', '\u23F8'); // ‖ / ⏸
    public char Stop        => '\u25A0';              // ■ safe
    public char Eject       => R('\u25B2', '\u23CF'); // ▲ / ⏏
    public char SkipBack    => R('\u25C4', '\u23EE'); // ◄ / ⏮
    public char SkipForward => R('\u25BA', '\u23ED'); // ► / ⏭
    public char Rewind      => R('\u25C4', '\u23EA'); // ◄ / ⏪
    public char FastForward => R('\u25BA', '\u23E9'); // ► / ⏩
    public char Add         => '+';                   // + ASCII
    public char Remove      => '\u2212';              // − safe
    public char Edit        => R('~',      '\u270E'); // ~  / ✎
    public char Delete      => R('X',      '\u2326'); // X  / ⌦
    public char Save        => R('\u2193', '\u2399'); // ↓  / ⎙
    public char Copy        => R('+',      '\u2398'); // +  / ⎘
    public char Paste       => R('\u2193', '\u2397'); // ↓  / ⎗
    public char Cut         => R('\\',     '\u2702'); // \  / ✂
    public char Expand      => R('\u2195', '\u2922'); // ↕  / ⤢
    public char Collapse    => R('\u2194', '\u2923'); // ↔  / ⤣
    public char Sort        => '\u2195';              // ↕ safe
    public char Filter      => '\u25BC';              // ▼ safe
    public char Menu        => R('=', '\u2630');      // =  / ☰
    public char Grid        => '\u25A6';              // ▦ safe
    public char List        => '\u2261';              // ≡ safe
    public char Help        => R('?', '\u2370');      // ?  / ⍰

    // ── Semantic Status ──────────────────────────────────────────────────────
    public char Error       => R('X',      '\u2716'); // X  / ✖
    public char Success     => R('\u221A', '\u2714'); // √  / ✔
    public char Warning     => R('!',      '\u26A0'); // !  / ⚠
    public char Information => R('i',      '\u2139'); // i  / ℹ
    public char Pending     => '\u25CC';              // ◌ safe
    public char Running     => '\u25B6';              // ▶ safe
    public char Spinner     => '\u25CC';              // ◌ safe

    // ── Objects ──────────────────────────────────────────────────────────────
    public char House     => '\u2302';         // ⌂ safe
    public char Telephone => R('T', '\u260E'); // T  / ☎
    public char Envelope  => R('@', '\u2709'); // @  / ✉
    public char Pencil    => R('~', '\u270F'); // ~  / ✏
    public char Scissors  => R('/', '\u2702'); // /  / ✂
    public char Hourglass => R('o', '\u231B'); // o  / ⌛
    public char Watch     => R('o', '\u231A'); // o  / ⌚
    public char Anchor    => R('+', '\u2693'); // +  / ⚓
    public char Flag      => R('|', '\u2691'); // |  / ⚑
    public char Skull     => R('X', '\u2620'); // X  / ☠
    public char Recycling => R('o', '\u267B'); // o  / ♻
    public char Power     => R('O', '\u23FB'); // O  / ⏻
    public char Gear      => R('*', '\u2699'); // *  / ⚙
    public char Lock      => R('o', '\u26BF'); // o  / ⚿
    public char Key       => R('-', '\u26BF'); // -  / ⚿
    public char File      => R('-', '\u2399'); // -  / ⎙
    public char Folder    => R('+', '\u2380'); // +  / ⎀
    public char Magnifier => R('?', '\u2315'); // ?  / ⌕
    public char Bell      => R('!', '\u237E'); // !  / ⍾
    public char BellSlash => R('x', '\u2298'); // x  / ⊘
    public char Wrench    => R('+', '\u2692'); // +  / ⚒
    public char Hammer    => R('#', '\u2692'); // #  / ⚒
    public char Pin       => R('.', '\u2316'); // .  / ⌖
    public char Tag       => R('#', '\u2318'); // #  / ⌘
    public char Bookmark  => R('|', '\u2767'); // |  / ❧
    public char Trophy    => '\u2605';         // ★ safe (star proxy)
    public char Target    => '\u25CE';         // ◎ safe
    public char Link      => R('-', '\u26AF'); // -  / ⚯
    public char Globe     => R('o', '\u25CF'); // o  / ●
    public char Cloud     => R('~', '\u2601'); // ~  / ☁
    public char Snowflake => R('*', '\u2744'); // *  / ❄
    public char Lightning => R('!', '\u26A1'); // !  / ⚡
    public char Umbrella  => R('n', '\u2602'); // n  / ☂
    public char Person    => R('i', '\u263F'); // i  / ☿
    public char Eye       => R('o', '\u25CE'); // o  / ◎
    public char Download  => '\u2193';         // ↓ safe
    public char Upload    => '\u2191';         // ↑ safe

    // ── Tree / Hierarchy (all CP437, always safe) ─────────────────────────────
    public char TreeBranch        => '\u251C'; // ├
    public char TreeNodeLast      => '\u2514'; // └
    public char TreeNodeExpanded  => '\u25BC'; // ▼
    public char TreeNodeCollapsed => '\u25BA'; // ►
    public char TreeIndent        => '\u2502'; // │

    public static IconTier DetectTier()
    {
        // Windows Terminal — full Unicode support
        if (Environment.GetEnvironmentVariable("WT_SESSION") != null)
        {
            return IconTier.Extended;
        }

        // iTerm2, VS Code terminal, Ghostty, Kitty, etc.
        var termProgram = Environment.GetEnvironmentVariable("TERM_PROGRAM") ?? "";
        if (termProgram is "iTerm.app" or "vscode" or "ghostty" or "WezTerm")
        {
            return IconTier.Extended;
        }

        // Modern Linux terminals via $TERM
        var term = Environment.GetEnvironmentVariable("TERM") ?? "";
        if (term is "xterm-256color" or "screen-256color" or "tmux-256color")
        {
            return IconTier.Extended;
        }

        // COLORTERM set to truecolor/24bit is a strong signal of a modern terminal
        var colorTerm = Environment.GetEnvironmentVariable("COLORTERM") ?? "";
        if (colorTerm is "truecolor" or "24bit")
        {
            return IconTier.Extended;
        }

        // Legacy Windows console (conhost.exe) — Safe only
        if (OperatingSystem.IsWindows())
        {
            return IconTier.Safe;
        }

        // Unknown — default safe
        return IconTier.Safe;
    }

    /// <summary>
    ///     Returns <paramref name="extended" /> when Tier is Extended,
    ///     otherwise returns <paramref name="safe" />.
    /// </summary>
    private char R(char safe, char extended)
    {
        return Tier == IconTier.Extended ? extended : safe;
    }
}