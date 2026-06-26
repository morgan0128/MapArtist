using BaseLib;
using BaseLib.Abstracts;
using Godot;
using MapArtist.MapArtistCode.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode.MapArtistController;

public sealed class MapArtistController
{
    static MapArtistController() { }
    private MapArtistController() { }
    public static MapArtistController Instance { get; } = new MapArtistController();
    
    private GUI.NMapArtistGUINode? _guiContainer;
    
    private Player? _localPlayer;
    
    private SubViewport _tempViewport;

    
    // Only to be called by NMapArtistGUIButton when enters tree
    public void InitializeGui(NMapScreen mapScene)
    {
        _guiContainer = MapArtistGuiInitializer.Instance.InitializeMapArtistNodes(mapScene);
        BroadcastCurrentSettings();
        CustomMessageWrapper.Send(new MapArtistBrushSettingsRequestMessage());
    }
    
    // Using "fetch" approach; so patched RunManager.Cleanup using Postfix to include call to ResetRunState()
    private Player? FetchLocalPlayer()
    {
        if (_localPlayer != null)
        {
            return _localPlayer;
        }
        _localPlayer = Util.GetLocalPlayer();
        return  _localPlayer;
    }

    public void ToggleGui()
    {
        if (_guiContainer == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] _guiContainer == null on ToggleGui() call.");
            return;
        }
        
        _guiContainer.Visible = !_guiContainer.Visible;
    }
    
    public void HideGui()
    {
        if (_guiContainer == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] _guiContainer == null on ToggleGui() call.");
            return;
        }
        
        // maybe tween animations later
        // _guiContainer.Tween = _guiContainer.CreateTween().SetParallel();
        // _guiContainer.Tween.TweenProperty((GodotObject) _guiContainer, (NodePath) "modulate:a", (Variant) 0.0f, 0.15);
        // _guiContainer.Tween.Kill();
        
        _guiContainer.Visible = false;
    }

    public void ApplySettings()
    {
        var player = FetchLocalPlayer();
        ApplySettingColor(player);
        ApplySettingWidth(player);
    }

    public void ApplySettingColor()
    {
        var player = FetchLocalPlayer();
        ApplySettingColor(player);
    }

    private void ApplySettingColor(Player? player)
    {
        if (player == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Failed to fetch player.");
            return;
        }

        // if (_rowitemColorPicker == null)
        // {
        //     BaseLibMain.Logger.Info("[MapArtistController] _rowitemColorPicker == null on ApplySettings() call.");
        //     return;
        // }
        
        // apply brush color
        MapArtistDictionaries.SetColor(player, _guiContainer.GetColorInColorPicker());
    }

    public void ApplySettingWidth()
    {
        var player = FetchLocalPlayer();
        ApplySettingWidth(player);
    }
    
    private void ApplySettingWidth(Player? player)
    {
        if (player == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Failed to fetch player.");
            return;
        }
        
        // if (_itemWidthButton == null)
        // {
        //     BaseLibMain.Logger.Info("[MapArtistController] _itemWidthButton == null on ApplySettings() call.");
        //     return;
        // }
        
        // apply pen width
        try {
            var widthVal = _guiContainer.GetValueBrushWidth();
            MapArtistDictionaries.SetPenWidth(player, (float)widthVal);
            CustomMessageWrapper.Send(new MapArtistBrushSettingsMessage(_guiContainer.GetColorInColorPicker(), (float)widthVal));
        } catch (FormatException notFloat)
        {
            // no valid pen width to apply... this should not be reached in current iteration
        }
    }
    
    
    
    public void ResetSettings(){
        var player = FetchLocalPlayer();
        if (player == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Failed to fetch player.");
            return;
        }
        
        
        // // test
        // /*
        MapArtistDictionaries.ClearAll(player);
        _guiContainer.SetColorInColorPicker(player.Character.MapDrawingColor);
        
        _guiContainer.SetValueBrushWidth(4);
        CustomMessageWrapper.Send(MapArtistBrushSettingsMessage.Reset());
        // */
        // UndoLine();
    }

    internal void BroadcastCurrentSettings(bool sendResetWhenDefault = false)
    {
        var player = FetchLocalPlayer();
        if (player == null)
        {
            return;
        }

        var hasColor = MapArtistDictionaries.TryGetColor(player, out var color);
        var hasWidth = MapArtistDictionaries.TryGetPenWidth(player, out var width);
        if (!hasColor && !hasWidth)
        {
            if (sendResetWhenDefault)
            {
                CustomMessageWrapper.Send(MapArtistBrushSettingsMessage.Reset());
            }

            return;
        }

        CustomMessageWrapper.Send(new MapArtistBrushSettingsMessage(
            hasColor ? color : player.Character.MapDrawingColor, hasWidth ? width : 4f));
    }

    public void UndoLine()
    {
        _tempViewport.RemoveChildSafely(_tempViewport.GetChildren().Last());
    }

    public void TemporaryUpdateViewport(SubViewport subViewport)
    {
        _tempViewport = subViewport;

    }

    internal void ResetRunState()
    {
        _localPlayer = null;
        MapArtistDictionaries.ClearAll();
    }
    

}
