using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Strem.Core.Web;

public class BrowserLoader : IBrowserLoader
{
    public void LoadUrl(string url)
    {
        try 
        { Process.Start(url); }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                //url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
        
    }
}