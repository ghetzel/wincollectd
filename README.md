# NOTE: This code is not yet functional #

wincollectd
===========

wincollectd is a Windows service that gathers user-specified data from system
[Performance Counters](http://msdn.microsoft.com/en-us/library/windows/desktop/aa373083\(v=vs.85\).aspx) and WMI (Windows Management Instrumentation) and delivers it to a remote collectd daemon via the collectd network plugin's [Binary Protocol](http://collectd.org/wiki/index.php/Binary_protocol).  The purpose of this service is to allow for metrics to be collected from Windows and delivered to a collectd server running on *nix such that both environments can be monitored homogenously.

Configuration
=============

The configuration for this service is modelled to be syntactically identical to existing collectd configuration files.  The purpose of this is to allow the same configuration to be deployed to both *nix and Windows clients.

Super-Alpha Software!
=====================

So far, this repository is here because I didn't want to keep it sitting solely at home anymore.  I do plan to revisit this project and make it functional.  As to when, I cannot say at the moment.