// using Godot;
// using HarmonyLib;
// using MegaCrit.Sts2.Core.Entities.Players;
// using MegaCrit.Sts2.Core.Nodes.Screens.Map;
// using System.Reflection;
//
// namespace MapArtist.MapArtistCode.Patches;
//
// [HarmonyPatch(typeof(NMapDrawings), "BeginLine", new Type[] { typeof(Object), typeof(Vector2), typeof(DrawingMode?) })]
// public class MapArtistDrawingHistoryPatch
// {
//
//     
//     private static void Postfix(Object state, Vector2 position, DrawingMode? overrideDrawingMode)
//     {
//         var outerType = typeof(NMapDrawings);
//         var nestedTypeDrawingState = outerType.GetNestedType("DrawingState", BindingFlags.NonPublic);
//
//         var dvp = (SubViewport)AccessTools.DeclaredField(nestedTypeDrawingState, "_drawViewport").GetValue(state);
//     }
//
// }