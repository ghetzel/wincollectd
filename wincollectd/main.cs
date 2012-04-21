using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace wincollectd
{
    static class main
    {

        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new MainService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
