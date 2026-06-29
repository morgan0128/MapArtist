using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode.Patches;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.CleanUp))]
public static class MapArtistRunCleanupPatch
{
    private static void Postfix()
    {
        MapArtistController.MapArtistController.Instance.ResetRunState();
    }
}
