using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace wincollectd
{
    public partial class MainService : ServiceBase
    {
        private Daemon _daemon = new Daemon();

        public MainService()
        {
            InitializeComponent();

            CanPauseAndContinue = false;
            ServiceName = Properties.Settings.Default.ServiceName;

            if (!System.Diagnostics.EventLog.SourceExists(Properties.Settings.Default.LogSource))
                System.Diagnostics.EventLog.CreateEventSource(Properties.Settings.Default.LogSource, Properties.Settings.Default.LogName);

            logger.Source = Properties.Settings.Default.LogSource;
            logger.Log = Properties.Settings.Default.LogName;

        }

        protected override void OnStart(string[] args)
        {
            _daemon.run();
            logger.WriteEntry(Properties.Settings.Default.LogSource + " Started...");
        }

        protected override void OnStop()
        {
            _daemon.stop();
            logger.WriteEntry(Properties.Settings.Default.LogSource + " Stopped");
        }
    }
}
