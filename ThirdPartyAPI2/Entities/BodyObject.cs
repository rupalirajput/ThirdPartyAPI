using System;

namespace ThirdPartyAPI2.Entities
{
    public class BodyObject
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
#nullable enable
        public string? Detail { get; set; }
#nullable disable
        public DateTime CreatedAt { get; set; }
#nullable enable
        public DateTime? UpdatedAt { get; set; }
    }
}
