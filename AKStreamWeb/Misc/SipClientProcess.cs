using LibCommon.Structs;

namespace AKStreamWeb.Misc
{
    public static class SipClientProcess
    {
        public static bool InviteChannel(ShareInviteInfo info)
        {
            var obj=Common.ShareInviteChannels.FindLast(x => x.Deviceid.Equals(info.ChannelId) && x.Ssrc.Equals(info.Ssrc));
            if (obj != null)
            {
                return false;
            }
            else
            {
                return true;  
            }
            
        }
    }
}