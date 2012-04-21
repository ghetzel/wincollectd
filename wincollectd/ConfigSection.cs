using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace wincollectd
{
    class ConfigSection
    {
        private string _sectionType = null;
        private string _sectionName = null;
        private Dictionary<string, ConfigOption> _options = new Dictionary<string, ConfigOption>();
        private List<ConfigSection> _sections = new List<ConfigSection>();

        public ConfigSection(){}
        public ConfigSection(string type)
        {
            SetType(type);
        }
        public ConfigSection(string type, string name)
        {
            SetType(type);
            SetName(name);
        }
        public ConfigSection(string type, string name, string data)
        {
            SetType(type);
            SetName(name);
            SetData(data);
        }

        public void SetType(string type)
        {
            _sectionType = type;
        }

        public void SetName(string name)
        {
            _sectionName = name;
        }

        public string Type()
        {
            return _sectionType;
        }

        public string Name()
        {
            return _sectionName;
        }

        public override string ToString()
        {
            if (Name() != null)
                return Type() + ":" + Name();
            return Type();
        }

        public ConfigOption FindOption(string path)
        {
            string[] cmp = path.Split('/');
            List<ConfigSection> sections;
            ConfigSection curSection = this;

            for (int i = 0; i < (cmp.Length - 1); i++)
            {
                sections = curSection.Sections();
                if (sections.Count == 0)
                    return null;

            //  preceding '@' means find by section name
                if (cmp[i].StartsWith("@"))
                    curSection = sections.Find(section => section.Name() == cmp[i].Substring(1));
                else
                    curSection = sections[0];
            }

            return curSection.Option(cmp[cmp.Length-1]);
        }

        public List<ConfigSection> Sections()
        {
            return _sections;
        }

        public List<ConfigSection> Sections(string type)
        {
            List<ConfigSection> rv = new List<ConfigSection>();
            foreach (ConfigSection section in _sections)
            {
                if (section.Type() == type)
                    rv.Add(section);
            }

            return rv;
        }

        public List<ConfigSection> Sections(string type, string name)
        {
            List<ConfigSection> rv = new List<ConfigSection>();
            foreach (ConfigSection section in _sections)
            {
                if (section.Type() == type && section.Name() == name)
                    rv.Add(section);
            }

            return rv;
        }

        public ConfigOption Option(string name)
        {
            if (_options.ContainsKey(name))
                return _options[name];
            return new ConfigOption(name);
        }

        public void SetData(string data)
        {
            string[] szConfigLines = data.Split('\n');
            string szNormalizedData = "";
            List<string> szCullStrings = new List<string>();

            foreach (string line in szConfigLines)
            {
                string szCurLine = line.Trim();

                //  skip commented lines
                if (Regex.Match(szCurLine, "^#.*").Success)
                    continue;

                //  skip blank lines
                if (Regex.Match(szCurLine, "^[ ]*$").Success)
                    continue;

                szNormalizedData += szCurLine +"\n";
            }


            Match sectionData = Regex.Match(szNormalizedData, "<[ ]*([A-Za-z0-9]*)[ ]*\"?([A-Za-z0-9]*)?\"?[ ]*>(.*?)</\\1>", RegexOptions.Singleline);
            while(sectionData.Success){
                string hType = sectionData.Groups[1].Captures[0].Value;
                string hName = sectionData.Groups[2].Captures[0].Value;
                string sData = sectionData.Groups[3].Captures[0].Value;

                if(sData.Trim().Length > 0)
                    szCullStrings.Add(sData);

                _sections.Add(new ConfigSection(hType, hName, sData));                
                sectionData = sectionData.NextMatch();
            }

        //  remove the sections we already delegate to another parser
            foreach (string cull in szCullStrings)
            {
                szNormalizedData = szNormalizedData.Replace(cull, "");
            }

        //  split the remaining lines
            foreach (string szLine in szNormalizedData.Split('\n'))
            {
                string szCurOptLine = szLine.Trim();

            //  only accept lines that aren't section openers/closers
                if (szCurOptLine.Length > 0 && szCurOptLine[0] != '<')
                {
                    char[] sep = { ' ' };
                    string[] szCmp = szCurOptLine.Split(sep, 2, StringSplitOptions.RemoveEmptyEntries);

                //  parse the option into name/value pair
                    if (szCmp.Length == 2)
                    {
                        string fieldName = szCmp[0];
                        string fieldValue = szCmp[1].Replace("\"", "");

                    //  create the temp/new option entry
                        ConfigOption curOpt = new ConfigOption(fieldName);

                    //  if this option already exists, pull it
                        if (_options.ContainsKey(fieldName))
                            curOpt = _options[fieldName];
                        else
                            _options.Add(fieldName, curOpt);

                    //  push the new value into the option
                        curOpt.PushValue(fieldValue);

                    //  save the new option
                        _options[fieldName] = curOpt;
                    }
                }
            }
        }
    }
}
