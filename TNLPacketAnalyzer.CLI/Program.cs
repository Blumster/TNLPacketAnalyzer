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

                sw.WriteLine($"Processing file: {file}");
                sw.WriteLine();

                var lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    if (line.Length == 0)
                        continue;

                    var arr = new byte[(line.Length + 1) / 3];
                    var split = line.Split(new[] { '-' });

                    for (var i = 0; i < arr.Length; ++i)
                        arr[i] = byte.Parse(split[i], NumberStyles.AllowHexSpecifier);

                    sw.WriteLine(new Analyzer(arr).Run());
                }
            }
        }
    }
}
