using System;
using System.Collections.Generic;
using System.Text;

namespace Common.WeComCore
{
    public class SignatureResultDto : WeComBaseResultDto
    {
        public SignatureDto Signature { get; set; }
    }

    public class SignatureDto
    {
        public string AgentId { get; set; }

        public string CorpId { get; set; }

        public string NonceStr { get; set; }

        public string Signature { get; set; }

        public string Ticket { get; set; }

        public long Timestamp { get; set; }
    }
}
