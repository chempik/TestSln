using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ConfigWrapper;

namespace Server
{
    internal class Program
    {
        private static int maxSequenceNumber = 1000;
        private static int currentSequenceNumber = 0;

        static void Main(string[] args)
        {
            Wrapper config = new Wrapper().XmlDeserializer();

            UdpClient udpClient = new UdpClient();
            udpClient.JoinMulticastGroup(IPAddress.Parse(config.MulticastAddress));

            Random random = new Random();

            while (true)
            {
                int randomNumber = random.Next(config.MinValue, config.MaxValue);
                byte[] data = BitConverter.GetBytes(randomNumber);

                byte[] sequenceNumberBytes = BitConverter.GetBytes(currentSequenceNumber);
                byte[] combinedData = new byte[data.Length + sequenceNumberBytes.Length];
                Array.Copy(data, 0, combinedData, 0, data.Length);
                Array.Copy(sequenceNumberBytes, 0, combinedData, data.Length, sequenceNumberBytes.Length);

                udpClient.Send(combinedData, combinedData.Length, config.MulticastAddress, config.MulticastPort);

                currentSequenceNumber = (currentSequenceNumber + 1) % maxSequenceNumber;

            }
        }
    }
}
