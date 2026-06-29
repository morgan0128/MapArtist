using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace MapArtist.MapArtistCode.Multiplayer;

public sealed class MapArtistBrushSettingsRequestMessage : ICustomMessage
{
    public bool ShouldBroadcast => true;

    public void HandleMessage(ulong senderId)
    {
        MapArtistController.MapArtistController.Instance.BroadcastCurrentSettings(true);
    }

    public void Serialize(PacketWriter writer)
    {
    }

    public void Deserialize(PacketReader reader)
    {
    }
}
