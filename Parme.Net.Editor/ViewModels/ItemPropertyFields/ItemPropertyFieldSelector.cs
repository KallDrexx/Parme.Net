using System;
using System.Collections.Generic;
using System.Reflection;

namespace Parme.Net.Editor.ViewModels.ItemPropertyFields;

public class ItemPropertyFieldSelector
{
    private readonly Dictionary<Type, Func<object, PropertyInfo, Guid?, ItemPropertyField>> _typeMapping = new()
    {
        { typeof(byte), (o, p, g) => new ByteInputField(p, o, g) },
        { typeof(int), (o, p, g) => new IntInputField(o, p, g) },
        { typeof(float), (o, p, g) => new FloatInputField(o, p, g) },
    };
    
    public ItemPropertyField GetItemProperty(object instance, PropertyInfo property, Guid? itemId)
    {
        return _typeMapping.TryGetValue(property.PropertyType, out var func) 
            ? func(instance, property, itemId) 
            : new NonEditablePropertyField(property, instance, itemId);
    }
}