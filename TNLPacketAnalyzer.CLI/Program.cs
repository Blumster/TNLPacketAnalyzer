using System;
using System.Globalization;
using System.IO;

namespace TNLPacketAnalyzer.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: TNLPacketAnalyzer.CLI.exe <file path> [array of more files]");
                return;
            }

            using var sw = new StreamWriter(@"output.txt", false);

            foreach (var file in args)
            {
                if (!File.Exists(file))
                {
                    sw.WriteLine("You must specify an existing file!");
                    return;
                }

                var netInterface = new NetInterface();

                sw.WriteLine($"Processing file: {file}");
                sw.WriteLine();

                string prefix = string.Empty;
                string ip = string.Empty;

                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (line.Length == 0)
                        continue;

                    if (line.StartsWith("RECV") || line.StartsWith("SEND"))
                    {
                        ip = line.Split(" - ")[2];
                        prefix = line[0..4];
                        sw.WriteLine(line);
                        continue;
                    }

                    var arr = new byte[(line.Length + 1) / 3];
                    var split = line.Split(new[] { '-' });

                    for (var i = 0; i < arr.Length; ++i)
                        arr[i] = byte.Parse(split[i], NumberStyles.AllowHexSpecifier);

                    sw.WriteLine(netInterface.ProcessPacket(arr, prefix == "RECV", ip));
                }
            }
        }
    }
}
