using System;

namespace TNLPacketAnalyzer.RUN
{
    public static class Const
    {
        public static String[] ClassNames =
        {
            "RPCMsgEndGhosting",
            "RPCMsgReadyForNormalGhosts",
            "RPCMsgStartGhosting",
            "RPCMsgGuaranteed",
            "RPCMsgGuaranteedFragmented",
            "RPCMsgGuaranteedOrdered",
            "RPCMsgGuaranteedOrderedFragmented",
            "RPCMsgNonGuaranteed",
            "RPCMsgNonGuaranteedFragmented"
        };

        public static String[] NetPacketType =
        {
            "DataPacket",
            "PingPacket",
            "AckPacket",
            "InvalidPacketType"
        };

        public static String[] PacketType =
        {
            "ConnectChallengeRequest",
            "ConnectChallengeResponse",
            "ConnectRequest",
            "ConnectReject",
            "ConnectAccept",
            "Disconnect",
            "Punch",
            "ArrangedConnectRequest",
            "FirstValidInfoPacketId"
        };
    }
}
