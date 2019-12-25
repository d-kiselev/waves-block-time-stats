using System;
using System.Collections.Generic;
using System.Linq;
using WavesCS;

namespace wavesblocktimestats
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Node node = new Node(Node.MainNetChainId);

            var startHeight = 1818967;
            var endHeight = node.GetHeight();

            Console.WriteLine($"start height = {startHeight}");
            Console.WriteLine($"end height = {endHeight}");

            var headers = new List<Dictionary<string, object>>();

            const int maxSequenceLength = 100;

            for (var h = startHeight; h <= endHeight; h += maxSequenceLength)
            {
                Console.WriteLine(h);

                var sequenceFirstBlock = h;
                var sequenceLastBlock = Math.Min(h + maxSequenceLength - 1, endHeight);
                headers.AddRange(node.GetHeadersSequence(sequenceFirstBlock, sequenceLastBlock));
            }

            var timestamps = headers.Select(header => header.GetLong("timestamp")).ToList();

            var delays = new List<long>();
            
            for (int i = 0; i < timestamps.Count() - 1; i++)
            {
                delays.Add(timestamps[i + 1] - timestamps[i]);
            }

            /*

            var histogram = new List<int>();
            var step = 1000;

            for (var i = delays.Min(); i <= delays.Max(); i += step)
            {
                var v = delays.Count(x => x >= i && x < i + step);
                histogram.Add(v);
            }

            */

            delays.Sort();

            for (int i = 1; i < 10; i++)
            {
                var index = delays.Count / 10 * i;
                Console.WriteLine($"{i * 10}%\t\t{(delays[index]) / 1000.0} sec");
            }
        }
    }

    static class NodeExtentions
    {
        public static List<Dictionary<string, object>> GetHeadersSequence(this Node node, long from, long to)
        {
            return node.GetObjects($"blocks/headers/seq/{from}/{to}").ToList();
        }
    }
}
