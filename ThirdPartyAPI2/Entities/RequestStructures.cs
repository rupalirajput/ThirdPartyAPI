using System;

namespace ThirdPartyAPI2.Entities
{
    public class RequestObject
    {
        public string Body { get; set; }
    }

    public class CallbackRequestObject
    {
        public string Status { get; set; }
        public string Detail { get; set; }
    }

    public class GetStatusResponseObject
    {
#nullable enable
        public string? Status { get; set; }
#nullable disable
        public string Detail { get; set; }
        public string Body { get; set; }
        public DateTime CreatedAt { get; set; }
#nullable enable
        public DateTime? UpdatedAt { get; set; }
    }
}
