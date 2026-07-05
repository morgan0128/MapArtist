using BaseLib;
using Godot;
using MapArtist.MapArtistCode.Config;
using MapArtist.MapArtistCode.GUI;
using MapArtist.MapArtistCode.GUI.Items;
using MapArtist.MapArtistCode.GUI.Items.Abstract;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using NMapArtistApplyButtonItem = MapArtist.MapArtistCode.GUI.Items.Buttons.NMapArtistApplyButtonItem;
using NMapArtistGuiButtonItem = MapArtist.MapArtistCode.GUI.NMapArtistGuiButtonItem;
using NMapArtistResetButtonItem = MapArtist.MapArtistCode.GUI.Items.Buttons.NMapArtistResetButtonItem;

namespace MapArtist.MapArtistCode;

public class MapArtistGuiInitializer
{
//--------------------------------------------------- Singleton ------------------------------------------------
    static MapArtistGuiInitializer() { }
    private MapArtistGuiInitializer() { }
    public static MapArtistGuiInitializer Instance { get; } = new MapArtistGuiInitializer();
//--------------------------------------------------------------------------------------------------------------

    private NMapScreen? _existingMapScene; // The single, instantiated NMapScreen scene itself
    
    // An existing Icon pulled from the Map Scene. Make new icons by deep copying then modifying Texture.
    private TextureRect? _prototypeIcon;

    // Used in this class for the initially rendered states of the MapArtist button icons
    private static readonly StringName ApplyImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply.png";
    private static readonly StringName ResetImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset.png";
    private static readonly StringName WidthImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width.png";
    private static readonly StringName LogoImagePath = "res://MapArtist/Images/CustomIcons/mapartist_logo.png";
    
    // The button added to the existing DrawingTools/HBoxContainer to toggle display of the MapArtist GUI
    private NMapArtistGuiButtonItem? _guiDisplayButton;
    
    // Container for the MapArtist GUI
    private NMapArtistGui? _guiContainer;

    public NMapArtistGui InitializeMapArtistNodes(NMapScreen existingMapScene)
    {
        _existingMapScene = existingMapScene;
        CompleteSetupForAddedNode();
        InitializeGui();
        return _existingMapScene.GetNode<NMapArtistGui>("MapArtistGUI");
    }


    private void CompleteSetupForAddedNode()
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to call InitializeAddedNodeGuiButton() before" +
                                     " assigning _existingMapScene.");
            return;
        }
        
        // the AddedNode
        _guiDisplayButton = _existingMapScene.GetNode<NMapArtistGuiButtonItem>("DrawingTools/HBoxContainer/MapArtistGUIButton");
        
        if (_guiDisplayButton == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Failed to fetch or assign _guiDisplayButton from _existingMapScene.");
            return;
        }
        
        InitializePrototypeIcon();
        _guiDisplayButton.InitializeIconUseDeepCopy(_prototypeIcon, LogoImagePath);
        
        // Have DrawingTools expand horizontally to visually house the newly added toggleGUI button
        var dTools = _existingMapScene.GetNode<NinePatchRect>("DrawingTools");
        dTools.SetOffset(Side.Right, (dTools.GetOffset(Side.Right) + 68f));
        var dToolsHBox = _existingMapScene.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
        dToolsHBox.SetOffset(Side.Left, (dToolsHBox.GetOffset(Side.Left) - 34f));
        dToolsHBox.SetOffset(Side.Right, (dToolsHBox.GetOffset(Side.Right) + 34f));
    }
    
    private void InitializeGui()
    {
        ConstructGui(MapArtistConfig.TopLeftGui);
    }

    private void ConstructGui(bool topLeft)
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to call ConstructGui() before" +
                                    " assigning _existingMapScene.");
            return;
        }

        _guiContainer = new NMapArtistGui();
        _existingMapScene.AddChild(_guiContainer);
        
        if (topLeft)
        {
            ConstructGuiRowItemColorPicker();
            ConstructGuiRowButtons();
        }
        else
        {
            _guiContainer.AddThemeConstantOverride("separation", 0);
            _guiContainer.SetGlobalPosition(new Vector2(1605f, 725f));
            if (MapArtistConfig.ColorSamplerTool)
            {
                // lazy way: to get v1.0.2 out today. refactor UI setup/config interactions later
                _guiContainer.SetGlobalPosition(new Vector2(1605f, 720f));
            }
            ConstructGuiRowButtons();
            ConstructGuiRowItemColorPicker();
        }
    }
    
    private void ConstructGuiRowItemColorPicker()
    {
        var colorPicker = new NColorPickerItem();
        var player = Util.GetLocalPlayer();
        if (player != null)
        {
            colorPicker.Color = player.Character.MapDrawingColor;
        }
        _guiContainer?.AddItemColorPicker(colorPicker);
    }
    
    private void ConstructGuiRowButtons()
    {
        var container = new NMapArtistBoxContainerItem();
        container.Name = "BrushPropertyButtonContainer";
        _guiContainer?.AddItem(container);
        
        var applyButton = new NMapArtistApplyButtonItem();
        applyButton.InitializeIconUseDeepCopy(_prototypeIcon, ApplyImagePath);
        container.AddItem(applyButton);
        
        var resetButton = new NMapArtistResetButtonItem();
        resetButton.InitializeIconUseDeepCopy(_prototypeIcon, ResetImagePath);
        container.AddItem(resetButton);

        var brushWidth = new NMapArtistBrushWidthItem(container);
        container.AddItem(brushWidth);
        brushWidth.InitializeIconUseDeepCopy(_prototypeIcon, WidthImagePath);
        MapArtistController.MapArtistController.Instance.BrushWidthInterface = brushWidth;
    }
    
    private void InitializePrototypeIcon()
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to call DebugInitializePlaceholderIcon() before" +
                                     " assigning _existingMapScene.");
            return;
        }
        
        _prototypeIcon = _existingMapScene.GetNode<TextureRect>("DrawingTools/HBoxContainer/ClearButton/Icon");
    }
    
}