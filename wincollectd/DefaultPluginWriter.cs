using System;
using System.Collections.Generic;
using System.Text;

namespace wincollectd
{
    class DefaultPluginWriter : IPluginWriter
    {
        private PacketWriter packeteer = PacketWriter.instance();

        public DefaultPluginWriter() { }
        public void pushChunk(Counter counter)
        {
            packeteer.pushString(2, counter.Name());
            packeteer.pushString(3, counter.Object().InstanceName);
            packeteer.pushString(4, counter.Name());
            packeteer.pushString(5, counter.Object().CounterName);
            
            List<ValuePacket> values = new List<ValuePacket>();
            ValuePacket data;

            data.datatype = 0;
            data.value = (ulong)counter.Object().NextValue(); // DANGER:  need to handle this (float->int truncate)

            values.Add(data);

            packeteer.pushValue(values);
        }
    }
}
