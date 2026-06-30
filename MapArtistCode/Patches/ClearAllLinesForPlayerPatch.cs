using System.Reflection;
using Godot;
using HarmonyLib;
using MapArtist.MapArtistCode.Config;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode.Patches;

[HarmonyPatch(typeof(NMapDrawings))]
[HarmonyPatch("ClearAllLinesForPlayer")]
public class ClearAllLinesForPlayerPatch
{
    // Patch for preserving player's cleared lines in their respective MapArtistDrawingHistory
    private static void Prefix(object[] __args)
    {
        var outerType = typeof(NMapDrawings);
        var nestedTypeDrawingState = outerType.GetNestedType("DrawingState", BindingFlags.NonPublic);
        var state = (object)__args[0];
        
        var playerId = (ulong?)AccessTools.Field(nestedTypeDrawingState, "playerId").GetValue(state);
        var dvp = (SubViewport?)AccessTools.Field(nestedTypeDrawingState, "drawViewport").GetValue(state);
        if (playerId == null || dvp == null) return;

        var id = (ulong)playerId; // explicit conversion in C#; no data lost
        
        var linesToClear = dvp.GetChildren().OfType<Line2D>().ToList();
        var linesSaved = new List<Line2D>();
        foreach (var line in linesToClear)
        {
            linesSaved.Add((Line2D)line.Duplicate());
        }
        
        MapArtistDrawingHistory.Instance.NotifyPlayerCleared(id, dvp, linesSaved);
    }
    
}