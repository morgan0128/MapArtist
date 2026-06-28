using BaseLib;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace MapArtist.MapArtistCode.GUI.Items;

[ScriptPath("res://MapArtistCode/GUI/Items/NMapArtistApplyButton.cs")]
public partial class NMapArtistApplyButton : GUI.Items.Abstract.NMapArtistButton
{
    private static readonly StringName ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply.png";
    private static readonly StringName GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply_glow.png";
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");
    
    public NMapArtistApplyButton()
    {
        Name = "MapArtistApplyButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;
    }
    
    public override void _Ready()
    {
        base._Ready();
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-APPLY_BUTTON.description");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-APPLY_BUTTON.title"), locDesc);
        
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
