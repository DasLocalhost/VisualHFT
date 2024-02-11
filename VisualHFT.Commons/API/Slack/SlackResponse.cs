using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.API;

namespace VisualHFT.Commons.API.Slack
{
    /// <summary>
    /// Basic Slack API response container with system data.
    /// </summary>
    public class SlackBaseResponse : IBaseResponse
    {
        [JsonProperty("ok")]
        public bool Status { get; set; }

        [JsonProperty("team")]
        public string? Team { get; set; }

        [JsonProperty("user")]
        public string? User { get; set; }

        [JsonProperty("team_id")]
        public string? TeamId { get; set; }

        [JsonProperty("user_id")]
        public string? UserId { get; set; }

        [JsonProperty("bot_id")]
        public string? BotId { get; set; }
    }

    /// <summary>
    /// Response for Slack chat.postMessage API
    /// </summary>
    public class SlackSendMessageResponse : SlackBaseResponse
    {
        [JsonProperty("channel")]
        public string? Channel { get; set; }

        [JsonProperty("ts")]
        public string? MessageToken { get; set; }

        public SlackMessageDetails? Message { get; set; }
    }

    [JsonObject("message")]
    public class SlackMessageDetails
    {
        [JsonProperty("bot_id")]
        public string? BotId { get; set; }

        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("text")]
        public string? Text { get; set; }

        [JsonProperty("user")]
        public string? User { get; set; }

        [JsonProperty("ts")]
        public string? MessageToken { get; set; }

        [JsonProperty("app_id")]
        public string? AppId { get; set; }

        // Blocks skipped

        [JsonProperty("team")]
        public string? Team { get; set; }

        // Bot profile skipped

        /*
         * 
        "bot_id": "B06AH38HKUL",
        "type": "message",
        "text": "test web",
        "user": "U06B3CWV3SL",
        "ts": "1702729444.038259",
        "app_id": "A06AH34HZPE",
        "blocks": [
            {
                "type": "rich_text",
                "block_id": "h32o",
                "elements": [
                    {
                        "type": "rich_text_section",
                        "elements": [
                            {
                                "type": "text",
                                "text": "test web"
                            }
                        ]
                    }
                ]
            }
        ],
        "team": "T06B3BBTBR6",
        "bot_profile": {
            "id": "B06AH38HKUL",
            "app_id": "A06AH34HZPE",
            "name": "Notification Test",
            "icons": {
                "image_36": "https://a.slack-edge.com/80588/img/plugins/app/bot_36.png",
                "image_48": "https://a.slack-edge.com/80588/img/plugins/app/bot_48.png",
                "image_72": "https://a.slack-edge.com/80588/img/plugins/app/service_72.png"
            },
            "deleted": false,
            "updated": 1702673064,
            "team_id": "T06B3BBTBR6"
        }
         * 
         */
    }

    /// <summary>
    /// Response for Slack conversations.list API
    /// </summary>
    public class SlackChannelsListResponse : SlackBaseResponse
    {
        [JsonProperty("channels")]
        public List<SlackChannelDetails>? Channels { get; set; }
    }

    public class SlackChannelDetails
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("is_channel")]
        public bool? IsChannel { get; set; }

        [JsonProperty("is_group")]
        public bool? IsGroup { get; set; }

        [JsonProperty("is_im")]
        public bool? IsIm { get; set; }

        [JsonProperty("is_mpim")]
        public bool? IsMpIm { get; set; }

        [JsonProperty("is_private")]
        public bool? IsPrivate { get; set; }

        [JsonProperty("created")]
        public string? Created { get; set; }

        [JsonProperty("is_archived")]
        public bool? IsArchived { get; set; }

        [JsonProperty("is_general")]
        public bool? IsGeneral { get; set; }

        [JsonProperty("unlinked")]
        public int? Unlinked { get; set; }

        [JsonProperty("name_normalized")]
        public string? NormalizedName { get; set; }

        [JsonProperty("is_shared")]
        public bool? IsShared { get; set; }

        [JsonProperty("is_org_shared")]
        public bool? IsOrgShared { get; set; }

        [JsonProperty("is_pending_ext_shared")]
        public bool? IsPendingExtShared { get; set; }

        // pending_shared skipped 

        [JsonProperty("context_team_id")]
        public string? ContextTeamId { get; set; }

        [JsonProperty("updated")]
        public string? Updated { get; set; }

        // parent_conservation skipped

        [JsonProperty("creator")]
        public string? Creator { get; set; }

        [JsonProperty("is_ext_shared")]
        public bool? IsExtShared { get; set; }

        // shared_team_ids skipped
        // pending_connected_team_ids skipped

        [JsonProperty("is_member")]
        public bool? IsMember { get; set; }

        // topic skipped
        // purpose skipped
        // previous_names skipped

        [JsonProperty("num_members")]
        public int? MembersCount { get; set; }

        /*
         * 
            "id": "C06A7UF96DU",
            "name": "random",
            "is_channel": true,
            "is_group": false,
            "is_im": false,
            "is_mpim": false,
            "is_private": false,
            "created": 1702672351,
            "is_archived": false,
            "is_general": false,
            "unlinked": 0,
            "name_normalized": "random",
            "is_shared": false,
            "is_org_shared": false,
            "is_pending_ext_shared": false,
            "pending_shared": [],
            "context_team_id": "T06B3BBTBR6",
            "updated": 1702672351258,
            "parent_conversation": null,
            "creator": "U06A7UF7H7Y",
            "is_ext_shared": false,
            "shared_team_ids": [
                "T06B3BBTBR6"
            ],
            "pending_connected_team_ids": [],
            "is_member": false,
            "topic": {
                "value": "",
                "creator": "",
                "last_set": 0
            },
            "purpose": {
                "value": "This channel is for... well, everything else. It’s a place for team jokes, spur-of-the-moment ideas, and funny GIFs. Go wild!",
                "creator": "U06A7UF7H7Y",
                "last_set": 1702672351
            },
            "previous_names": [],
            "num_members": 1
         * 
         */
    }
}
