using System;
using NUnit.Framework;
using Torshify.Radio.Framework;

namespace Torshify.Radio.Tests
{
    [TestFixture]
    public class TrackLinkTest
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParseInvalidTrackLink_ThrowsExecption()
        {
            TrackLink.FromUri("gibberish:moregibberish");
        }

        [Test]
        public void ParseTrackLink_DataIsDecrypted()
        {
            TrackLink expected = new TrackLink("test");
            expected["argument1"] = "some_value";
            expected["argument2"] = "another_value";

            Assert.IsNotNull(expected.Uri);

            TrackLink result = TrackLink.FromUri(expected.Uri);

            Assert.AreEqual(expected.Uri, result.Uri);
            Assert.AreEqual(expected.TrackSource, result.TrackSource);
            Assert.AreEqual(expected["argument1"], result["argument1"]);
            Assert.AreEqual(expected["argument2"], result["argument2"]);
        }
    }
}