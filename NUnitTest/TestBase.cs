using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NUnitTest
{
    public class TestBase
    {
        public FieldInfo GetFieldInfoPrivateStatic(Type type, string name)
        {
            return type.GetField(name, BindingFlags.Static | BindingFlags.NonPublic);
        }
    }
}
