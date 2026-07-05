using BaseLib;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode;

public static class Util
{
    public const int DefaultBrushWidth = 4;
    
    public static Player? GetLocalPlayer()
    {
        var currState = RunManager.Instance.DebugOnlyGetState();
        if (currState != null) return currState.GetPlayer(RunManager.Instance.NetService.NetId);
        BaseLibMain.Logger.Error("[MapArtistController] Failed to load current state");
        return null;
    }

    public static ulong GetLocalPlayerId()
    {
        return RunManager.Instance.NetService.NetId;
    }
    
    public static TextureRect DeepCopyIcon(TextureRect toCopy)
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

    public static TextureRect DeepCopyIcon(TextureRect toCopy, StringName imagePath)
    {
        var icon = DeepCopyIcon(toCopy);
        icon.Texture = ResourceLoader.Load<Texture2D>(imagePath);

        return icon;
    }
}