using Strem.Core.Services.Clipboard;
using TextCopy;

namespace Strem.Platforms.Windows.Services.Clipboard;

public class ClipboardHandler : IClipboardHandler
{
    public Task CopyText(string text)
    { return ClipboardService.SetTextAsync(text); }
}