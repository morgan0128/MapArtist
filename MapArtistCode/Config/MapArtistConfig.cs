using BaseLib.Config;

namespace MapArtist.MapArtistCode.Config;

[ConfigHoverTipsByDefault]
internal class MapArtistConfig : SimpleModConfig
{
    [ConfigSection("General Settings")]
    public static bool TopLeftGui { get; set; } = false;
    public static bool SynchronizedColorPicker { get; set; } = false;
    public static bool SynchronizedWidthSlider { get; set; } = false;
    public static bool ColorSamplerTool { get; set; } = false;
    
    [ConfigSection("Experimental Settings")]
    public static bool UseVanillaEraser { get; set; } = false;

}