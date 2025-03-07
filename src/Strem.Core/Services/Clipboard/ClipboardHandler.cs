using TextCopy;

namespace Strem.Core.Services.Clipboard;

public class ClipboardHandler : IClipboardHandler
{
    public Task CopyText(string text)
    { return ClipboardService.SetTextAsync(text); }
}