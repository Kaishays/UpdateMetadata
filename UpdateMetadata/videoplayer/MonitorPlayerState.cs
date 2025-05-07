using KlvPlayer;
using StCoreWr;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdateMetadata.videoplayer
{
    public static class MonitorPlayerState
    {
        public static async Task<bool> Manage()
        {
            try
            {
                if (StoreVidPlayer.m_KlvPlayer != null)
                {
                    bool playerStateComplete
                        = await MonitorVideoPlayerState(StoreVidPlayer.m_KlvPlayer);
                    return playerStateComplete;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public static async Task<bool> MonitorVideoPlayerState(CKlvPlayer cKlvPlayer)
        {
            while (ForDurationExtractor_CheckPlayerState(cKlvPlayer.PlayerState))
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
        private static bool ForDurationExtractor_CheckPlayerState(Player_State playerstate)
        {
            return (playerstate != Player_State.Completed
                && playerstate != Player_State.Running);
        }
    }
}
