using Godot;

namespace MapArtist.MapArtistCode.GUI.Items.Abstract;

public partial class NMapArtistBoxContainerItem : BoxContainer
{
    protected List<Control>? ChildItems; // Ideally, T would be an interface for "MapArtistItems", however, there is no
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

    protected void AddItem(Control item)
    {
        ChildItems ??= [];
        ChildItems.Add(item);
        AddChild(item);
    }
    
    protected void AddItem(int index, Control item)
    {
        ChildItems ??= [];
        if (index > ChildItems.Count) return;
        if (index == ChildItems.Count)
        {
            AddItem(item);
        }
        else
        {
            ChildItems.Insert(index, item);
            if (index > 0)
            {
                ChildItems[index - 1].AddSibling(item);
            }
            else
            {
                AddChild(item);
            }
        }

    }
    
    protected Control? FetchFirstItemByType(Type t)
    {
        if (ChildItems == null) return null;
        foreach (var ctrl in ChildItems)
        {
            if (ctrl.GetType() == t)
            {
                return ctrl;
            } else if (ctrl.GetType() == typeof(NMapArtistBoxContainerItem))
            {
                var box = (NMapArtistBoxContainerItem)ctrl;
                var result = box.FetchFirstItemByType(t);
                if (result != null) return result;
            }
        }
    
        return null;
    }
    


    
}