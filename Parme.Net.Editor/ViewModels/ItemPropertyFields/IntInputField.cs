using System;
using System.Reflection;

namespace Parme.Net.Editor.ViewModels.ItemPropertyFields;

public class IntInputField : ItemPropertyField
{
    public int Value
    {
        get => (int)RawValue!;
        set => RawValue = value;
    }
    
    public IntInputField(object instance, PropertyInfo property, Guid? itemId) : base(instance, property, itemId)
    {
    }
}