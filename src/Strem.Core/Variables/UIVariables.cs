namespace Strem.Core.Variables;

public class UIVariables
{
    public static readonly VariableEntry ShowHelpersVariable = new VariableEntry("show-helpers", "ui");
    public static readonly VariableEntry BackupFrequencyInDays = new VariableEntry("backup.frequency", "ui");
    public static readonly VariableEntry LastBackupDate = new VariableEntry("backup.last-run", "ui");
}