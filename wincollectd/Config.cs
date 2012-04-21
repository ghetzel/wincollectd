using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace wincollectd
{
    class Config
    {
        private static Config _instance;
        private string _filename;
        private ConfigSection _root = new ConfigSection();

        public Config() { }

        public static Config instance()
        {
            if (_instance == null)
                _instance = new Config();
            return _instance;
        }

        public Config(string filename)
        {
            SetFilename(filename);
        }

        public void SetFilename(string filename)
        {
            _filename = filename;
            init();
        }

        public ConfigOption FindOption(string path)
        {
            return Root().FindOption(path);
        }

        public ConfigSection Root()
        {
            return _root;
        }

        private void init()
        {
            try
            {
                _root.SetType("ROOT");
                _root.SetData(File.ReadAllText(_filename));
            }
            catch (Exception)
            {
            //  error occurred
            }
        }
    }
}
