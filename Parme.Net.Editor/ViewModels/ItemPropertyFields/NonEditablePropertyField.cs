using System;
using System.Reflection;

namespace Parme.Net.Editor.ViewModels.ItemPropertyFields;

public class NonEditablePropertyField : ItemPropertyField
{
    public string? DisplayValue { get; }

    public NonEditablePropertyField(PropertyInfo property, object instance, Guid? itemId) 
        : base(instance, property, itemId)
    {
        DisplayValue = RawValue?.ToString() ?? "<null>";
    }
}