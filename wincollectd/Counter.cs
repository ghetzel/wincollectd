using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace wincollectd
{
    class Counter
    {
        private PerformanceCounter _perfCounter;
        private string _mappedName;

        public Counter(string mappedName, PerformanceCounter counter)
        {
            _mappedName = mappedName;
            _perfCounter = counter;
        }

        public PerformanceCounter Object()
        {
            return _perfCounter;
        }

        public string Name()
        {
            return _mappedName;
        }

        public override string ToString()
        {
            if (_perfCounter.InstanceName.Length > 0)
                return Name() + "-" + _perfCounter.InstanceName;

            return Name();
        }
    }
}
