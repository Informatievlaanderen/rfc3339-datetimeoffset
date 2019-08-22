namespace Be.Vlaanderen.Basisregisters.Utilities.Rfc3339DateTimeOffset.Tests
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Xunit;

    public class Rfc3339DateTimeOffsetTests
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        public Rfc3339DateTimeOffsetTests()
        {
            SerializerSettings.Converters.Add(new Rfc3339SerializableDateTimeOffsetConverter());
        }

        [Fact]
        public void WhenDeserializingXmlToDateTimeOffsetAndParsingThenEqualsExpectedResult()
        {
            var poco = "<Poco xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><Versie>2002-08-13T17:32:32+02:00</Versie></Poco>";

            var serializer = new DataContractSerializer(typeof(DeserializablePoco));

            using (var contentXmlReader = XmlReader.Create(new StringReader(poco), new XmlReaderSettings { Async = false }))
            {
                var result = (DeserializablePoco)serializer.ReadObject(contentXmlReader);
                var versie = DateTimeOffset.Parse(result.Versie);

                versie.Year.Should().Be(2002);
                versie.Month.Should().Be(8);
                versie.Day.Should().Be(13);
                versie.Hour.Should().Be(17);
                versie.Minute.Should().Be(32);
                versie.Second.Should().Be(32);
                versie.Offset.Should().Be(new TimeSpan(2, 0, 0));
            }
        }

        [Fact]
        public void WhenDeserializingXmlToDateTimeOffsetAndParsingThenEqualsExpectedUtcResult()
        {
            var poco = "<Poco xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><Versie>2002-08-13T17:32:32+02:00</Versie></Poco>";

            var serializer = new DataContractSerializer(typeof(DeserializablePoco));

            using (var contentXmlReader = XmlReader.Create(new StringReader(poco), new XmlReaderSettings { Async = false }))
            {
                var result = (DeserializablePoco)serializer.ReadObject(contentXmlReader);
                var versie = DateTimeOffset.Parse(result.Versie).UtcDateTime;

                versie.Year.Should().Be(2002);
                versie.Month.Should().Be(8);
                versie.Day.Should().Be(13);
                versie.Hour.Should().Be(15);
                versie.Minute.Should().Be(32);
                versie.Second.Should().Be(32);
            }
        }

        [Fact]
        public void WhenSerializingToJsonThenExpectCorrectString()
        {
            var poco1 = new JsonPoco1
            {
                Versie = new Rfc3339SerializableDateTimeOffset(new DateTimeOffset(2002, 08, 13, 17, 32, 32, 999, new TimeSpan(2, 0, 0)))
            };

            var poco2 = new JsonPoco2
            {
                Versie = new Rfc3339SerializableDateTimeOffset(new DateTimeOffset(2002, 08, 13, 17, 32, 32, 999, new TimeSpan(2, 0, 0)))
            };

            var poco3 = new JsonPoco2
            {
                Versie = null
            };

            var result1 = JsonConvert.SerializeObject(poco1, SerializerSettings);
            result1.Should().NotBeEmpty();
            result1.Should().Be("{\"Versie\":\"2002-08-13T17:32:32.999+02:00\"}");

            var result2 = JsonConvert.SerializeObject(poco2, SerializerSettings);
            result2.Should().NotBeEmpty();
            result2.Should().Be("{\"Versie\":\"2002-08-13T17:32:32.999+02:00\"}");

            var result3 = JsonConvert.SerializeObject(poco3, SerializerSettings);
            result3.Should().NotBeEmpty();
            result3.Should().Be("{\"Versie\":null}");

            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            var result4 = JsonConvert.SerializeObject(poco3, SerializerSettings);
            result4.Should().NotBeEmpty();
            result4.Should().Be("{}");
        }

        [Fact]
        public void GivenDateTimeAsDateHandlingWhenDeserializingToJsonThenExpectCorrectString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = (DateTimeOffset)result.Versie;

            versie.Year.Should().Be(2002);
            versie.Month.Should().Be(8);
            versie.Day.Should().Be(13);
            versie.Hour.Should().Be(15);
            versie.Minute.Should().Be(32);
            versie.Second.Should().Be(32);
            versie.Offset.Should().Be(new TimeSpan(0, 0, 0));
        }

        [Fact]
        public void GivenDateTimeAsDateHandlingWhenDeserializingToJsonThenExpectCorrectUtcString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = ((DateTimeOffset)result.Versie).UtcDateTime;

            versie.Year.Should().Be(2002);
            versie.Month.Should().Be(8);
            versie.Day.Should().Be(13);
            versie.Hour.Should().Be(15);
            versie.Minute.Should().Be(32);
            versie.Second.Should().Be(32);
        }

        [Fact]
        public void GivenDateTimeOffsetAsDateHandlingWhenDeserializingToJsonThenExpectCorrectString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = (DateTimeOffset)result.Versie;

            versie.Year.Should().Be(2002);
            versie.Month.Should().Be(8);
            versie.Day.Should().Be(13);
            versie.Hour.Should().Be(17);
            versie.Minute.Should().Be(32);
            versie.Second.Should().Be(32);
            versie.Offset.Should().Be(new TimeSpan(2, 0, 0));
        }

        [Fact]
        public void GivenDateTimeOffsetAsDateHandlingWhenDeserializingToJsonThenExpectCorrectUtcString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = ((DateTimeOffset)result.Versie).UtcDateTime;

            versie.Year.Should().Be(2002);
            versie.Month.Should().Be(8);
            versie.Day.Should().Be(13);
            versie.Hour.Should().Be(15);
            versie.Minute.Should().Be(32);
            versie.Second.Should().Be(32);
        }

        [Fact]
        public void GivenNoneAsDateHandlingWhenDeserializingToJsonThenExpectCorrectString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.None;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = (DateTimeOffset)result.Versie;

            versie.Year.Should().Be(2002);
            versie.Month.Should().Be(8);
            versie.Day.Should().Be(13);
            versie.Hour.Should().Be(17);
            versie.Minute.Should().Be(32);
            versie.Second.Should().Be(32);
            versie.Offset.Should().Be(new TimeSpan(2, 0, 0));
        }

        [Fact]
        public void GivenNoneAsDateHandlingWhenDeserializingToJsonThenExpectCorrectUtcString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.None;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = ((DateTimeOffset)result.Versie).UtcDateTime;

            versie.Year.Should().Be(2002);
            versie.Month.Should().Be(8);
            versie.Day.Should().Be(13);
            versie.Hour.Should().Be(15);
            versie.Minute.Should().Be(32);
            versie.Second.Should().Be(32);
        }

        [DataContract(Name = "Poco", Namespace = "")]
        public class DeserializablePoco
        {
            [DataMember]
            public string Versie { get; set; }
        }

        [DataContract(Name = "Poco", Namespace = "")]
        public class JsonPoco1
        {
            [DataMember]
            public Rfc3339SerializableDateTimeOffset Versie { get; set; }
        }

        [DataContract(Name = "Poco", Namespace = "")]
        public class JsonPoco2
        {
            [DataMember]
            public Rfc3339SerializableDateTimeOffset? Versie { get; set; }
        }
    }
}
