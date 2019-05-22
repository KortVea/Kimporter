using System;
using System.Collections.Generic;
using System.Text;
using DataProcessor.Models;

namespace DataProcessor.DAL
{
    public class TracePropertyRepo: RepoBase<DownloadedPropertyData>
    {
        public TracePropertyRepo(string connStr): base(connStr)
        {
        }

    }
}
