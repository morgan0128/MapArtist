using Godot;
using MapArtist.MapArtistCode.GUI.Items.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace MapArtist.MapArtistCode.GUI.Items.Buttons;

[ScriptPath("res://MapArtistCode/GUI/Items/NMapArtistUndoButtonItem.cs")]
public partial class NMapArtistUndoButtonItem : NMapArtistButtonItem
{
    // private static readonly StringName ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_undo.png";
    // private static readonly StringName GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_undo_glow.png";
    private static readonly Color ActiveColor = new Color("FFE57DFF");
    private static readonly Color InactiveColor = new Color("FFFFFF80");
    
    public NMapArtistUndoButtonItem()
    {
        Name = "MapArtistUndoButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;
        
        ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_undo.png";
        GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_undo_glow.png";
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
        // test
        // MapArtistController.MapArtistController.Instance.ApplySettings();
        MapArtistController.MapArtistController.Instance.LocalDrawingHistoryUndo();
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
