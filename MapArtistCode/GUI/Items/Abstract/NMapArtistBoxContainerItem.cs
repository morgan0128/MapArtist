using Godot;

namespace MapArtist.MapArtistCode.GUI.Items.Abstract;

public partial class NMapArtistBoxContainerItem : BoxContainer
{

    private List<Control>? _childItems; // Ideally, T would be an interface for "MapArtistItems", however, there is no
                                        // convenient way of using interfaces for set of classes that should all be Godot Nodes;
                                        // to my knowledge, Godot provides no Node interface or anything of the sort

    public NMapArtistBoxContainerItem()
    {
        Vertical = false; // Horizontal by default
    }
    
    public NMapArtistBoxContainerItem(bool isVBox)
    {
        Vertical = isVBox;
    }

    public void AddItem(NMapArtistBoxContainerItem item)
    {
        AddItem((Control)item);
    }
    
    public void AddItem(int index, NMapArtistBoxContainerItem item)
    {
        AddItem(index, (Control)item);
    }
    
    public void AddItem(NMapArtistButtonItem item)
    {
        AddItem((Control)item);
    }
    
    public void AddItem(int index, NMapArtistButtonItem item)
    {
        AddItem(index, (Control)item);
    }

    private void AddItem(Control item)
    {
        _childItems ??= [];
        _childItems.Add(item);
        AddChild(item);
    }
    
    private void AddItem(int index, Control item)
    {
        _childItems ??= [];
        if (index > _childItems.Count) return;
        if (index == _childItems.Count)
        {
            AddItem(item);
        }
        else
        {
            _childItems.Insert(index, item);
            if (index > 0)
            {
                _childItems[index - 1].AddSibling(item);
            }
            else
            {
                AddChild(item);
            }
        }

    }
    


    
}