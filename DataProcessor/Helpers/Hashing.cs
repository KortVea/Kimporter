using DataProcessor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataProcessor.Helpers
{
    public static class Hashing
    {
        /// <summary>
        /// Computes a hash over a series of data members to produce a final hash for the TraceData record.
        /// Specifically this method computes the hash based on the hash of
        /// TraceData's AST|Time property x DID property
        ///
        /// The hashing algorithm here is based on a variation of the  Berstein hash, with a different constant
        /// (23) applied
        ///
        /// Refer to https://stackoverflow.com/questions/1646807/quick-and-simple-hash-code-combinations
        /// for more information
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long CreateHash(DownloadedTraceData data)
        {
            long hash = 23;

            if (data.DownloadedPropertyData?.FirstOrDefault(p => p.PropertyKey == "AST") != null)
            {
                var ast = data.DownloadedPropertyData.FirstOrDefault(p => p.PropertyKey == "AST");
                if (ast != null)
                {
                    hash = hash * 31 + ast.PropertyValue.GetHashCode();
                }
            }
            else
            {
                hash = hash * 31 + data.Time.Ticks.GetHashCode();
            }

            if (data.DownloadedPropertyData?.FirstOrDefault(p => p.PropertyKey == "DID") != null)
            {
                var did = data.DownloadedPropertyData.FirstOrDefault(p => p.PropertyKey == "DID");
                if (did != null)
                {
                    if (did.PropertyValue.Contains('|'))
                    {
                        try
                        {
                            var lines = did.PropertyValue.Split('|');
                            hash = hash * 31 + lines[0].GetHashCode();
                        }
                        catch (Exception)
                        {
                            hash = hash * 31 + did.PropertyValue.GetHashCode();
                        }
                    }
                    else
                    {
                        hash = hash * 31 + did.PropertyValue.GetHashCode();
                    }
                }
            }

            hash = hash * 31 + data.Source.GetHashCode();
            hash = hash * 31 + data.Type.GetHashCode();

            return hash;
        }
    }
}
