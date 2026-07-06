using Godot;
using MapArtist.MapArtistCode.GUI.Items.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace MapArtist.MapArtistCode.GUI.Items.Buttons;

[ScriptPath("res://MapArtistCode/GUI/Items/NMapArtistApplyButtonItem.cs")]
public partial class NMapArtistApplyButtonItem : NMapArtistButtonItem
{
    public NMapArtistApplyButtonItem()
    {
        Name = "MapArtistApplyButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;
        
        ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply.png";
        GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply_glow.png";
    }
    
    public override void _Ready()
    {
        base._Ready();
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-APPLY_BUTTON.description");
        HoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-APPLY_BUTTON.title"), locDesc);
        
        ConnectSignals();
    }
    
    protected override void OnPress()
    {
        base.OnPress();
        MapArtistController.MapArtistController.Instance.ApplySettings();
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
