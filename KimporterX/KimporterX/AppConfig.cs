using DataProcessor;
using DataProcessor.DAL;
using DataProcessor.Interfaces;
using DataProcessor.Models;
using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace KimporterX
{
    public static class AppConfig
    {
        public static void Config()
        {
            //IOC
            FreshIOC.Container.Register<ITraceRepo<DownloadedTraceData>, TraceRepo>();
            FreshIOC.Container.Register<IKMLParser, KMLParser>();
            FreshIOC.Container.Register<IConnStrJsonParser, ConnStrJsonParser>();
        }

    }
}
