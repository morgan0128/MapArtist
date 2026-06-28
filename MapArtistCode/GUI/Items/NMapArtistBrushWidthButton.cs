using Godot;
using MapArtist.MapArtistCode.Config;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Range = Godot.Range;

namespace MapArtist.MapArtistCode.GUI.Items;

[ScriptPath("res://MapArtistCode/GUI/Items/NMapArtistBrushWidthButton.cs")]
public partial class NMapArtistBrushWidthButton : GUI.Items.Abstract.NMapArtistButton
{
    private static readonly StringName ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width.png";
    private static readonly StringName GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_width_glow.png";
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");

    
    public NMapArtistBrushWidthButton()
    {
    }
    
    public NMapArtistBrushWidthButton(Control mapArtistAncestorItemContainer)
    {
        Name = "MapArtistBrushWidthButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;

        MapArtistButtonContainer = mapArtistAncestorItemContainer;
    }

    public override void _Ready()
    {
        base._Ready();
        // Localization
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-BRUSH_WIDTH.title"), locDesc);
        
        ConnectSignals();
    }
    
    protected override void OnPress()
    {
        base.OnPress();
        MapArtistController.MapArtistController.Instance.ToggleBrushWidthGui();
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
