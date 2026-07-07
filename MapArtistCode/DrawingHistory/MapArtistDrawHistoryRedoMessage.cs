using BaseLib;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace MapArtist.MapArtistCode.DrawingHistory;

public sealed class MapArtistDrawHistoryRedoMessage : ICustomMessage
{
    public bool ShouldBroadcast => true;

    // public SubViewport? DrawViewport;
    //
    // public Line2D? Line;

    // public MapArtistDrawHistoryUndoMessage(SubViewport drawViewport, Line2D line)
    // {
    //     DrawViewport = drawViewport;
    //     Line = line;
    // }

    public MapArtistDrawHistoryRedoMessage()
    {
    }

    public NetTransferMode Mode => NetTransferMode.Reliable;

    public LogLevel LogLevel => LogLevel.VeryDebug;

    public bool ShouldBuffer => true;

    public void Serialize(PacketWriter writer)
    {
    }

    public void Deserialize(PacketReader reader)
    {
    }

    public void HandleMessage(ulong playerId)
    {
        BaseLibMain.Logger.Info("Received message.\n");
        // BaseLibMain.Logger.Info("Message has null DrawViewport? " + (DrawViewport == null) + "\n");
        // DrawViewport?.RemoveChildSafely(Line);
        
        MapArtistLocalDrawingHistory.Instance.Redo(playerId);
        
    }

}