using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.EmitterManagement;
using Parme.Net.Editor.Messages;

namespace Parme.Net.Editor.ViewModels;

public partial class InitializerListViewModel : ObservableObject, IRecipient<InitializersChangedMessage>
{
    public record Item(Guid Id, string DisplayText);
    
    private bool _isUpdating;

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasItemSelected))] 
    private Item? _selectedItem;

    public bool HasItemSelected => _selectedItem != null;

    public InitializerListViewModel()
    {
        WeakReferenceMessenger.Default.Register(this);

        var message = WeakReferenceMessenger.Default.Send<CurrentInitializersRequestMessage>();
        UpdateItems(message.Response);
    }

    public ObservableCollection<Item> Items { get; } = new();

    public void Receive(InitializersChangedMessage message)
    {
        UpdateItems(message.Value);
    }
    
    private void UpdateItems(IReadOnlyList<TaggedInitializer> initializers)
    {
        _isUpdating = true;

        var selectedItemId = _selectedItem?.Id;
        _selectedItem = null;
        Items.Clear();

        foreach (var initializer in initializers)
        {
            var item = new Item(initializer.Id, initializer.Initializer.GetType().Name);
            if (item.Id == selectedItemId)
            {
                SelectedItem = item;
            }
            
            Items.Add(item);
        }
        _isUpdating = false;
    }
}