using System.Numerics;
using System.Reflection;
using Godot;
using HarmonyLib;
using MapArtist.MapArtistCode.Config;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode.Patches;

[HarmonyPatch(typeof(NMapDrawings))]
[HarmonyPatch("BeginLine")]
public class BeginLinePatch
{
    

    static Exception Finalizer(Exception __exception)
    {
        if (!MapArtistConfig.SuppressVanillaGameDrawingButtonsException) return __exception;
        
        switch (__exception)
        {
            case InvalidOperationException:
                /*
                 * 
                 * Bug in vanilla game: by clicking on the quill or erase button, then the clear draw button, then clicking anywhere on the screen:
                 * the vanilla code then throws away your click as result of throwing an exception in NMapDrawings.BeginLine().
                 *
                 * "MapArtistConfig.SuppressVanillaGameDrawingButtonsException"
                 * The user may toggle whether to suppress this exception or not in the Mod Configuration menu.
                 * The default configuration dictates that the exception be suppressed.
                 * 
                 */
                return null;
            default:
                return __exception;
        }
    }
    
    private static void Postfix(object[] __args)
    {
        var outerType = typeof(NMapDrawings);
        var nestedTypeDrawingState = outerType.GetNestedType("DrawingState", BindingFlags.NonPublic);
        var state = (object)__args[0];

        var dvp = (SubViewport)AccessTools.Field(nestedTypeDrawingState, "drawViewport").GetValue(state);
        var playerId = (ulong)AccessTools.Field(nestedTypeDrawingState, "playerId").GetValue(state);
        var line = (Line2D?)AccessTools.Field(nestedTypeDrawingState, "currentlyDrawingLine").GetValue(state);
        
        MapArtistDrawingHistories.Instance.CheckUpdateDrawViewports(playerId, dvp);
        MapArtistDrawingHistories.Instance.AddEntry(playerId, line);
        // var dvp = (SubViewport)AccessTools.Field(nestedTypeDrawingState, "drawViewport").GetValue(state);
        // MapArtistController.MapArtistController.Instance.TemporaryUpdateViewport(dvp);
    }
    
    
}