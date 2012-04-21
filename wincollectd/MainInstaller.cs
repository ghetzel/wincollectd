using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;


namespace wincollectd
{
    [RunInstaller(true)]
    public partial class MainInstaller : Installer
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public MainInstaller()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = Properties.Settings.Default.ServiceAccount;
            serviceInstaller.StartType = Properties.Settings.Default.ServiceStartMode;
            serviceInstaller.ServiceName = Properties.Settings.Default.ServiceName;
            serviceInstaller.DisplayName = Properties.Settings.Default.ServiceDisplayName;
            serviceInstaller.Description = Properties.Settings.Default.ServiceDescription;

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}
