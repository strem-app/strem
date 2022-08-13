namespace Strem.Infrastructure.Extensions;

public static class TextReaderExtensions
{
    public static string[] Tail(this TextReader reader, int lineCount) {
        var buffer = new List<string>(lineCount);
        string line;
        for (var i = 0; i < lineCount; i++) {
            line = reader.ReadLine();
            if (line == null) return buffer.ToArray();
            buffer.Add(line);
        }

        var lastLine = lineCount - 1;

        while (null != (line = reader.ReadLine())) {
            lastLine++;
            if (lastLine == lineCount) lastLine = 0;
            buffer[lastLine] = line;
        }

        if (lastLine == lineCount - 1) return buffer.ToArray();
        var retVal = new string[lineCount];
        buffer.CopyTo(lastLine + 1, retVal, 0, lineCount - lastLine - 1);
        buffer.CopyTo(0, retVal, lineCount - lastLine - 1, lastLine + 1);
        return retVal;
    }
    
    public static async Task<string[]> TailAsync(this TextReader reader, int lineCount) {
        var buffer = new List<string>(lineCount);
        string line;
        for (var i = 0; i < lineCount; i++) {
            line = await reader.ReadLineAsync();
            if (line == null) return buffer.ToArray();
            buffer.Add(line);
        }

        var lastLine = lineCount - 1;

        while (null != (line = await reader.ReadLineAsync())) {
            lastLine++;
            if (lastLine == lineCount) lastLine = 0;
            buffer[lastLine] = line;
        }

        if (lastLine == lineCount - 1) return buffer.ToArray();
        var retVal = new string[lineCount];
        buffer.CopyTo(lastLine + 1, retVal, 0, lineCount - lastLine - 1);
        buffer.CopyTo(0, retVal, lineCount - lastLine - 1, lastLine + 1);
        return retVal;
    }
}