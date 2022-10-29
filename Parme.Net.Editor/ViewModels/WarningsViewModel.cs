using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.Messages;

namespace Parme.Net.Editor.ViewModels;

public partial class WarningsViewModel : ObservableObject, IRecipient<EmitterConfigChangedMessage>
{
    public record Item(Guid Id, string WarningText);

    [ObservableProperty] private bool _hasAnyWarnings;

    public ObservableCollection<string> Items { get; } = new();

    public WarningsViewModel()
    {
        var currentEmitter = WeakReferenceMessenger.Default.Send<CurrentEmitterConfigRequestMessage>();
        UpdateWarningList(currentEmitter.Response);
        
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(EmitterConfigChangedMessage message)
    {
        UpdateWarningList(message.Value);
    }

    private void UpdateWarningList(EmitterConfig? emitterConfig)
    {
        Items.Clear();

        if (emitterConfig != null)
        {
            AddRequiredProperties(emitterConfig);
            AddPropertiesNotInitialized(emitterConfig);
            AddPropertiesSetButNotRead(emitterConfig);
        }

        HasAnyWarnings = Items.Any();
    }

    private void AddPropertiesNotInitialized(EmitterConfig emitterConfig)
    {
        var initializedProperties = emitterConfig.Initializers
            .SelectMany(x => x.PropertiesISet)
            .ToHashSet();
        
        var missingProperties = emitterConfig.Modifiers
            .SelectMany(x => x.PropertiesIRead.Union(x.PropertiesIUpdate))
            .Where(x => !initializedProperties.Contains(x))
            .ToArray();

        foreach (var property in missingProperties)
        {
            var text = $"The property {property.Name}({property.Type.Name}) is read but never initialized";
            Items.Add(text);
        }
    }

    private void AddPropertiesSetButNotRead(EmitterConfig emitterConfig)
    {
        var readProperties = emitterConfig.Modifiers
            .SelectMany(x => x.PropertiesIRead)
            .Union(new[]
            {
                // Properties usually only read at render time
                StandardParmeProperties.CurrentAlpha,
                StandardParmeProperties.CurrentBlue,
                StandardParmeProperties.CurrentGreen,
                StandardParmeProperties.CurrentRed,
                StandardParmeProperties.CurrentHeight,
                StandardParmeProperties.CurrentWidth,
                StandardParmeProperties.PositionX,
                StandardParmeProperties.PositionY,
                StandardParmeProperties.RotationInRadians,
                
                // Initial properties can be set but not read when no interpolation is used
                StandardParmeProperties.InitialAlpha,
                StandardParmeProperties.InitialBlue,
                StandardParmeProperties.InitialGreen,
                StandardParmeProperties.InitialRed,
                StandardParmeProperties.InitialHeight,
                StandardParmeProperties.InitialWidth,
            })
            .ToHashSet();
        
        var missingProperties = emitterConfig.Initializers
            .SelectMany(x => x.PropertiesISet)
            .Union(emitterConfig.Modifiers.SelectMany(x => x.PropertiesIUpdate))
            .Where(x => !readProperties.Contains(x))
            .ToHashSet();

        foreach (var property in missingProperties)
        {
            var text = $"The property {property.Name}({property.Type.Name}) is set but never read";
            Items.Add(text);
        }
    }

    private void AddRequiredProperties(EmitterConfig emitterConfig)
    {
        var setProperties = emitterConfig.Initializers
            .SelectMany(x => x.PropertiesISet)
            .Union(emitterConfig.Modifiers.SelectMany(x => x.PropertiesIUpdate))
            .ToHashSet();

        var missingProperties = new[]
            {
                StandardParmeProperties.CurrentWidth,
                StandardParmeProperties.CurrentHeight,
                StandardParmeProperties.CurrentAlpha,
                StandardParmeProperties.CurrentBlue,
                StandardParmeProperties.CurrentGreen,
                StandardParmeProperties.CurrentRed,
            }.Where(x => !setProperties.Contains(x))
            .ToArray();

        foreach (var property in missingProperties)
        {
            var text = $"The property {property.Name}({property.Type.Name}) is required but never set";
            Items.Add(text);
        }
    }
}