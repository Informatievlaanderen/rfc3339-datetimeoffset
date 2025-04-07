namespace Be.Vlaanderen.Basisregisters.Utilities.Rfc3339DateTimeOffset.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Newtonsoft.Json;
    using Shouldly;
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

                versie.Year.ShouldBe(2002);
                versie.Month.ShouldBe(8);
                versie.Day.ShouldBe(13);
                versie.Hour.ShouldBe(17);
                versie.Minute.ShouldBe(32);
                versie.Second.ShouldBe(32);
                versie.Offset.ShouldBe(new TimeSpan(2, 0, 0));
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

                versie.Year.ShouldBe(2002);
                versie.Month.ShouldBe(8);
                versie.Day.ShouldBe(13);
                versie.Hour.ShouldBe(15);
                versie.Minute.ShouldBe(32);
                versie.Second.ShouldBe(32);
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
            result1.ShouldNotBeNullOrEmpty();
            result1.ShouldBe("{\"Versie\":\"2002-08-13T17:32:32.999+02:00\"}");

            var result2 = JsonConvert.SerializeObject(poco2, SerializerSettings);
            result2.ShouldNotBeNullOrEmpty();
            result2.ShouldBe("{\"Versie\":\"2002-08-13T17:32:32.999+02:00\"}");

            var result3 = JsonConvert.SerializeObject(poco3, SerializerSettings);
            result3.ShouldNotBeNullOrEmpty();
            result3.ShouldBe("{\"Versie\":null}");

            SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            var result4 = JsonConvert.SerializeObject(poco3, SerializerSettings);
            result4.ShouldNotBeNullOrEmpty();
            result4.ShouldBe("{}");
        }

        [Fact]
        public void GivenDateTimeAsDateHandlingWhenDeserializingToJsonThenExpectCorrectString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = (DateTimeOffset)result.Versie;
            var utcVersionTime = versie.UtcDateTime;

            utcVersionTime.Year.ShouldBe(2002);
            utcVersionTime.Month.ShouldBe(8);
            utcVersionTime.Day.ShouldBe(13);
            utcVersionTime.Hour.ShouldBe(15);
            utcVersionTime.Minute.ShouldBe(32);
            utcVersionTime.Second.ShouldBe(32);

            versie.Offset.ShouldBe(DateTimeOffset.Now.Offset);
        }

        [Fact]
        public void GivenDateTimeAsDateHandlingWhenDeserializingToJsonThenExpectCorrectUtcString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = ((DateTimeOffset)result.Versie).UtcDateTime;

            versie.Year.ShouldBe(2002);
            versie.Month.ShouldBe(8);
            versie.Day.ShouldBe(13);
            versie.Hour.ShouldBe(15);
            versie.Minute.ShouldBe(32);
            versie.Second.ShouldBe(32);
        }

        [Fact]
        public void GivenDateTimeOffsetAsDateHandlingWhenDeserializingToJsonThenExpectCorrectString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = (DateTimeOffset)result.Versie;

            versie.Year.ShouldBe(2002);
            versie.Month.ShouldBe(8);
            versie.Day.ShouldBe(13);
            versie.Hour.ShouldBe(17);
            versie.Minute.ShouldBe(32);
            versie.Second.ShouldBe(32);
            versie.Offset.ShouldBe(new TimeSpan(2, 0, 0));
        }

        [Fact]
        public void GivenDateTimeOffsetAsDateHandlingWhenDeserializingToJsonThenExpectCorrectUtcString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = ((DateTimeOffset)result.Versie).UtcDateTime;

            versie.Year.ShouldBe(2002);
            versie.Month.ShouldBe(8);
            versie.Day.ShouldBe(13);
            versie.Hour.ShouldBe(15);
            versie.Minute.ShouldBe(32);
            versie.Second.ShouldBe(32);
        }

        [Fact]
        public void GivenNoneAsDateHandlingWhenDeserializingToJsonThenExpectCorrectString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.None;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = (DateTimeOffset)result.Versie;

            versie.Year.ShouldBe(2002);
            versie.Month.ShouldBe(8);
            versie.Day.ShouldBe(13);
            versie.Hour.ShouldBe(17);
            versie.Minute.ShouldBe(32);
            versie.Second.ShouldBe(32);
            versie.Offset.ShouldBe(new TimeSpan(2, 0, 0));
        }

        [Fact]
        public void GivenNoneAsDateHandlingWhenDeserializingToJsonThenExpectCorrectUtcString()
        {
            SerializerSettings.DateParseHandling = DateParseHandling.None;
            var result = JsonConvert.DeserializeObject<JsonPoco1>("{\"Versie\":\"2002-08-13T17:32:32+02:00\"}", SerializerSettings);
            var versie = ((DateTimeOffset)result.Versie).UtcDateTime;

            versie.Year.ShouldBe(2002);
            versie.Month.ShouldBe(8);
            versie.Day.ShouldBe(13);
            versie.Hour.ShouldBe(15);
            versie.Minute.ShouldBe(32);
            versie.Second.ShouldBe(32);
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
