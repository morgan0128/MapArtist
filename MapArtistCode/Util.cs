using BaseLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;

namespace MapArtist.MapArtistCode;

public static class Util
{
    public static Player? GetLocalPlayer()
    {
        var currState = RunManager.Instance.DebugOnlyGetState();
        if (currState != null) return currState.GetPlayer(RunManager.Instance.NetService.NetId);
        BaseLibMain.Logger.Error("[MapArtistController] Failed to load current state");
        return null;
    }
}