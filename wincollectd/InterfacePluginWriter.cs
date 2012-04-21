using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace wincollectd
{
    class InterfacePluginWriter : IPluginWriter
    {
        private PacketWriter packeteer = PacketWriter.instance();

        public InterfacePluginWriter() { }

        public void pushChunk(Counter counter)
        {
            
        }
    }
}
