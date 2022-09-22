using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.EmitterManagement;
using Parme.Net.Editor.Messages;

namespace Parme.Net.Editor.ViewModels;

public partial class ModifierListViewModel : ObservableObject, IRecipient<ModifiersChangedMessage>
{
    private bool _isUpdating;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasItemSelected))] private Item? _selectedItem;

    public bool HasItemSelected => _selectedItem != null;

    public ObservableCollection<Item> Items { get; } = new();

    public ModifierListViewModel()
    {
        PropertyChanged += OnPropertyChanged;
        WeakReferenceMessenger.Default.Register(this);
        
        var modifiers = WeakReferenceMessenger.Default.Send(new CurrentModifiersRequestMessage());
        UpdateItems(modifiers.Response);
    }

    public void Receive(ModifiersChangedMessage message)
    {
        UpdateItems(message.Value);
    }

    private void UpdateItems(IReadOnlyList<TaggedModifier> modifiers)
    {
        _isUpdating = true;

        var selectedItemId = _selectedItem?.Id;
        _selectedItem = null;
        Items.Clear();

        foreach (var modifier in modifiers)
        {
            var item = new Item(modifier.Id, modifier.Modifier.GetType().Name);
            if (item.Id == selectedItemId)
            {
                SelectedItem = item;
            }
            
            Items.Add(item);
        }
        

        _isUpdating = false;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }
    }

    public record Item(Guid Id, string DisplayText);
}