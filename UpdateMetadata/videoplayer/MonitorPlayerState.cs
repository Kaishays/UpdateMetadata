using KlvPlayer;
using StCoreWr;
using System.Threading.Tasks;

namespace UpdateMetadata.videoplayer
{
    public static class MonitorPlayerState
    {
        public static async Task<bool> Manage()
        {
            if (StoreVidPlayer.m_KlvPlayer != null)
            {
                bool playerStateComplete 
                    = await MonitorVideoPlayerState(StoreVidPlayer.m_KlvPlayer);
                return playerStateComplete;
            } else
            {
                return false;
            }
        }
        public static async Task<bool> MonitorVideoPlayerState(CKlvPlayer cKlvPlayer)
        {
            while (cKlvPlayer.PlayerState != Player_State.Running && cKlvPlayer.PlayerState != Player_State.Completed)
            {
                await Task.Delay(RuntimeVariables.delayMonitorVideoPlayerState);
                if (HandleCorruptedVids
                    .CheckPlayerStateError(cKlvPlayer.PlayerState))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
