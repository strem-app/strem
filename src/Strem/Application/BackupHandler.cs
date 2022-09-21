using System.IO.Compression;
using LiteDB;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Services;

namespace Strem.Application;

public class BackupHandler
{
    public void CheckAndBackupIfNeeded(List<string> preStartupLogs)
    {
        if (!File.Exists(StremPathHelper.BackupIndicatorFile)) return;
        
        preStartupLogs.Add("Located Backup Indicator File, Ensuring Database Is Up To Date");
        EnsureDatabaseIsUpToDate();
        
        preStartupLogs.Add("Backing Up Database File");
        try { BackupFiles(); }
        catch (Exception e)
        {
            preStartupLogs.Add($"Couldnt Backup Database, {e.Message}");
            return;
        }
        
        preStartupLogs.Add("Backed Up Database Files, Deleting Backup Indicator");
        try { File.Delete(StremPathHelper.BackupIndicatorFile); }
        catch (Exception e)
        { preStartupLogs.Add($"Couldnt Delete Backup Indicator, {e.Message}"); }
        preStartupLogs.Add("Backup Indicator File Deleted");
    }

    public void EnsureDatabaseIsUpToDate()
    {
        var liteDb = new LiteDatabase($"{StremPathHelper.StremDataDirectory}/default.db");
        liteDb.Checkpoint();
        liteDb.Dispose();
    }
    
    public void BackupFiles()
    {
        var backupDir = $"{StremPathHelper.StremDataDirectory}/backups";
        if (!Directory.Exists(backupDir))
        { Directory.CreateDirectory(backupDir); }
        
        var dateFormat = DateTime.Now.ToString("yyMMdd");
        var backupFile = $"{backupDir}/data-backup-{dateFormat}.zip";
        if (File.Exists(backupFile)) { return; }
        
        using var zip = ZipFile.Open(backupFile, ZipArchiveMode.Create);
        { zip.CreateEntryFromGlob(StremPathHelper.StremDataDirectory, "*.db"); }
    }
}