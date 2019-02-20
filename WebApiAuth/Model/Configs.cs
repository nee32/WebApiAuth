using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAuth.Model
{
    public class Configs
    {
        public int ApiRequestExpireTime { get; set; }

        public List<ApiUser> ApiUser { get; set; }
    }

    public class ApiUser
    {
        public string app_key { get; set; }

        public string secret { get; set; }
    }
}
