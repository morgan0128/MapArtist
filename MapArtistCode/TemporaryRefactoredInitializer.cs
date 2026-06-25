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

public class TemporaryRefactoredInitializer
{
//--------------------------------------------------- Singleton ------------------------------------------------
    static TemporaryRefactoredInitializer() { }
    private TemporaryRefactoredInitializer() { }
    public static TemporaryRefactoredInitializer Instance { get; } = new TemporaryRefactoredInitializer();
//--------------------------------------------------------------------------------------------------------------

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
    
    
    // temporary
    private Player? _localPlayer;




    public NMapArtistGUIButton CompleteSetupAddedNodeGuiButton(NMapScreen? existingMapScene)
    {
        _existingMapScene = existingMapScene;
        
        // if (_existingMapScene == null)
        // {
        //     BaseLibMain.Logger.Info("[MapArtistController] Attempted to call InitializeAddedNodeGuiButton() before" +
        //                              " assigning _existingMapScene.");
        //     return;
        // }
        
        _guiDisplayButton = _existingMapScene.GetNode<GUI.NMapArtistGUIButton>("DrawingTools/HBoxContainer/MapArtistGUIButton");
        // if (_guiDisplayButton == null)
        // {
        //     BaseLibMain.Logger.Error("[MapArtistController] Failed to fetch or assign _guiDisplayButton from _existingMapScene.");
        //     return;
        // }
        
        InitializePrototypeIcon();
        InitializeIconUseShallow(_prototypeIcon, LogoImagePath, _guiDisplayButton);
        return _guiDisplayButton;
        // _guiDisplayButton.GetNode<TextureRect>("Icon").SetScale(new Vector2(0.5f, 0.5f));
    }
    
    public NMapArtistGUINode InitializeGui()
    {
        ConstructGui(MapArtistConfig.TopLeftGui);
        return _guiContainer;
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
    
    private static TextureRect ShallowCopyIcon(TextureRect toCopy)
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

    private static TextureRect ShallowCopyIcon(TextureRect toCopy, StringName imagePath)
    {
        var icon = ShallowCopyIcon(toCopy);
        icon.Texture = ResourceLoader.Load<Texture2D>(imagePath);

        return icon;
    }
    
    private static void InitializeIconUseShallow(TextureRect toCopy, StringName imagePath, GUI.Items.Abstract.NMapArtistButton forButton)
    {
        var icon = ShallowCopyIcon(toCopy, imagePath);
        forButton.SetIcon(icon);
        forButton.AddChild(icon);
    }

    private void ConstructGui(bool topLeft)
    {
        if (_existingMapScene == null)
        {
            BaseLibMain.Logger.Info("[MapArtistController] Attempted to call ConstructGui() before" +
                                    " assigning _existingMapScene.");
            return;
        }
        
        // Have DrawingTools expand horizontally to visually house the newly added toggleGUI button
        var dTools = _existingMapScene.GetNode<NinePatchRect>("DrawingTools");
        dTools.SetOffset(Side.Right, (dTools.GetOffset(Side.Right) + 68f));
        var dToolsHBox = _existingMapScene.GetNode<HBoxContainer>("DrawingTools/HBoxContainer");
        dToolsHBox.SetOffset(Side.Left, (dToolsHBox.GetOffset(Side.Left) - 34f));
        dToolsHBox.SetOffset(Side.Right, (dToolsHBox.GetOffset(Side.Right) + 34f));
        
        
        _guiContainer = new NMapArtistGUINode();
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
        var player = TemporaryFetchLocalPlayer();
        if (player != null)
        {
            colorPicker.Color = TemporaryFetchLocalPlayer().Character.MapDrawingColor;
        }
        _guiContainer.AssignRowitemColorPicker(colorPicker);
    }
    
    private void ConstructGuiRowButtons()
    {
        var container = InitHBoxContainer();
        container.Name = "BrushPropertyButtonContainer";
        _guiContainer.AssignRowButtonsContainer(container);
        
        var applyButton = new NMapArtistApplyButton();
        InitializeIconUseShallow(_prototypeIcon, ApplyImagePath, applyButton);
        _guiContainer.AssignItemApplyButton(applyButton);
        
        var resetButton = new NMapArtistResetButton();
        InitializeIconUseShallow(_prototypeIcon, ResetImagePath, resetButton);
        _guiContainer.AssignItemResetButton(resetButton);
        
        var widthButton = new NMapArtistBrushWidthButton();
        InitializeIconUseShallow(_prototypeIcon, WidthImagePath, widthButton);
        _guiContainer.AssignItemWidthButton(widthButton);
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
    
    private Player? TemporaryFetchLocalPlayer()
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
    
    public void TemporaryResetPlayer()
    {
        _localPlayer = null;
    }
}