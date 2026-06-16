using Godot;
using HarmonyLib;
using MapArtist.MapArtistCode.MapArtistController;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode.Patches;

[HarmonyPatch(typeof(NMapDrawings), "CreateLineForPlayer", new Type[] { typeof(Player), typeof(bool) })]
public static class MapArtistDrawingPatch
{
    private static Texture2D? _hardEraserTexture;

    private static void Postfix(Player player, bool isErasing, Line2D __result)
    {
        // NMapColorPickerButton.SetOwner __result.GetOwner();
        if (__result == null)
        {
            return;
        }

        if (isErasing)
        {
            __result.DefaultColor = Colors.White;
            __result.Texture = GetHardEraserTexture();
            return;
        }

        if (MapArtistDictionaries.TryGetColor(player, out var color))
        {
            __result.DefaultColor = color;
        }

        if (MapArtistDictionaries.TryGetPenWidth(player, out var width))
        {
            __result.Width = width;
        }

    }

    private static Texture2D GetHardEraserTexture()
    {
        if (_hardEraserTexture != null)
        {
            return _hardEraserTexture;
        }

        var image = Image.CreateEmpty(2, 2, false, Image.Format.Rgba8);
        image.Fill(Colors.White);
        _hardEraserTexture = ImageTexture.CreateFromImage(image);
        return _hardEraserTexture;
    }
}
