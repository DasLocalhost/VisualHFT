using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.API.Slack;

namespace VisualHFT.Commons.API.Twitter
{
    public class TwitterClient
    {
        // TODO : token could be replaced in settings, so token update method needs to be created.
        private readonly string? _consumerKey;
        private readonly string? _consumerSecret;
        private readonly string? _token;
        private readonly string? _tokenSecret;

        private readonly RestClient _client;

        #region API

        private readonly static string _baseApiUrl = "https://api.twitter.com/";

        private readonly static string _tokenRequest = "oauth/request_token";
        private readonly static string _accessTokenRequest = "oauth/access_token";
        private readonly static string _tokenValidation = "1.1/account/verify_credentials.json";
        private readonly static string _sendMessage = "2/tweets";

        private readonly static CookieContainer _cookie = new CookieContainer();

        #endregion

        public TwitterClient(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            if (consumerKey == null)
                throw new ArgumentNullException(nameof(consumerKey));
            if (consumerSecret == null)
                throw new ArgumentNullException(nameof(consumerSecret));
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            if (tokenSecret == null)
                throw new ArgumentNullException(nameof(tokenSecret));

            this._consumerKey = consumerKey;
            this._consumerSecret = consumerSecret;
            this._token = token;
            this._tokenSecret = tokenSecret;

            var authenticator = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, token, tokenSecret);
            var options = new RestClientOptions(_baseApiUrl)
            {
                Authenticator = authenticator
            };

            _client = new RestClient(options);

            if (!ValidateToken())
                throw new InvalidTokenException();
        }

        #region OAuth methods

        /// <summary>
        /// First step to auth to the Twitter API. Send the request to get an oauth token.
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>
        /// <returns></returns>
        public static (string? token, string? secret)? GetOAuthToken(string consumerKey, string consumerSecret)
        {
            var authHeader = new OAuth1Authenticator()
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret,
            };
            var options = new RestClientOptions(_baseApiUrl)
            {
                Authenticator = authHeader,
                CookieContainer = _cookie
            };

            var client = new RestClient(options);

            var result = RequestBuilder.Make(client, _tokenRequest).WithParams("oauth_callback".With("oob")).Post();

            if (result == null)
                return null;

            var oauthToken = result.GetUriParameter("oauth_token");
            var oauthTokenSecret = result.GetUriParameter("oauth_token_secret");

            return (oauthToken, oauthTokenSecret);
        }

        /// <summary>
        /// Second step to auth to the Twitter API. Open the oauth URL on browser and ask user to log in.
        /// </summary>
        /// <param name="token">Auth token from the first step</param>
        /// <param name="secret">Auth token from the second step</param>
        public static void OpenAuthUrl(string token, string secret)
        {
            var link = $"https://api.twitter.com/oauth/authorize?oauth_token={token}&oauth_token_secret={secret}&oauth_callback_confirmed=true";
            Process.Start(new ProcessStartInfo(link) { UseShellExecute = true });
        }

        /// <summary>
        /// Third step to auth to the Twitter API. Use the verifier string from user to get access token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="verifier"></param>
        /// <returns></returns>
        public static (string? token, string? secret)? GetAccessToken(string token, string verifier)
        {
            //var authHeader = new OAuth1Authenticator()
            //{
            //    ConsumerKey = consumerKey,
            //    ConsumerSecret = consumerSecret,
            //};
            var options = new RestClientOptions(_baseApiUrl)
            {
                CookieContainer = _cookie
            };

            var client = new RestClient(options);

            var result = RequestBuilder.Make(client, _accessTokenRequest)
                .WithParams("oauth_token".With(token), "oauth_verifier".With(verifier)).Post();

            if (result == null)
                return null;

            var oauthToken = result.GetUriParameter("oauth_token");
            var oauthTokenSecret = result.GetUriParameter("oauth_token_secret");

            return (oauthToken, oauthTokenSecret);
        }

        #endregion

        public void Send(TwitterMessage message)
        {
            var sendResponse = RequestBuilder.Make(_client, _sendMessage)
                .WithBody(message)
                .Post<TwitterPostResponse>();
        }

        private bool ValidateToken()
        {
            var validationStatus = RequestBuilder.Make(_client, _tokenValidation).Get();

            return validationStatus;
        }
    }
}