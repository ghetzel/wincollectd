namespace wincollectd
{
    interface IPluginWriter
    {
        void pushChunk(Counter counter);
    }
}
