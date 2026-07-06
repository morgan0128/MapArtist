using BaseLib;
using Godot;
using MapArtist.MapArtistCode.Config;
using MapArtist.MapArtistCode.GUI;
using MapArtist.MapArtistCode.GUI.Items;
using MapArtist.MapArtistCode.GUI.Items.Abstract;
using MapArtist.MapArtistCode.GUI.Items.Buttons;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using NMapArtistApplyButtonItem = MapArtist.MapArtistCode.GUI.Items.Buttons.NMapArtistApplyButtonItem;
using NMapArtistGuiButtonItem = MapArtist.MapArtistCode.GUI.NMapArtistGuiButtonItem;
using NMapArtistResetButtonItem = MapArtist.MapArtistCode.GUI.Items.Buttons.NMapArtistResetButtonItem;

namespace MapArtist.MapArtistCode;

public class MapArtistGuiDirector
{
//--------------------------------------------------- Singleton ------------------------------------------------
    static MapArtistGuiDirector() { }
    private MapArtistGuiDirector() { }
    public static MapArtistGuiDirector Instance { get; } = new MapArtistGuiDirector();
//--------------------------------------------------------------------------------------------------------------

    private NMapScreen? _existingMapScene; // The single, instantiated NMapScreen scene itself
    
    // An existing Icon pulled from the Map Scene. Make new icons by deep copying then modifying Texture.
    private TextureRect? _prototypeIcon;

    // Used in this class for the initially rendered states of the MapArtist button icons
    // private static readonly StringName ApplyImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply.png";
    // private static readonly StringName ResetImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset.png";
    // private static readonly StringName WidthImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width.png";
    // private static readonly StringName LogoImagePath = "res://MapArtist/Images/CustomIcons/mapartist_logo.png";
    
    // The button added to the existing DrawingTools/HBoxContainer to toggle display of the MapArtist GUI
    private NMapArtistGuiButtonItem? _guiDisplayButton;
    
    // Container for the MapArtist GUI
    private NMapArtistGui? _guiContainer;

    public NMapArtistGui? InitializeMapArtistNodes(NMapScreen existingMapScene)
    {
        _existingMapScene = existingMapScene;
        CompleteSetupForAddedNode();
        ConstructGui(MapArtistConfig.TopLeftGui);
        // return _existingMapScene.GetNode<NMapArtistGui>("MapArtistGUI");
        return _guiContainer;
    }


    private void CompleteSetupForAddedNode()
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistDirector] Attempted to call InitializeAddedNodeGuiButton() before assigning _existingMapScene.");
            return;
        }
        
        // the AddedNode
        _guiDisplayButton = _existingMapScene.GetNodeOrNull<NMapArtistGuiButtonItem>("DrawingTools/HBoxContainer/MapArtistGUIButton");
        
        if (_guiDisplayButton == null)
        {
            BaseLibMain.Logger.Error("[MapArtistDirector] Failed to fetch or assign _guiDisplayButton from _existingMapScene.");
            return;
        }
        
        InitializePrototypeIcon();
        _guiDisplayButton.InitializeIconUseDeepCopy(_prototypeIcon);
        
        // Have DrawingTools expand horizontally to visually house the newly added toggleGUI button
        var dTools = _existingMapScene.GetNode<NinePatchRect>("DrawingTools");
        dTools.SetOffset(Side.Right, (dTools.GetOffset(Side.Right) + 68f));
        var dToolsHBox = _existingMapScene.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
        dToolsHBox.SetOffset(Side.Left, (dToolsHBox.GetOffset(Side.Left) - 34f));
        dToolsHBox.SetOffset(Side.Right, (dToolsHBox.GetOffset(Side.Right) + 34f));
    }

    private void ConstructGui(bool topLeft)
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistDirector] Attempted to call ConstructGui() before assigning _existingMapScene.");
            return;
        }

        _guiContainer = new NMapArtistGui();
        _existingMapScene.AddChild(_guiContainer);
        
        if (topLeft)
        {
            ConstructGuiRowItemColorPicker();
            ConstructGuiRowButtons();
            // ConstructButtonRowsStandard();
            _guiContainer.SetGlobalPosition(new Vector2(12f, 158f));
        }
        else
        {
            ConstructGuiRowButtons();
            // ConstructButtonRowsStandard();
            ConstructGuiRowItemColorPicker();
            _guiContainer.SetGlobalPosition(new Vector2(1590f, 717f));
        }
        ApplyConfigColorSampler();
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

    private void ConstructButtonRowsStandard()
    {
        ConstructGuiButtonRowTools();
        if (!MapArtistConfig.SynchronizedColorPicker && !MapArtistConfig.SynchronizedWidthSlider)
        {
            ConstructGuiButtonRowApplyReset();
        }
    }
    
    private void ConstructGuiRowButtons()
    {
        var container = new NMapArtistBoxContainerItem();
        container.SetHSizeFlags(Control.SizeFlags.Fill);
        container.Name = "BrushPropertyButtonContainer";
        _guiContainer?.AddItem(container);

        if (!MapArtistConfig.SynchronizedColorPicker || !MapArtistConfig.SynchronizedWidthSlider)
        {
            var applyButton = new NMapArtistApplyButtonItem();
            applyButton.InitializeIconUseDeepCopy(_prototypeIcon);
            container.AddItem(applyButton);
        }
        
        var resetButton = new NMapArtistResetButtonItem();
        resetButton.InitializeIconUseDeepCopy(_prototypeIcon);
        container.AddItem(resetButton);
        
        var undoButton = new NMapArtistUndoButtonItem();
        undoButton.InitializeIconUseDeepCopy(_prototypeIcon);
        container.AddItem(undoButton);
        
        var redoButton = new NMapArtistRedoButtonItem();
        redoButton.InitializeIconUseDeepCopy(_prototypeIcon);
        container.AddItem(redoButton);
        
        var brushWidth = new NMapArtistBrushWidthItem(container);
        container.AddItem(brushWidth);
        brushWidth.InitializeIconUseDeepCopy(_prototypeIcon);
        MapArtistController.MapArtistController.Instance.BrushWidthInterface = brushWidth;
    }
    
    private void ConstructGuiButtonRowApplyReset()
    {
        var container = new NMapArtistBoxContainerItem();
        container.SetHSizeFlags(Control.SizeFlags.Fill);
        container.Name = "ApplyAndResetButtonContainer";
        _guiContainer?.AddItem(container);
        
        var applyButton = new NMapArtistApplyButtonItem();
        applyButton.InitializeIconUseDeepCopy(_prototypeIcon);
        container.AddItem(applyButton);
        
        var resetButton = new NMapArtistResetButtonItem();
        resetButton.InitializeIconUseDeepCopy(_prototypeIcon);
        container.AddItem(resetButton);
    }
    
    private void ConstructGuiButtonRowTools()
    {
        var container = new NMapArtistBoxContainerItem();
        container.SetHSizeFlags(Control.SizeFlags.Fill);
        container.Name = "ToolsButtonContainer";
        _guiContainer?.AddItem(container);

        var undoButton = new NMapArtistUndoButtonItem();
        undoButton.InitializeIconUseDeepCopy(_prototypeIcon);
        container.AddItem(undoButton);
        
        var redoButton = new NMapArtistRedoButtonItem();
        redoButton.InitializeIconUseDeepCopy(_prototypeIcon);
        container.AddItem(redoButton);
        
        var brushWidth = new NMapArtistBrushWidthItem(container);
        container.AddItem(brushWidth);
        brushWidth.InitializeIconUseDeepCopy(_prototypeIcon);
        MapArtistController.MapArtistController.Instance.BrushWidthInterface = brushWidth;
    }

    private void ApplyConfigColorSampler()
    {
        if (_guiContainer == null || !_guiContainer.AssignedColorPicker())
        {
            BaseLibMain.Logger.Info("[MapArtistDirector] In ApplyConfigColorSampler(): Construct gui and assign color picker, first!");
            return;
        }
        
        _guiContainer.SetColorSamplerVisible(MapArtistConfig.ColorSamplerTool);
    }
    
    private void InitializePrototypeIcon()
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistDirector] Attempted to call InitializePrototypeIcon() before assigning _existingMapScene.");
            return;
        }
        
        _prototypeIcon = _existingMapScene.GetNodeOrNull<TextureRect>("DrawingTools/HBoxContainer/ClearButton/Icon");
    }
    
}