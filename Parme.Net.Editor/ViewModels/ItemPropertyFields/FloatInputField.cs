using System;
using System.Reflection;

namespace Parme.Net.Editor.ViewModels.ItemPropertyFields;

public class FloatInputField : ItemPropertyField
{
    public float Value
    {
        get => (float)RawValue!;
        set => RawValue = value;
    }
    
    public FloatInputField(object instance, PropertyInfo property, Guid? itemId) : base(instance, property, itemId)
    {
    }
}