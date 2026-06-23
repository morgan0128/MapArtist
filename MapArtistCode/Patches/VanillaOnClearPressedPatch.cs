// not yet figured out what causes game to handle input, after draw/erase -> clear, as a drawing input.
// the below does not resolve the issue but may be close.
//
// using HarmonyLib;
// using MegaCrit.Sts2.Core.Models.Events;
// using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
// using MegaCrit.Sts2.Core.Nodes.Screens.Map;
//
// namespace MapArtist.MapArtistCode.Patches;
//
// [HarmonyPatch(typeof(NMapScreen), "OnClearMapDrawingButtonPressed", new Type[] { typeof(NButton) })]
// public class VanillaOnClearPressedPatch
// {
//     private static void Postfix(NButton _, NMapScreen __instance)
//     {
//         var drawings = (NMapDrawings)__instance.Drawings;
//         drawings.StopLineLocal();
//     }
// }