using BaseLib.Config;

namespace TheEngineer.TheEngineerCode.Data;

public class EngineerConfig : SimpleModConfig
{
    [ConfigHoverTip]
    public static bool UploadMetrics { get; set; } = false;

    [ConfigHideInUI]
    [ConfigIgnoreRestoreDefaults]
    public static bool UploadMetricsFtueSeen { get; set; } = false;
}