//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace DataProcessor.Helpers
//{
//    public class Hashing
//    {
//        /// <summary>
//        /// Computes a hash over a series of data members to produce a final hash for the TraceData record.
//        /// Specifically this method computes the hash based on the hash of
//        /// TraceData's AST|Time property x DID property
//        ///
//        /// The hashing algorithm here is based on a variation of the  Berstein hash, with a different constant
//        /// (23) applied
//        ///
//        /// Refer to https://stackoverflow.com/questions/1646807/quick-and-simple-hash-code-combinations
//        /// for more information
//        /// </summary>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        private static long CreateHash(TraceData data)
//        {
//            long hash = 23;

//            if (data.property?.FirstOrDefault(p => p.key == "AST") != null)
//            {
//                var ast = data.property.FirstOrDefault(p => p.key == "AST");
//                if (ast != null)
//                {
//                    hash = hash * 31 + ast.value.GetHashCode();
//                }
//            }
//            else
//            {
//                hash = hash * 31 + data.time.Ticks.GetHashCode();
//            }

//            if (data.property?.FirstOrDefault(p => p.key == "DID") != null)
//            {
//                var did = data.property.FirstOrDefault(p => p.key == "DID");
//                if (did != null)
//                {
//                    if (did.value.Contains('|'))
//                    {
//                        try
//                        {
//                            var lines = did.value.Split('|');
//                            hash = hash * 31 + lines[0].GetHashCode();
//                        }
//                        catch (Exception)
//                        {
//                            hash = hash * 31 + did.value.GetHashCode();
//                        }
//                    }
//                    else
//                    {
//                        hash = hash * 31 + did.value.GetHashCode();
//                    }
//                }
//            }

//            hash = hash * 31 + data.source.GetHashCode();
//            hash = hash * 31 + data.type.GetHashCode();

//            return hash;
//        }
//    }
//}
