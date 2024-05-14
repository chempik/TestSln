using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ConfigWrapper;

namespace Client
{
    public class Program
    {
        private static readonly List<int> _quotes = new List<int>();
        private static readonly object _lockObject = new object();
        private static int _expectedSequenceNumber = 0;
        private static int _lostPacketCount = 0;

        static void Main(string[] args)
        {
            Wrapper config = new Wrapper().XmlDeserializer();

            UdpClient udpClient = new UdpClient(config.MulticastPort);
            udpClient.JoinMulticastGroup(IPAddress.Parse(config.MulticastAddress));

            Thread receiveThread = new Thread(() => ReceiveData(udpClient));
            receiveThread.Start();

            while (true)
            {
                Console.WriteLine("Press Enter to display statistics...");
                Console.ReadLine();

                DisplayStatistics();
            }
        }

        public static void ReceiveData(UdpClient client)
        {
            while (true)
            {
                IPEndPoint remoteEndPoint = null;
                byte[] receivedData = client.Receive(ref remoteEndPoint);

                // We get the serial number from the data
                int sequenceNumber = BitConverter.ToInt32(receivedData, receivedData.Length - sizeof(int));
                byte[] quoteBytes = new byte[receivedData.Length - sizeof(int)];
                Array.Copy(receivedData, 0, quoteBytes, 0, quoteBytes.Length);
                int quote = BitConverter.ToInt32(quoteBytes, 0);

                if (sequenceNumber == _expectedSequenceNumber)
                {
                    lock (_lockObject)
                    {
                        _quotes.Add(quote);
                    }

                    _expectedSequenceNumber = (_expectedSequenceNumber + 1) % 1000;
                }
                else
                {
                    _lostPacketCount++;
                }
            }
        }

        public static void DisplayStatistics()
        {
            lock (_lockObject)
            {
                if (_quotes.Count == 0)
                {
                    Console.WriteLine("No quotes received yet.");
                    return;
                }

                double average = _quotes.Average();
                double standardDeviation = CalculateStandardDeviation(_quotes);
                int mode = _quotes.GroupBy(n => n).OrderByDescending(g => g.Count()).First().Key;
                int median = CalculateMedian(_quotes);

                Console.WriteLine($"Average: {average}");
                Console.WriteLine($"Standard Deviation: {standardDeviation}");
                Console.WriteLine($"Mode: {mode}");
                Console.WriteLine($"Median: {median}");
                Console.WriteLine($"Total quotes received: {_quotes.Count}");
                Console.WriteLine($"Lost packet count: {_lostPacketCount}");
            }
        }

        public static double CalculateStandardDeviation(List<int> data)
        {
            double mean = data.Average();
            double sumOfSquares = data.Sum(num => Math.Pow(num - mean, 2));
            double variance = sumOfSquares / data.Count;
            return Math.Sqrt(variance);
        }

        public static int CalculateMedian(List<int> data)
        {
            List<int> sortedData = data.OrderBy(n => n).ToList();
            int size = sortedData.Count;
            int mid = size / 2;
            return (size % 2 != 0) ? sortedData[mid] : (sortedData[mid - 1] + sortedData[mid]) / 2;
        }
    }
}
