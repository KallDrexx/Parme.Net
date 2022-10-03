using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.EmitterManagement;
using Parme.Net.Editor.Messages;

namespace Parme.Net.Editor.ViewModels;

public partial class InitializerListViewModel : ObservableObject, 
    IRecipient<InitializersChangedMessage>,
    IRecipient<ItemSelectedMessage>,
    IRecipient<TriggerSelectedMessage>
{
    public record Item(Guid Id, string DisplayText);
    
    private bool _isUpdating;

    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(HasItemSelected))] 
    private Item? _selectedItem;

    public bool HasItemSelected => _selectedItem != null;
    public ObservableCollection<Item> Items { get; } = new();
    public ICommand RemoveInitializerCommand { get; }
    public ICommand AddInitializerCommand { get; }

    public InitializerListViewModel()
    {
        PropertyChanged += OnPropertyChanged;
        RemoveInitializerCommand = new RelayCommand(ExecuteRemoveCommand);
        AddInitializerCommand = new RelayCommand(ExecuteAddCommand);
        WeakReferenceMessenger.Default.RegisterAll(this);

        var message = WeakReferenceMessenger.Default.Send<CurrentInitializersRequestMessage>();
        UpdateItems(message.Response);
    }

    public void Receive(InitializersChangedMessage message)
    {
        if (_isUpdating)
        {
            return;
        }
        
        UpdateItems(message.Value);
    }

    public void Receive(ItemSelectedMessage message)
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;
        SelectedItem = Items.FirstOrDefault(x => x.Id == message.Value);
        OnPropertyChanged(nameof(HasItemSelected));
        _isUpdating = false;
    }

    public void Receive(TriggerSelectedMessage message)
    {
        SelectedItem = null;
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

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;
        switch (e.PropertyName)
        {
            case nameof(SelectedItem):
                OnPropertyChanged(nameof(HasItemSelected));
                WeakReferenceMessenger.Default.Send(new ItemSelectedMessage(SelectedItem?.Id));
                break;
        }

        _isUpdating = false;
    }

    private void ExecuteRemoveCommand()
    {
        if (_selectedItem == null)
        {
            return;
        }

        WeakReferenceMessenger.Default.Send(new ItemRemovedMessage(_selectedItem.Id));
        WeakReferenceMessenger.Default.Send(new ItemSelectedMessage(null));
    }

    private void ExecuteAddCommand()
    {
        WeakReferenceMessenger.Default.Send(new ItemSelectedMessage(null));
        WeakReferenceMessenger.Default.Send(new AddInitializerMessage());
    }
}