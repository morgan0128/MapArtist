using System.ComponentModel;
using Godot;
using Godot.NativeInterop;
using HarmonyLib;
using MapArtist.MapArtistCode.Config;
using MapArtist.MapArtistCode.MapArtistController;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode.Patches;

[HarmonyPatch(typeof(NMapDrawings), "CreateLineForPlayer", new Type[] { typeof(Player), typeof(bool) })]
public static class MapArtistDrawingPatch
{
    
    // test
    // private static readonly StringName Image1Path = "res://images/relics/bing_bong.png";
    private static readonly StringName TestImagePath = "res://MapArtist/Images/CustomIcons/mapartist_apply.png";
    private static readonly StringName Image1Path = "res://MapArtist/Images/Textures/texture6.png";
    
    private static Texture2D? _hardEraserTexture;
    
    private static Texture2D? _customDrawingTexture1;

    private static void Postfix(Player player, bool isErasing, Line2D __result)
    {
        // NMapColorPickerButton.SetOwner __result.GetOwner();
        if (__result == null)
        {
            return;
        }

        if (isErasing)
        {
            if (MapArtistConfig.UseVanillaEraser)
            {
                return;
            }
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
        
        // test
        // var g = new Gradient();
        // __result.SetGradient(g);
        __result.Texture = GetCustomDrawingTexture1();
        __result.TextureRepeat = CanvasItem.TextureRepeatEnum.Mirror;
        __result.TextureMode = Line2D.LineTextureMode.Tile;
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
    
    private static Texture2D GetCustomDrawingTexture1()
    {
        if (_customDrawingTexture1 != null)
        {
            return _customDrawingTexture1;
        }
        
        // var t2d = new Texture2D();
        // var tr = new TextureRect();
        // var t = ResourceLoader.Load<Texture2D>(Image1Path);
        // tr.Texture = t;
        // tr.SetExpandMode(TextureRect.ExpandModeEnum.FitHeightProportional);
        // tr.SetStretchMode(TextureRect.StretchModeEnum.Tile);
        // var r2 = new Rect2();
        // r2.Size = new Vector2(2f, 2f);
        // var rid = GetHardEraserTexture()._GetRid();
        // tr.Texture = t;
        // t.DrawRect(rid, r2, true, null, false);
        // _customDrawingTexture1 = ImageTexture.CreateFromImage(image1);
        // return _customDrawingTexture1;
        // t.Set(Texture2D.);
        
        // var image = Image.CreateEmpty(4, 2, false, Image.Format.Rgba8);
        // var image = Image.CreateFromData(4, 4, false, Image.Format.Rgba8, new ReadOnlySpan<byte>());
        // image.Fill(Colors.White);
        // var image2 = Image.LoadFromFile(Image1Path);
        // image.BlendRectMask(image, image2, image.GetUsedRect(), new Vector2I(0, 0));
        // _customDrawingTexture1 = ImageTexture.CreateFromImage(image);
        
        
        
        
        var t = ResourceLoader.Load<Texture2D>(Image1Path);
        // var t_img = t.GetImage();
        // t_img.Crop(4, 2);
        // _customDrawingTexture1 = ImageTexture.CreateFromImage(t_img);
        _customDrawingTexture1 = t;
        
        return _customDrawingTexture1;
    }
}
