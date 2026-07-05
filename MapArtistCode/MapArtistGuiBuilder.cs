// using MapArtist.MapArtistCode.Config;
// using MapArtist.MapArtistCode.GUI;
// using MapArtist.MapArtistCode.GUI.Items;
// using MapArtist.MapArtistCode.GUI.Items.Abstract;
//
// namespace MapArtist.MapArtistCode;
//
// public class MapArtistGuiBuilder : MapArtistBuilder
// {
//     private ConfigParameters _configuration;
//
//     private int _rowsBuilt = 0;
//
//     private readonly struct ConfigParameters
//     {
//         public enum ColorPickerPolicy
//         {
//             Single,
//             Multiple,
//             None
//         }
//         
//         public ConfigParameters()
//         {
//         }
//
//         public readonly bool TopLeft = MapArtistConfig.TopLeftGui;
//         public readonly bool AllowDuplicateButtons = false;
//         public readonly ColorPickerPolicy PolicyColorPicker = ColorPickerPolicy.Single;
//         public readonly int ColorPickerRow => (PolicyColorPicker == ColorPickerPolicy.Single) ? (TopLeft ? 0 : 1) : -1;
//         public readonly bool IncludeColorPickerSampler => (PolicyColorPicker != ColorPickerPolicy.None && MapArtistConfig.ColorSamplerTool);
//
//         public readonly bool BuildApplyReset = true;
//         public readonly bool BuildUndoRedo = true;
//         public readonly bool BuildItemWidth = true;
//
//     }
//         
//     public NMapArtistGui Build()
//     {
//         Reset();
//         var product = new NMapArtistGui();
//         var buildingRow = 0;
//         
//         product.Rows.Add(BuildRow(buildingRow));
//
//         return product;
//     }
//
//     private void Reset()
//     {
//         _configuration = new ConfigParameters();
//         _rowsBuilt = 0;
//     }
//
//     private NMapArtistItem BuildUtilityButtonsItem()
//     {
//         if (_configuration.ColorPickerRow == row)
//         {
//             var colorPicker = BuildColorPicker();
//         }
//         
//     }
//
//     private NMapArtistItem BuildApplyResetItem()
//     {
//         
//     }
//     
//     private NMapArtistItem BuildWidthItem
//
//     private NMapArtistItem BuildUndoRedoItem()
//     {
//         
//     }
//
//     private NColorPickerItem BuildColorPicker()
//     {
//         return new NColorPickerItem();
//     }
// }