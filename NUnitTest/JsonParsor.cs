using NUnit.Framework;
using DataProcessor;
using DataProcessor.Interfaces;
using System;
using Newtonsoft.Json;

namespace TestDataProcessor.Parsors
{
    [TestFixture]
    public class JsonParsor
    {
        private IConnStrJsonParsor parsor;

        [SetUp]
        public void Setup()
        {
            parsor = new ConnStrJsonParsor();
        }

        [TestCase("", ExpectedResult = 0, Description ="Correct JSON syntax")]
        [TestCase("{'a x':'b', 'c()':'d'}", ExpectedResult = 2, Description = "Correct JSON syntax")]
        public int JsonParsorToDicCorrectSyntax(string input)
        {
            return parsor.Parse(input).Count;
        }

        
        [TestCase("['a':'b']")]
        public void JsonParsorToDicWrongSyntaxJsonSerializationException(string input)
        {
            Assert.Throws<JsonSerializationException>(() => parsor.Parse(input));
            
        }

        [TestCase("{a}")]
        [TestCase("x")]
        public void JsonParsorToDicWrongSyntaxJsonReaderException(string input)
        {
            Assert.Throws<JsonReaderException>(() => parsor.Parse(input));

        }

    }
}