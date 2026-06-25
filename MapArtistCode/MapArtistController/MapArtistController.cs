using BaseLib;
using BaseLib.Abstracts;
using Godot;
using MapArtist.MapArtistCode.Config;
using MapArtist.MapArtistCode.GUI.Items;
using MapArtist.MapArtistCode.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode.MapArtistController;

public sealed class MapArtistController
{
//--------------------------------------------------- Singleton ------------------------------------------------
    static MapArtistController() { }
    private MapArtistController() { }
    public static MapArtistController Instance { get; } = new MapArtistController();
//--------------------------------------------------------------------------------------------------------------
//-------------------------------------- Relevant NMapScreen Scene Nodes ---------------------------------------
    private NMapScreen? _existingMapScene; // The single, instantiated NMapScreen scene itself
    
    // An existing Icon pulled from the Map Scene. Make new icons by shallow copying and modifying Texture.
    private TextureRect? _prototypeIcon;

    private static readonly StringName ApplyImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply.png";
    // private static readonly StringName ApplyGlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply_glow.png";
    private static readonly StringName ResetImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset.png";
    // private static readonly StringName ResetGlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset_glow.png";
    private static readonly StringName WidthImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width.png";
    // private static readonly StringName WidthGlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width_glow.png";
    private static readonly StringName LogoImagePath = "res://MapArtist/Images/CustomIcons/mapartist_logo.png";
    
    // // The button added to the existing DrawingTools/HBoxContainer to display the MapArtist GUI
    private GUI.NMapArtistGUIButton? _guiDisplayButton;
    
    // // Container for the MapArtist GUI
    private GUI.NMapArtistGUINode? _guiContainer;
    
    // // Both a row and an item; no container exclusively for this item; first row of the MapArtist GUI container
    // private NColorPicker? _rowitemColorPicker;
    //
    // // Container for buttons row of the MapArtist GUI container
    // private HBoxContainer? _rowButtonsContainer;
    // private NMapArtistBrushWidthButton? _itemWidthButton;
    // private NMapArtistApplyButton? _itemApplyButton;
    // private NMapArtistResetButton? _itemResetButton;
    // private HBoxContainer? _bWidthSliderContainer;
    // private HSlider? _bWidthSlider;
    // private Label? _bWidthLabel;
    
//--------------------------------------------------------------------------------------------------------------
//---------------------------------------- Additional Member Variables -----------------------------------------
    private Player? _localPlayer; // Needed for controller logic
//--------------------------------------------------------------------------------------------------------------

//----------------------------------- GUI Initialization Methods and Helpers -----------------------------------
    public void InitializeGui(NMapScreen? mapScene)
    {
        if (mapScene == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Null mapScene passed to InitializeExisting.");
            return;
        }
        
        _existingMapScene =  mapScene;
        _guiDisplayButton = TemporaryRefactoredInitializer.Instance.CompleteSetupAddedNodeGuiButton(mapScene);
        _guiContainer = TemporaryRefactoredInitializer.Instance.InitializeGui();
        
        
        BroadcastCurrentSettings();
        CustomMessageWrapper.Send(new MapArtistBrushSettingsRequestMessage());
    }
//---------------------------------------------- Controller Logic ----------------------------------------------
    private Player? FetchLocalPlayer()
    {
        // Not yet tested/suitable for Multiplayer
        
        if (_localPlayer != null)
        {
            // results in ResetSettings() grabbing the wrong color to display in ColorPicker where character changed
            return _localPlayer;
        }

        var currState = RunManager.Instance.DebugOnlyGetState();
        if (currState == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Failed to load current state");
            return null;
        }

        _localPlayer = currState.GetPlayer(RunManager.Instance.NetService.NetId);
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
        MapArtistDictionaries.SetColor(player, _guiContainer.GetRowItemColorPicker().Color);
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
            var widthVal = _guiContainer.GetItemBrushWidthButton().BrushWidth;
            MapArtistDictionaries.SetPenWidth(player, (float)widthVal);
            CustomMessageWrapper.Send(new MapArtistBrushSettingsMessage(_guiContainer.GetRowItemColorPicker().Color, (float)widthVal));
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

        // if (_rowitemColorPicker == null)
        // {
        //     BaseLibMain.Logger.Info("[MapArtistController] _rowitemColorPicker == null on ResetSettings() call.");
        //     return;
        // }
        //
        // if (_bWidthSlider == null)
        // {
        //     BaseLibMain.Logger.Info("[MapArtistController] _bWidthSlider == null on ResetSettings() call.");
        //     return;
        // }
        
        MapArtistDictionaries.ClearAll(player);
        _guiContainer.GetRowItemColorPicker().Color = player.Character.MapDrawingColor;

        _guiContainer.GetItemWidthSlider().Value = 4; // changing slider value without Brush width; ValueChanged signal to update BrushWidth
        CustomMessageWrapper.Send(MapArtistBrushSettingsMessage.Reset());
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

    internal void ResetRunState()
    {
        _localPlayer = null;
        TemporaryRefactoredInitializer.Instance.TemporaryResetPlayer();
        MapArtistDictionaries.ClearAll();
    }

    // public void DisableDrawingMode()
    // {
    //     var player = FetchLocalPlayer();
    //     if (player == null)
    //     {
    //         return;
    //     }
    //     //
    //     // var scenePath = SceneHelper.GetScenePath("screens/map/map_line_draw");
    //     //
    //     // var parent = _existingMapScene.GetParent();
    //     // if (parent == null)
    //     // {
    //     //     BaseLibMain.Logger.Info("[MapArtistController] parent is null");
    //     // }
    //     // else
    //     // {
    //     //     BaseLibMain.Logger.Info("[MapArtistController] parent is " + parent.Name);
    //     // }
    //
    //     var nMapDrawings = _existingMapScene.GetNode<NMapDrawings>("TheMap/Drawings");
    //     if (nMapDrawings == null)
    //         {
    //
    //         }
    //         else
    //         {
    //             nMapDrawings.SetDrawingModeLocal(DrawingMode.None);
    //         }
    //
    //
    // }

}
