namespace TNLPacketAnalyzer.CLI
{
    public static class Const
    {
        public static readonly string[] ClassNames =
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

        public static readonly string[] NetPacketType =
        {
            "DataPacket",
            "PingPacket",
            "AckPacket",
            "InvalidPacketType"
        };

        public static readonly string[] PacketType =
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
