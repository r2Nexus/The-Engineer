using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace TheEngineer.TheEngineerCode.Util;

public static class EngineerHoverTips
{
    public static IHoverTip GetStaticHoverTip(string locEntry)
    {
        const string locTable = "static_hover_tips";

        return new HoverTip(
            new LocString(locTable, locEntry + ".title"),
            new LocString(locTable, locEntry + ".description")
        );
    }
}