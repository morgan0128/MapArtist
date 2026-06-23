using System.Numerics;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MapArtist.MapArtistCode.Patches;

[HarmonyPatch(typeof(NMapDrawings))]
[HarmonyPatch("BeginLine")]
public class CommonDrawingModeExceptionPatch
{
    
    // temporary patch to suppress exception, to prevent users from experiencing BaseLib log window crash.
    // to be removed upon BaseLib patching issue.
    static Exception Finalizer(Exception __exception)
    {
        switch (__exception)
        {
            case InvalidOperationException:
                /*
                 * 
                 * Bug in vanilla game: by clicking on the quill or erase button, then the clear draw button, then clicking anywhere on the screen:
                 * the vanilla code then throws away your click as result of throwing an exception in NMapDrawings.BeginLine().
                 *
                 * In BaseLib's latest release, there seemingly is a bug related to flooding the log window with
                 * exceptions. By using the map drawing feature a lot, as I expect one may do with this mod,
                 * this BaseLib bug may eventually result in the game crashing if the log window is open.
                 *
                 * Simply suppressing this exception as is done here seems to solve this particular issue and not
                 * reveal any differences in the vanilla game's implementations, but need more time to test and
                 * understand Harmony exception suppression.
                 * 
                 */
                return null;
            default:
                return __exception;
        }

    }
    
    
}