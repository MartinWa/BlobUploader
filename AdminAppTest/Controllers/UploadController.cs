using System.Web.Mvc;

namespace AdminAppTest.Controllers
{
    [Authorize(Roles = "Uploader")]
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            return View();
        }

    }
}
