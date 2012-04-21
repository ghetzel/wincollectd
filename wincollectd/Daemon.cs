using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace wincollectd
{
    class Daemon
    {
        private Timer _timer = new Timer();
        private Config _config = Config.instance();
        private List<Counter> _counters = new List<Counter>();
        private int _interval = 0; // seconds
        private Dictionary<string, string> _counterCategoryMap = new Dictionary<string, string>();

        public Daemon(){
            _config.SetFilename(Util.GetProgramFilesDir() + "\\" + Properties.Settings.Default.ProgramName + "\\" + Properties.Settings.Default.ConfigFilename);
            init();
        }

        public void run()
        {
            if (Properties.Settings.Default.ConsoleMode)
            {
            //  console mode is blocking
                while (true)
                {
                    consoleRefresh();
                //  sleep for (interval * 1000) msec
                    System.Threading.Thread.Sleep(_interval * 1000);
                }
            }
            else
            {
            //  service mode is non-blocking
                _timer.Elapsed += new ElapsedEventHandler(poll);
                _timer.Interval = (_interval * 1000);
                _timer.Enabled = true;
            }
        }

        public void stop()
        {
            _timer.Enabled = false;
        }

        private void consoleRefresh()
        {
            Console.Clear();
            Console.WriteLine("COUNTER".PadRight(30) + "  " + "INSTANCE".PadLeft(8) + "  " + "VALUE".PadLeft(16));
            Console.WriteLine("================================================================================");

            foreach (Counter c in _counters)
            {
                PerformanceCounter pcObject = c.Object();

                Console.WriteLine((c.ToString()).PadRight(20) + "  " + pcObject.CounterName.PadRight(20) + "  " + pcObject.NextValue().ToString().PadLeft(16));
            }
        }

        private void poll(object source, ElapsedEventArgs e)
        {
            PacketWriter.instance().SendData();
        }

        private void init(){
            buildCounters();

            //PacketWriter.instance().SetHost(_config.FindOption("@network/Server").FirstValue(DEFAULT_HOST));
            //PacketWriter.instance().SetHost("10.200.50.10");
            PacketWriter.instance().SetHost("192.168.78.105");

            _interval = int.Parse(_config.FindOption("Interval").FirstValue(Properties.Settings.Default.Interval));
            if (_interval <= 0)
                _interval = Properties.Settings.Default.Interval;

            foreach (Counter c in _counters)
                PacketWriter.instance().addCounter(c);
        }

        private void buildCounters()
        {
        //  for each 'counter' Plugin...
            foreach (ConfigSection section in _config.Root().Sections("Plugin", "counter"))
            {

            //  for each Counter definition section...
                foreach (ConfigSection counter in section.Sections("Counter"))
                {
                //  the Counter must have a name, and a Category must be specified
                    if (counter.Name().Length > 0 && !counter.Option("Category").IsEmpty())
                    {
                        string szSystemCategory = counter.Option("Category").FirstValue();
                        _counterCategoryMap.Add(counter.Name(), szSystemCategory);

                    //  pull the current counter category
                        PerformanceCounterCategory pcCategory = new PerformanceCounterCategory(szSystemCategory);

                    //  get all of its instances
                        string[] pcInstances = pcCategory.GetInstanceNames();

                        if (pcInstances.Length > 0)
                        {
                        //  for each instance in this category...
                            foreach (string szInstance in pcInstances)
                            {
                            //  if an exclude is specified, all but that instance should be shown
                                if (!counter.Option("ExcludeInstance").IsEmpty() && counter.Option("ExcludeInstance").Values().Contains(szInstance))
                                    continue;

                            //  if an include is specified, only those instances should be shown
                                if (!counter.Option("Instance").IsEmpty() && !counter.Option("Instance").Values().Contains(szInstance))
                                    continue;

                            //  get all counters
                                PerformanceCounter[] pcCounters = pcCategory.GetCounters(szInstance);

                            //  for each counter...
                                foreach (PerformanceCounter pcCounter in pcCounters)
                                {
                                //  only add this counter if it was explicitly added in the config
                                    if (!counter.Option("CounterName").Values().Contains(pcCounter.CounterName))
                                        continue;

                                    pcCounter.InstanceName = szInstance;
                                    _counters.Add(new Counter(counter.Name(), pcCounter));
                                }
                            }
                        }
                        else
                        {
                        //  get all counters
                            PerformanceCounter[] pcCounters = pcCategory.GetCounters();

                        //  for each counter...
                            foreach (PerformanceCounter pcCounter in pcCounters)
                            {
                            //  only add this counter if it was explicitly added in the config
                                if (!counter.Option("CounterName").Values().Contains(pcCounter.CounterName))
                                    continue;

                                _counters.Add(new Counter(counter.Name(), pcCounter));
                            }
                        }
                    }
                }
            }
        }
    }
}
