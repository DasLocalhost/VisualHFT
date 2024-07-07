using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using System.Threading.Channels;
using VisualHFT.Commons.API;

namespace VisualHFT.Commons.API.Slack
{
    /// <summary>
    /// Implementation of the Rest client for the Slack API.
    /// </summary>
    public class SlackClient
    {
        // TODO : token could be replaced in settings, so token update method needs to be created.
        private readonly string _token = "";

        private readonly RestClient _client;

        #region API

        private readonly string _baseApiUrl = "https://slack.com/api/";

        private readonly string _tokenValidation = "auth.test";
        private readonly string _sendMessage = "chat.postMessage";
        private readonly string _updateMessage = "chat.update";
        private readonly string _channelsList = "conversations.list";

        #endregion

        public SlackClient(string token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            _token = token;

            var authHeader = new OAuth2AuthorizationRequestHeaderAuthenticator(_token, "Bearer");
            var options = new RestClientOptions(_baseApiUrl)
            {
                Authenticator = authHeader
            };

            _client = new RestClient(options);

            if (!ValidateToken())
                throw new InvalidTokenException();
        }

        private bool ValidateToken()
        {
            var validationResp = RequestBuilder.Make(_client, _tokenValidation)
                .Post<SlackBaseResponse>();

            return validationResp?.Status ?? false;
        }

        private string? GetChannelToken(string channelName)
        {
            var channels = GetChannels();

            var channel = channels?.FirstOrDefault(_ => _.Name == channelName);

            return channel?.Id;
        }

        private IList<SlackChannelDetails>? GetChannels()
        {
            var channelsResp = RequestBuilder.Make(_client, _channelsList)
                .Post<SlackChannelsListResponse>();

            return channelsResp?.Channels ?? null;
        }

        public void Send(SlackMessage message)
        {
            // TODO : add exception if channel is null
            var chToken = GetChannelToken(message.Channel) ?? message.Channel;

            var sendResponse = RequestBuilder.Make(_client, _sendMessage)
                .WithParams("channel".With(chToken), "text".With(message.Text))
                .Post<SlackSendMessageResponse>();

            // TODO : make an exception for this case (Slack API not responding)
            if (!sendResponse?.Status ?? false)
                throw new Exception();
        }

        // TODO : change method from send to update
        public string? Update(SlackMessage message)
        {
            var sendResponse = RequestBuilder.Make(_client, _sendMessage)
                .WithParams("channel".With(message.Channel), "text".With(message.Text))
                .Post<SlackSendMessageResponse>();

            // TODO : make an exception for this case (Slack API not responding)
            if (!sendResponse?.Status ?? false)
                throw new Exception();

            return sendResponse?.MessageToken;
        }
    }

    public class InvalidTokenException : Exception
    {
        public InvalidTokenException() : base("Invalid auth token for Slack API") { }
    }

    // TODO : move out to some kind of helper with extentions methods
    public static class StringExtentions
    {
        public static KeyValuePair<string, string?> With(this string key, string? value)
        {
            return new KeyValuePair<string, string?>(key, value);
        }
    }
}