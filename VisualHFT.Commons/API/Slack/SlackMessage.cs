using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.Commons.API.Slack
{
    public class SlackMessage
    {
        private bool _markdown = true;

        public string? Text { get; set; }

        public string? ResponseType { get; set; }

        public bool ReplaceOriginal { get; set; }

        public bool DeleteOriginal { get; set; }

        public string? Channel { get; set; }

        public string? Username { get; set; }

        public string? IconEmoji { get; set; }

        public Uri? IconUrl { get; set; }

        [JsonProperty(PropertyName = "mrkdwn")]
        public bool Markdown
        {
            get
            {
                return _markdown;
            }
            set
            {
                _markdown = value;
            }
        }

        [Obsolete("Mrkdwn has been deprecated, please use 'Markdown' instead.")]
        [JsonIgnore]
        public bool Mrkdwn
        {
            get
            {
                return _markdown;
            }
            set
            {
                _markdown = value;
            }
        }

        public bool LinkNames { get; set; }

        public ParseMode Parse { get; set; }

        [JsonProperty("thread_ts")]
        public string? ThreadId { get; set; }

        //public List<SlackAttachment> Attachments { get; set; }

        //public List<Block> Blocks { get; set; }

        public SlackMessage Clone(string? newChannel = null)
        {
            return new SlackMessage
            {
                //Attachments = Attachments,
                //Blocks = Blocks,
                Channel = newChannel ?? Channel,
                DeleteOriginal = DeleteOriginal,
                IconEmoji = IconEmoji,
                IconUrl = IconUrl,
                LinkNames = LinkNames,
                Markdown = Markdown,
                Parse = Parse,
                ReplaceOriginal = ReplaceOriginal,
                ResponseType = ResponseType,
                Text = Text,
                Username = Username
            };
        }

        public bool ShouldSerializeIconEmoji()
        {
            if (IconUrl == null)
            {
                return IconEmoji != null;
            }

            return false;
        }

        //public string AsJson()
        //{
        //    return SlackClient.SerializeObject(this);
        //}
    }
    public enum ParseMode
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "full")]
        Full
    }
}
