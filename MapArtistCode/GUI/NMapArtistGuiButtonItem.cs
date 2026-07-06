using BaseLib.Utils;
using Godot;
using MapArtist.MapArtistCode.GUI.Items.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode.GUI;

[ScriptPath("res://MapArtistCode/GUI/NMapArtistGuiButtonItem.cs")]
public partial class NMapArtistGuiButtonItem : NMapArtistButtonItem
{
    private NMapScreen? _mapScene;
    private Control? _drawingToolHolder;
    
    // The existing, instantiated NMapScreen passed by constructor (because using lambda: AddedNode)
    private NMapArtistGuiButtonItem(NMapScreen mapScene)
    {
        Name = "MapArtistGUIButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(68, 60);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;

        _mapScene = mapScene;
        
        ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_logo.png";
        GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_logo_glow.png";
    }

    private NMapArtistGuiButtonItem() {}
    
    public static readonly AddedNode<NMapScreen, NMapArtistGuiButtonItem> Map = new((mapScreen) =>
    {
        // grab the drawing tools node
        var drawingTools = mapScreen.GetNode<NinePatchRect>("DrawingTools");
        
        // grab drawing tools display container node
        var parent = drawingTools.GetNode<HBoxContainer>("HBoxContainer");
        
        // grab the (to be) neighboring button
        var clearButton = (NButton)parent.GetNode("ClearButton");
        
        // initialize, grabbing the instantiated NMapScreen node to give to controller for gui initialization process
        var button = new NMapArtistGuiButtonItem(mapScreen);
        
        // add this node to the drawing tools container
        parent.AddChild(button);
        
        // introduce the new neighbors
        clearButton.FocusNeighborRight = button.GetPath();
        button.FocusNeighborLeft = new NodePath("../ClearButton");
        
        button._drawingToolHolder = parent;
        button.MapArtistButtonContainer = button._drawingToolHolder;
        
        // return the newly created color picker button
        return button;
    });

    public override void _Ready()
    {
        var locDesc = new LocString("static_hover_tips", "MAPARTIST-GUI_BUTTON.description");
        HoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-GUI_BUTTON.title"), locDesc);
        
        ConnectSignals();
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        if (_mapScene == null) return;
        
        if (_mapScene.GetNodeOrNull<VBoxContainer>("MapArtistGUI") == null)
        {
            MapArtistController.MapArtistController.Instance.InitializeGui(_mapScene);
        }
    }
    
    protected override void OnPress()
    {
        base.OnPress();
        MapArtistController.MapArtistController.Instance.ToggleGui();
    }

    protected override void OnFocus()
    {
        ChildIconSfxGlow(GlowImagePath, ActiveColor);
    }

    protected override void OnUnfocus()
    {
        ChildIconSfxUnglow(ImagePath, InactiveColor);
    }
    
}
