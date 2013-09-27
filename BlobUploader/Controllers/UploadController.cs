using System.Web.Mvc;

namespace BlobUploader.Controllers
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
