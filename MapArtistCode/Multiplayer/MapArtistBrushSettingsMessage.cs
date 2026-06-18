using BaseLib.Abstracts;
using Godot;
using MapArtist.MapArtistCode.MapArtistController;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace MapArtist.MapArtistCode.Multiplayer;

public sealed class MapArtistBrushSettingsMessage : ICustomMessage
{
    public bool IsReset { get; private set; }
    public Color Color { get; private set; }
    public float Width { get; private set; }

    public bool ShouldBroadcast => true;

    public MapArtistBrushSettingsMessage()
    {
    }

    public MapArtistBrushSettingsMessage(Color color, float width)
    {
        Color = color;
        Width = width;
    }

    public static MapArtistBrushSettingsMessage Reset()
    {
        return new MapArtistBrushSettingsMessage
        {
            IsReset = true
        };
    }

    public void HandleMessage(ulong senderId)
    {
        if (IsReset)
        {
            MapArtistDictionaries.ClearAll(senderId);
            return;
        }

        MapArtistDictionaries.SetColor(senderId, Color);
        MapArtistDictionaries.SetPenWidth(senderId, Width);
    }

    public void Serialize(PacketWriter writer)
    {
        writer.WriteBool(IsReset);
        writer.WriteFloat(Color.R);
        writer.WriteFloat(Color.G);
        writer.WriteFloat(Color.B);
        writer.WriteFloat(Color.A);
        writer.WriteFloat(Width);
    }

    public void Deserialize(PacketReader reader)
    {
        IsReset = reader.ReadBool();
        Color = new Color(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
        Width = reader.ReadFloat();
    }
}
