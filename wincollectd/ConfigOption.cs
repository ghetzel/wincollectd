using System;
using System.Collections.Generic;
using System.Text;

namespace wincollectd
{
    class ConfigOption
    {
        private string _name = null;
        private List<string> _values = new List<string>();

        public ConfigOption() { }
        public ConfigOption(string name)
        {
            SetName(name);
        }
        public ConfigOption(string name, string firstValue)
        {
            SetName(name);
            PushValue(firstValue);
        }
        public ConfigOption(string name, List<string> values)
        {
            SetName(name);
            SetValues(values);
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public void PushValue(string value)
        {
            _values.Add(value);
        }

        public string Name()
        {
            return _name;
        }

        public List<string> Values()
        {
            return _values;
        }

        public string FirstValue()
        {
            if (_values.Count > 0)
                return _values[0];
            return null;
        }

        public string FirstValue(string def)
        {
            if (_values.Count > 0)
                return _values[0];
            return def;
        }

        public string FirstValue(int def)
        {
            if (_values.Count > 0)
                return _values[0];
            return def.ToString();
        }

        public string FirstValue(double def)
        {
            if (_values.Count > 0)
                return _values[0];
            return def.ToString();
        }

        public bool IsEmpty()
        {
            return (_values.Count == 0);
        }

        private void SetValues(List<string> values)
        {
            _values = values;
        }
    }
}
