using Godot;
using MapArtist.MapArtistCode.GUI.Items.Abstract;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace MapArtist.MapArtistCode.GUI.Items.Buttons;

[ScriptPath("res://MapArtistCode/GUI/Items/NMapArtistUndoButtonItem.cs")]
public partial class NMapArtistUndoButtonItem : NMapArtistButtonItem
{
    public NMapArtistUndoButtonItem()
    {
        Name = "MapArtistUndoButton";
        UniqueNameInOwner = true;
        CustomMinimumSize = new Vector2(35f, 35f);
        LayoutMode = 2;
        FocusMode = FocusModeEnum.All;
        
        ImagePath = "res://MapArtist/Images/CustomIcons/mapartist_undo1.png";
        GlowImagePath = "res://MapArtist/Images/CustomIcons/mapartist_undo1_glow.png";
    }
    
    public override void _Ready()
    {
        base._Ready();
        LocString locDesc = new LocString("static_hover_tips", "MAPARTIST-UNDO_BUTTON.description");
        HoverTip = new HoverTip(new LocString("static_hover_tips", "MAPARTIST-UNDO_BUTTON.title"), locDesc);
        
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
