using BaseLib.Config;

namespace MapArtist.MapArtistCode.Config;

[ConfigHoverTipsByDefault]
internal class MapArtistConfig : SimpleModConfig
{
    [ConfigSection("General Settings")]
    public static bool TopLeftGui { get; set; } = false; // class responsible: director
    public static bool SynchronizedColorPicker { get; set; } = true; // class responsible: controller
    public static bool SynchronizedWidthSlider { get; set; } = true; // class responsible: controller
    public static bool ColorSamplerTool { get; set; } = true; // class responsible: director
    
    [ConfigSection("Experimental Settings")]
    public static bool UseVanillaEraser { get; set; } = false; // class responsible: MapArtistDrawingPatch (CreateLineForPlayer patch)

    public static bool SuppressVanillaGameDrawingButtonsException { get; set; } = true; // class responsible: BeginLinePatch

}