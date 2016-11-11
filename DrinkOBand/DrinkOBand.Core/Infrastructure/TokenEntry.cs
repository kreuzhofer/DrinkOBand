using System;
using System.Collections.Generic;
using System.Text;

namespace DrinkOBand.Shared.Infrastructure
{
    public class UserClaim
    {
        public string typ { get; set; }
        public string val { get; set; }
    }

    public class TokenEntry
    {
        public string access_token { get; set; }
        public string expires_on { get; set; }
        public string provider_name { get; set; }
        public string refresh_token { get; set; }
        public List<UserClaim> user_claims { get; set; }
        public string user_id { get; set; }
    }
}
