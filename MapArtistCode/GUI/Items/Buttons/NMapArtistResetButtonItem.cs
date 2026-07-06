using Godot;
using MapArtist.MapArtistCode.GUI.Items.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace MapArtist.MapArtistCode.GUI.Items.Buttons;

[ScriptPath("res://MapArtistCode/GUI/Items/NMapArtistResetButtonItem.cs")]
public partial class NMapArtistResetButtonItem : NMapArtistButtonItem
{
    public NMapArtistResetButtonItem()
    {
        Name = "MapArtistResetButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;
        
        ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset.png";
        GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_reset_glow.png";
    }

    public override void _Ready()
    {
        base._Ready();
        var locDesc = new LocString("static_hover_tips", "MAPARTIST-RESET_BUTTON.description");
        HoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-RESET_BUTTON.title"), locDesc);
        
        ConnectSignals();
    }
    
    protected override void OnPress()
    {
        base.OnPress();
        MapArtistController.MapArtistController.Instance.ResetSettings();
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
