namespace Parme.Net.Editor.ViewModels.ItemProperties;

public partial class NonEditableProperty : ItemProperty
{
    public string? DisplayValue { get; }
    
    public NonEditableProperty(string propertyName, string? displayValue) : base(propertyName)
    {
        DisplayValue = displayValue;
    }
}