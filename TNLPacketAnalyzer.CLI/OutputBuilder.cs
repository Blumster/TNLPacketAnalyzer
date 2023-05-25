using System.Text;

namespace TNLPacketAnalyzer.CLI;

public class OutputBuilder
{
    public const int DepthSize = 2;

    public StringBuilder Builder { get; } = new();
    public int Depth { get; set; } = 0;

    public OutputBuilder IncreaseDepth(int level = 1)
    {
        Depth += level * DepthSize;

        return this;
    }

    public OutputBuilder DecreaseDepth(int level = 1)
    {
        Depth -= level * DepthSize;

        return this;
    }

    public OutputBuilder Append(object obj)
    {
        AppendDepth();

        Builder.Append(obj);

        return this;
    }

    public OutputBuilder AppendLine()
    {
        Builder.AppendLine();

        return this;
    }

    public OutputBuilder AppendLine(string obj)
    {
        AppendDepth();

        Builder.AppendLine(obj);

        return this;
    }

    private void AppendDepth()
    {
        Builder.Append(' ', Depth);
    }

    public override string ToString() => Builder.ToString();
}
