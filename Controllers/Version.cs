
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OKexTime.Controllers
{
    [Route("")]
    [ApiController]
    public class Version : ControllerBase
    {
        public ActionResult Index()
        {
            return Ok("Version: 1.0.0.1");
        }

        
    }
}
