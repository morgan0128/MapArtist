using BaseLib;
using BaseLib.Abstracts;
using Godot;
using MapArtist.MapArtistCode.Config;
using MapArtist.MapArtistCode.GUI;
using MapArtist.MapArtistCode.GUI.Items;
using MapArtist.MapArtistCode.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

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
    private NMapArtistGuiButton? _guiDisplayButton;
    
    // Container for the MapArtist GUI
    private NMapArtistGuiNode? _guiContainer;

    public NMapArtistGuiNode InitializeMapArtistNodes(NMapScreen existingMapScene)
    {
        _existingMapScene = existingMapScene;
        CompleteSetupForAddedNode();
        InitializeGui();
        return _existingMapScene.GetNode<NMapArtistGuiNode>("MapArtistGUI");
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
        _guiDisplayButton = _existingMapScene.GetNode<GUI.NMapArtistGuiButton>("DrawingTools/HBoxContainer/MapArtistGUIButton");
        
        if (_guiDisplayButton == null)
        {
            BaseLibMain.Logger.Error("[MapArtistController] Failed to fetch or assign _guiDisplayButton from _existingMapScene.");
            return;
        }
        
        InitializePrototypeIcon();
        InitializeIconUseDeepCopy(_prototypeIcon, LogoImagePath, _guiDisplayButton);
        
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

        _guiContainer = new NMapArtistGuiNode();
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
        var colorPicker = new NColorPicker();
        colorPicker.Name = "ItemColorPicker";
        colorPicker.UniqueNameInOwner = true;
        var player = Util.GetLocalPlayer();
        if (player != null)
        {
            colorPicker.Color = player.Character.MapDrawingColor;
        }
        _guiContainer.AssignRowitemColorPicker(colorPicker);
    }
    
    private void ConstructGuiRowButtons()
    {
        var container = InitHBoxContainer();
        container.Name = "BrushPropertyButtonContainer";
        _guiContainer.AssignRowButtonsContainer(container);
        
        var applyButton = new NMapArtistApplyButton();
        InitializeIconUseDeepCopy(_prototypeIcon, ApplyImagePath, applyButton);
        _guiContainer.AssignItemApplyButton(applyButton);
        
        var resetButton = new NMapArtistResetButton();
        InitializeIconUseDeepCopy(_prototypeIcon, ResetImagePath, resetButton);
        _guiContainer.AssignItemResetButton(resetButton);

        var brushWidth = new NMapArtistBrushWidth(container);
        _guiContainer.AssignItemBrushWidthInterface(brushWidth);
        InitializeIconUseDeepCopy(_prototypeIcon, WidthImagePath, brushWidth.WidthButton);
        MapArtistController.MapArtistController.Instance.BrushWidthInterface = brushWidth;
    }
    
    private static HBoxContainer InitHBoxContainer()
    {
        var hbc = new HBoxContainer();
        hbc.UniqueNameInOwner = true;
        hbc.SizeFlagsHorizontal = Control.SizeFlags.Fill;
        hbc.SizeFlagsVertical = Control.SizeFlags.Fill;
        hbc.MouseFilter = Control.MouseFilterEnum.Ignore;
      
        return hbc;
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
    
    private static TextureRect DeepCopyIcon(TextureRect toCopy)
    {
        var icon = new TextureRect();

        icon.Name = "Icon";
        
        icon.SelfModulate = toCopy.SelfModulate;
        icon.SetMaterial(toCopy.GetMaterial());
        icon.SetUseParentMaterial(toCopy.GetUseParentMaterial());
        icon.LayoutMode = toCopy.LayoutMode;
        icon.AnchorsPreset = toCopy.AnchorsPreset;
        icon.AnchorRight = toCopy.AnchorRight;
        icon.AnchorBottom = toCopy.AnchorBottom;
        icon.GrowHorizontal = toCopy.GrowHorizontal;
        icon.GrowVertical = toCopy.GrowVertical;
        icon.Scale =  new Vector2(toCopy.Scale.X, toCopy.Scale.Y);
        icon.PivotOffset = new Vector2(toCopy.PivotOffset.X, toCopy.PivotOffset.Y);
        icon.MouseFilter = toCopy.MouseFilter;
        icon.SetTexture(toCopy.GetTexture());
        icon.SetUseParentMaterial(toCopy.GetUseParentMaterial());
        icon.ExpandMode = toCopy.ExpandMode;
        icon.StretchMode = toCopy.StretchMode;
        
        return icon;
    }

    private static TextureRect DeepCopyIcon(TextureRect toCopy, StringName imagePath)
    {
        var icon = DeepCopyIcon(toCopy);
        icon.Texture = ResourceLoader.Load<Texture2D>(imagePath);

        return icon;
    }
    
    private static void InitializeIconUseDeepCopy(TextureRect toCopy, StringName imagePath, GUI.Items.Abstract.NMapArtistButton forButton)
    {
        var icon = DeepCopyIcon(toCopy, imagePath);
        forButton.SetIcon(icon);
        forButton.AddChild(icon);
    }
    
}