using Strem.Core.Variables;
using Strem.Infrastructure.Plugin;

namespace Strem.Infrastructure.Extensions;

public static class IApplicationConfigExtensions
{
    public static string GetEncryptionKey(this IApplicationConfig config) => config[InfrastructurePluginSettings.EncryptionKeyKey].ToString();
    public static string GetEncryptionIV(this IApplicationConfig config) => config[InfrastructurePluginSettings.EncryptionIVKey].ToString();
}