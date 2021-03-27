using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class HubSpotEntity
    {
        public class Firstname
        {
            public string value { get; set; }
        }

        public class Lastmodifieddate
        {
            public string value { get; set; }
        }

        public class Company
        {
            public string value { get; set; }
        }

        public class Lastname
        {
            public string value { get; set; }
        }

        public class Properties
        {
            public Firstname firstname { get; set; }
            public Lastmodifieddate lastmodifieddate { get; set; }
            public Company company { get; set; }
            public Lastname lastname { get; set; }
        }

        public class Identity
        {
            public string type { get; set; }
            public string value { get; set; }
            public object timestamp { get; set; }

            [JsonProperty("is-primary")]
            public bool IsPrimary { get; set; }
        }

        public class IdentityProfile
        {
            public int vid { get; set; }

            [JsonProperty("saved-at-timestamp")]
            public object SavedAtTimestamp { get; set; }

            [JsonProperty("deleted-changed-timestamp")]
            public int DeletedChangedTimestamp { get; set; }
            public List<Identity> identities { get; set; }
        }

        public class Contact
        {
            public object addedAt { get; set; }
            public int vid { get; set; }

            [JsonProperty("canonical-vid")]
            public int CanonicalVid { get; set; }

            [JsonProperty("merged-vids")]
            public List<object> MergedVids { get; set; }

            [JsonProperty("portal-id")]
            public int PortalId { get; set; }

            [JsonProperty("is-contact")]
            public bool IsContact { get; set; }
            public Properties properties { get; set; }

            [JsonProperty("form-submissions")]
            public List<object> FormSubmissions { get; set; }

            [JsonProperty("identity-profiles")]
            public List<IdentityProfile> IdentityProfiles { get; set; }

            [JsonProperty("merge-audits")]
            public List<object> MergeAudits { get; set; }
        }

        public class Root
        {
            public List<Contact> contacts { get; set; }

            [JsonProperty("has-more")]
            public bool HasMore { get; set; }

            [JsonProperty("vid-offset")]
            public int VidOffset { get; set; }
        }
    }
}
