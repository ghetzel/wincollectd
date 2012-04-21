using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace wincollectd
{
    public struct ValuePacket
    {
        public byte datatype;
        public UInt64 value;
    };

    class PacketWriter
    {
        private static PacketWriter _instance;

        private string _host = null;
        private ushort _port = Properties.Settings.Default.Port;
        private UdpClient _client;
        private MemoryStream _buffer = new MemoryStream(Properties.Settings.Default.MaxPacketSize);
        private StreamWriter _writer;
        private List<Counter> _counters = new List<Counter>();

        public PacketWriter() { }

        public PacketWriter(string host)
        {
            SetHost(host);
        }

        public PacketWriter(string host, ushort port)
        {
            SetHost(host, port);
        }

        public static PacketWriter instance()
        {
            if (_instance == null)
                _instance = new PacketWriter();

            return _instance;
        }

        public void SetHost(string host)
        {
            _host = host;
            init();
        }

        public void SetHost(string host, ushort port)
        {
            _host = host;
            _port = port;
            init();
        }

        public void SendData()
        {
            pushPacketHeader();

            foreach (Counter c in _counters)
                pushCounter(c);


            byte[] payload = _buffer.ToArray();
            _client.Send(payload, payload.Length);
            _buffer = new MemoryStream(Properties.Settings.Default.MaxPacketSize);
        }

        private void init()
        {
            _writer = new StreamWriter(_buffer);

            if (_host != null && _port > 0)
            {
                _client = new UdpClient(_host, _port);
            }
        }

        public void addCounter(Counter counter)
        {
            _counters.Add(counter);
        }

        public void pushCounter(Counter counter)
        {
            IPluginWriter pluginWriter;

            switch (counter.Name())
            {
                default:
                    pluginWriter = new DefaultPluginWriter();
                    break;
            }

            pluginWriter.pushChunk(counter);
        }


        public void pushPartHeader(ushort type, ushort length)
        {
        //  write opening sequence for a part
        //      part type (16 bits)
        //      part length (including header, 16 bits)
        //
            _buffer.WriteByte((byte)((type & 0xFF00) >> 8));
            _buffer.WriteByte((byte)(type & 0x00FF));
            _buffer.WriteByte((byte)(((length + 4) & 0xFF00) >> 8));
            _buffer.WriteByte((byte)((length + 4) & 0x00FF));
        }

        public void pushInt64(UInt64 value)
        {
        //  split the 64 bit value into its constituent bytes and push them (big endian)
            _buffer.WriteByte((byte)(((UInt64)value & 0xFF00000000000000) >> 56));
            _buffer.WriteByte((byte)(((UInt64)value & 0x00FF000000000000) >> 48));
            _buffer.WriteByte((byte)(((UInt64)value & 0x0000FF0000000000) >> 40));
            _buffer.WriteByte((byte)(((UInt64)value & 0x000000FF00000000) >> 32));
            _buffer.WriteByte((byte)(((UInt64)value & 0x00000000FF000000) >> 24));
            _buffer.WriteByte((byte)(((UInt64)value & 0x0000000000FF0000) >> 16));
            _buffer.WriteByte((byte)(((UInt64)value & 0x000000000000FF00) >> 8));
            _buffer.WriteByte((byte)(((UInt64)value & 0x00000000000000FF)));
        }

        public void pushDouble(double value)
        {
        // dont know how to do this yet
        }

        public void pushString(ushort type, string value)
        {
            pushPartHeader(type, (ushort)(value.Length + 1));

        //  push each byte into the buffer
            foreach (byte B in Encoding.ASCII.GetBytes(value))
                _buffer.WriteByte(B);

        //  then null-terminate it
            _buffer.WriteByte(0);
        }

        public void pushNumber(ushort type, UInt64 value)
        {
            pushPartHeader(type, 8);
            pushInt64(value);
        }

        public void pushValue(List<ValuePacket> values)
        {
        //  length:  n values * (8 bytes [64-bit] + 1 byte type) + 2 bytes (value count)
            pushPartHeader(6, (ushort)((9*values.Count)+2));

            _buffer.WriteByte((byte)((values.Count & 0xFF00) >> 8));
            _buffer.WriteByte((byte)(values.Count & 0x00FF));

            foreach (ValuePacket vpacket in values)
            {
            //  push the datatype byte
                _buffer.WriteByte(vpacket.datatype);
            }

            foreach (ValuePacket vpacket in values)
            {
            //  split the 64 bit numeric into its constituent bytes and push them (big endian)
                _buffer.WriteByte((byte)((vpacket.value & 0xFF00000000000000) >> 56));
                _buffer.WriteByte((byte)((vpacket.value & 0x00FF000000000000) >> 48));
                _buffer.WriteByte((byte)((vpacket.value & 0x0000FF0000000000) >> 40));
                _buffer.WriteByte((byte)((vpacket.value & 0x000000FF00000000) >> 32));
                _buffer.WriteByte((byte)((vpacket.value & 0x00000000FF000000) >> 24));
                _buffer.WriteByte((byte)((vpacket.value & 0x0000000000FF0000) >> 16));
                _buffer.WriteByte((byte)((vpacket.value & 0x000000000000FF00) >> 8));
                _buffer.WriteByte((byte)((vpacket.value & 0x00000000000000FF)));
            }
        }

        public void pushPacketHeader()
        {
            pushString(0, Config.instance().FindOption("Hostname").FirstValue(Dns.GetHostName()));
            pushNumber(1, (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            pushNumber(7, (ulong)int.Parse(Config.instance().FindOption("Interval").FirstValue()));
        }
    }
}
