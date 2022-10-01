using System;
using System.Numerics;
using System.Reflection;

namespace Parme.Net.Editor.ViewModels.ItemPropertyFields;

public class Vector2InputField : ItemPropertyField
{
    public float X
    {
        get => ((Vector2)RawValue!).X;
        set => RawValue = new Vector2(value, ((Vector2)RawValue!).Y);
    }
    
    public float Y
    {
        get => ((Vector2)RawValue!).Y;
        set => RawValue = new Vector2(((Vector2)RawValue!).X, value);
    }
    
    public Vector2InputField(object instance, PropertyInfo property, Guid? itemId) : base(instance, property, itemId)
    {
    }
}