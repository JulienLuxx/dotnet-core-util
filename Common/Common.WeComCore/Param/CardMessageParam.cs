using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public class CardMessageParam
    {
        [JsonProperty("touser")]
        public string ToUser { get; set; }

        [JsonProperty("toparty")]
        public string ToParty { get; set; }

        [JsonProperty("totag")]
        public string ToTag { get; set; }

        [JsonProperty("msgtype")]
        public string MsgType { get; set; }

        [JsonProperty("agentid")]
        public int AgentId { get; set; }

        [JsonProperty("textcard")]
        public TextCard TextCard { get; set; }

        [JsonProperty("enable_id_trans")]
        public int EnableIdTrans { get; set; }

        [JsonProperty("enable_duplicate_check")]
        public int EnableDuplicateCheck { get; set; }

        [JsonProperty("duplicate_check_interval")]
        public int DuplicateCheckInterval { get; set; }

        public CardMessageParam() 
        {
            DuplicateCheckInterval = 1800;
        }
    }

    public class TextCard
    {
        /// <summary>
        /// 标题
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// 按钮文本
        /// </summary>
        [JsonProperty("btntxt")]
        public string BtnTxt { get; set; }
    }
}
