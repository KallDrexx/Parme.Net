using System;
using System.Reflection;

namespace Parme.Net.Editor.ViewModels.ItemPropertyFields;

public class ByteInputField : ItemPropertyField
{
    public byte Value
    {
        get => (byte)RawValue!;
        set => RawValue = value;
    }
    
    public ByteInputField(PropertyInfo property, object instance, Guid? itemId) 
        : base(instance, property, itemId)
    {
    }
}