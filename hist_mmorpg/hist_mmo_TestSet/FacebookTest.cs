using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;
namespace hist_mmo_TestSet
{
    class FacebookTest
    {
        public void getToken()
        {
            var fb = new FacebookClient();
            dynamic result = fb.Get("oauth/access_token", new
            {
                client_id = "app_id",
                client_secret = "app_secret",
                grant_type = "client_credentials"
            });
        }
    }
}
