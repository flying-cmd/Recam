namespace Remp.Common.Helpers;

public static class Checker
{
    // Compare all properties of two classes and return a dictionary to record all changed fields and their values
    public static Dictionary<string, FieldChange> CheckChanges<TOriginal, TUpdated>(TOriginal original, TUpdated updated) where TOriginal : class where TUpdated : class
    {
        var changes = new Dictionary<string, FieldChange>();

        foreach (var property in typeof(TOriginal).GetProperties())
        {
            var originalValue = property.GetValue(original);
            var updatedValue = property.GetValue(updated);

            // Check if the property value has changed
            if (originalValue != null && updatedValue != null && !Equals(originalValue, updatedValue))
            {
                changes[property.Name] = new FieldChange(originalValue.ToString(), updatedValue.ToString());
            }
        }

        return changes;
    }
}
