namespace TNLPacketAnalyzer.CLI;

public class NetConnection
{
    public long Nonce { get; set; }
    public bool DebugObjectSizes { get; set; }
    public string IP { get; set; }

    public virtual void ReadPacket(OutputBuilder builder, Reader reader, bool isClient)
    {
    }

    public virtual void ReadConnectRequest(OutputBuilder builder, Reader reader, bool isClient)
    {
        builder.AppendLine().AppendLine("NetConnection::ReadConnectRequest").AppendLine("[").IncreaseDepth();
        builder.AppendLine($"Net Class Group: {reader.ReadInt(32)}");
        builder.AppendLine($"Net Class Group CRC: {reader.ReadInt(32):X8}");
        builder.DecreaseDepth().AppendLine("]");
    }

    public virtual void ReadConnectAccept(OutputBuilder builder, Reader reader, bool isClient)
    {
    }
}
