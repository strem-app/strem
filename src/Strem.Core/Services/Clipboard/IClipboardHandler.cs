namespace Strem.Core.Services.Clipboard;

public interface IClipboardHandler
{
    Task CopyText(string text);
}