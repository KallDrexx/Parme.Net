using CommunityToolkit.Mvvm.ComponentModel;

namespace Parme.Net.Editor.ViewModels.ItemProperties;

public abstract partial class ItemProperty : ObservableObject
{
    public string PropertyName { get; }

    public ItemProperty(string propertyName)
    {
        PropertyName = propertyName;
    }
}