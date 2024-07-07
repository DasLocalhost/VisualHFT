using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.API.Slack;

namespace VisualHFT.Test.API
{

    // TODO : change from unit tests to contract-based tests
    public class SlackApiTest
    {
        private string _token = string.Empty;

        //[SetUp]
        public void Setup()
        {
            _token = "";
        }

        //[Test]
        public void InitTest()
        {
            try
            {
                var _client = new SlackClient(_token);
                Assert.Pass();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        //[Test]
        [TestCase("invalid_token_for_tests")]
        public void InitFailedTest(string token)
        {
            try
            {
                var _client = new SlackClient(token);
                Assert.Fail("Init done successfully");
            }
            catch (InvalidTokenException)
            {
                Assert.Pass();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        //[Test]
        public void SendTest()
        {
            try
            {
                var _client = new SlackClient(_token);
                Assert.Pass();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
