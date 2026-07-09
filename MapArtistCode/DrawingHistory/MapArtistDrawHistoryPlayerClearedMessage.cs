// using BaseLib;
// using BaseLib.Abstracts;
// using MegaCrit.Sts2.Core.Logging;
// using MegaCrit.Sts2.Core.Multiplayer.Serialization;
// using MegaCrit.Sts2.Core.Multiplayer.Transport;
//
// namespace MapArtist.MapArtistCode.DrawingHistory;
//
// public sealed class MapArtistDrawHistoryPlayerClearedMessage : ICustomMessage
// {
//     public bool ShouldBroadcast => true;
//
//     private readonly bool _calledFromRedo;
//
//     public MapArtistDrawHistoryPlayerClearedMessage()
//     {
//     }
//
//     public MapArtistDrawHistoryPlayerClearedMessage(bool calledFromRedo = false)
//     {
//         _calledFromRedo = calledFromRedo;
//     }
//
//     public NetTransferMode Mode => NetTransferMode.Reliable;
//
//     public LogLevel LogLevel => LogLevel.VeryDebug;
//
//     public bool ShouldBuffer => true;
//
//     public void Serialize(PacketWriter writer)
//     {
//     }
//
//     public void Deserialize(PacketReader reader)
//     {
//     }
//
//     public void HandleMessage(ulong playerId)
//     {
//         BaseLibMain.Logger.Info("Received message.\n");
//         // BaseLibMain.Logger.Info("Message has null DrawViewport? " + (DrawViewport == null) + "\n");
//         // DrawViewport?.RemoveChildSafely(Line);
//
//         MapArtistLocalDrawingHistory.Instance.NetPlayerOperationCleared(playerId, _calledFromRedo);
//
//     }
//
// }