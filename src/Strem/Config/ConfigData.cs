namespace Strem.Config;

/// <summary>
/// The idea here is if you want to compile your own variant with your own client ids etc,
/// you can just change this and build, so this is why the client ids are kept at root app
/// level not in each plugin, as the plugins in this repo are implicit plugins and always
/// included in the app, maybe in the future this will change if we had some sort of fancy
/// plugin system that could pull in external plugins easily.
/// </summary>
public class ConfigData
{
    public static readonly string TwitchClientId = "yejalwgrfnh5vcup3db5bdxkko2xh1";
    
    public static readonly string EncryptionKey = "UxRBN8hfjzTG86d6SkSSNzyUhERGu5Zj";
    public static readonly string EncryptionIV = "7cA8jkRMJGZ8iMeJ";
}