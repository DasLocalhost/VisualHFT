using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Authenticators.OAuth2;
using VisualHFT.Commons.API;

namespace VisualHFT.Commons.API.Zapier
{
    public class ZapierClient
    {
        // TODO : token could be replaced in settings, so token update method needs to be created.
        #region API

        private readonly string _baseWebhookUrl = "https://hooks.zapier.com/hooks/";
        private readonly string _webhook = "catch/17381048/3agp54i/";

        #endregion

        private readonly RestClient _client;

        public ZapierClient(string? webhook)
        {
            if (webhook == null)
                throw new ArgumentNullException(nameof(webhook));

            _webhook = webhook;

            _client = new RestClient(_baseWebhookUrl);
        }

        public void Send(ZapierMessage message)
        {
            var sendResponse = RequestBuilder.Make(_client, _webhook)
                .WithBody(message)
                .Post<ZapierBaseResponse>();

            if (!(sendResponse?.Status == "success"))
                throw new Exception();
        }
    }
}
