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
            FreshIOC.Container.Register<ITraceRepo<DownloadedTraceData>, TraceRepo>();
        }
    }
}
