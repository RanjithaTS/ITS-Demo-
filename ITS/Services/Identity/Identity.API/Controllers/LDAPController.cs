using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    /// <summary>
    /// <Reference>
    /// URL : https://decovar.dev/blog/2022/06/16/dotnet-ldap-authentication/
    /// </Reference>
    /// </summary>
    /// 

    [ApiController]
    [Route("api/[controller]")]
    public class LDAPController
    {
        public LDAPController()
        {

        }

        [HttpGet]
        public string ConnectionCheck()
        {
            return "LDAP Connection Established";
        }



    }
}
