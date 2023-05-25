namespace TNLPacketAnalyzer.CLI;

public class TNLConnection : GhostConnection
{
    public override void ReadPacket(OutputBuilder sb, Reader reader, bool isClient)
    {
        base.ReadPacket(sb, reader, isClient);
    }

    public override void ReadConnectRequest(OutputBuilder builder, Reader reader, bool isClient)
    {
        base.ReadConnectRequest(builder, reader, isClient);

        builder.AppendLine().AppendLine("TNLConnection::ReadConnectRequest").AppendLine("[").IncreaseDepth();

        builder.AppendLine($"Version: {reader.ReadInt(32)}");
        builder.AppendLine($"Key: {reader.ReadInt(32)}");
        builder.AppendLine($"Coid: {reader.ReadInt(64)}");

        builder.DecreaseDepth().AppendLine("]");
    }

    public override void OnEvent(long classId, OutputBuilder builder, Reader reader, bool isClient)
    {
        switch (classId)
        {
            case 3:
                AnalyzeRPCMsgGuaranteed(builder, reader);
                break;

            case 4:
                AnalyzeRPCMsgGuaranteedFragmented(builder, reader);
                break;

            case 5:
                AnalyzeRPCMsgGuaranteedOrdered(builder, reader);
                break;

            case 6:
                AnalyzeRPCMsgGuaranteedOrderedFragmented(builder, reader);
                break;

            case 7:
                AnalyzeRPCMsgNonGuaranteed(builder, reader);
                break;

            case 8:
                AnalyzeRPCMsgNonGuaranteedFragmented(builder, reader);
                break;

            default:
                base.OnEvent(classId, builder, reader, isClient);
                break;
        }
    }

    private void AnalyzeRPCMsgGuaranteed(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine($"Type: {reader.ReadInt(32)}");
        builder.Append("Buffer: ");

        ReadByteBuffer(builder, reader);
    }

    private void AnalyzeRPCMsgGuaranteedFragmented(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine($"Type: {reader.ReadInt(32)}");
        builder.AppendLine($"Fragment: {reader.ReadInt(16)}");
        builder.AppendLine($"FragmentId: {reader.ReadInt(16)}");
        builder.AppendLine($"FragmentCount: {reader.ReadInt(16)}");
        builder.Append("Buffer: ");

        ReadByteBuffer(builder, reader);
    }

    private void AnalyzeRPCMsgGuaranteedOrdered(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine($"Type: {reader.ReadInt(32)}");
        builder.Append("Buffer: ");

        ReadByteBuffer(builder, reader);
    }

    private void AnalyzeRPCMsgGuaranteedOrderedFragmented(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine($"Type: {reader.ReadInt(32)}");
        builder.AppendLine($"Fragment: {reader.ReadInt(16)}");
        builder.AppendLine($"FragmentId: {reader.ReadInt(16)}");
        builder.AppendLine($"FragmentCount: {reader.ReadInt(16)}");
        builder.Append("Buffer: ");

        ReadByteBuffer(builder, reader);
    }

    private void AnalyzeRPCMsgNonGuaranteed(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine($"Type: {reader.ReadInt(32)}");
        builder.Append("Buffer: ");

        ReadByteBuffer(builder, reader);
    }

    private void AnalyzeRPCMsgNonGuaranteedFragmented(OutputBuilder builder, Reader reader)
    {
        builder.AppendLine($"Type: {reader.ReadInt(32)}");
        builder.AppendLine($"Fragment: {reader.ReadInt(16)}");
        builder.AppendLine($"FragmentId: {reader.ReadInt(16)}");
        builder.AppendLine($"FragmentCount: {reader.ReadInt(16)}");
        builder.Append("Buffer: ");

        ReadByteBuffer(builder, reader);
    }
}
