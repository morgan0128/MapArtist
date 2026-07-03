using Godot;

namespace MapArtist.MapArtistCode.GUI.Items.Abstract;

public abstract partial class NMapArtistItem : HBoxContainer
{
    protected List<NMapArtistItem>? NestedItems;
    protected List<NMapArtistButton>? NestedButtons;
    protected List<Control>? NestedControlNodes;

    // protected NMapArtistItem()
    // {
    //     UniqueNameInOwner = true;
    //     SizeFlagsHorizontal = Control.SizeFlags.Fill;
    //     SizeFlagsVertical = Control.SizeFlags.Fill;
    //     MouseFilter = Control.MouseFilterEnum.Ignore;
    // }

    // public override void _EnterTree()
    // {
    //     if (NestedItems != null)
    //         foreach (var item in NestedItems)
    //         {
    //             AddChild(item);
    //         }
    //     if (NestedButtons != null)
    //     {
    //         foreach (var button in NestedButtons)
    //         {
    //             AddChild(button);
    //         }
    //     }
    //     if (NestedControlNodes != null)
    //     {
    //         foreach (var control in NestedControlNodes)
    //         {
    //             AddChild(control);
    //         }
    //     }
    // }
    

}