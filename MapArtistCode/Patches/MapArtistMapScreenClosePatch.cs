using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode.Patches;

[HarmonyPatch(typeof(NMapScreen), "AnimClose")]
public static class MapScreenClosePatch
{
    private static void Prefix()
    {
        // before this patch, gui gets hidden after other nodes tween hide animations finish. looks dissonant from
        // vanilla game.
        // this patch makes it look a bit smoother, but should ideally provide gui container with similar tween
        // animation in the future.
        
        MapArtistController.MapArtistController.Instance.HideGui();
    }
    
}