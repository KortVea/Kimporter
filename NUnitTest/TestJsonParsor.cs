using NUnit.Framework;
using DataProcessor;
using DataProcessor.Interfaces;
using System;
using Newtonsoft.Json;
using NUnitTest;

namespace TestDataProcessor.Parsors
{
    [TestFixture]
    public class TestJsonParsor: TestBase
    {
        private IConnStrJsonParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new ConnStrJsonParser();
        }

        [TestCase("", ExpectedResult = 0, Description ="Correct JSON syntax")]
        [TestCase("{'a x':'b', 'c()':'d'}", ExpectedResult = 2, Description = "Correct JSON syntax")]
        public int JsonParsorToDicCorrectSyntax(string input)
        {
            return _parser.Parse(input).Count;
        }

        
        [TestCase("['a':'b']")]
        public void JsonParsorToDicWrongSyntaxJsonSerializationException(string input)
        {
            Assert.Throws<JsonSerializationException>(() => _parser.Parse(input));
            
        }

        [TestCase("{a}")]
        [TestCase("x")]
        public void JsonParsorToDicWrongSyntaxJsonReaderException(string input)
        {
            Assert.Throws<JsonReaderException>(() => _parser.Parse(input));

        }

    }
}