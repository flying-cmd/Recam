namespace Remp.Common.Helpers;

public class FieldChange
{
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public FieldChange(string? oldValue, string? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}