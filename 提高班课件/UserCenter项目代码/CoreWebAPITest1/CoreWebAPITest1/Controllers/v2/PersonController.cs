using Microsoft.AspNetCore.Mvc;

namespace CoreWebAPITest1.Controllers.v2
{
    [Produces("application/json")]
    //[Route("api/Person")]
    public class PersonController : Controller
    {
        public string Get()
        {
            return "v2";
        }
    }
}
