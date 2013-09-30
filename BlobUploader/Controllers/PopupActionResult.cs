using System.Web.Mvc;

namespace BlobUploader.Controllers
{
    public class PopupActionResult : ActionResult
    {
        private readonly string _url;
        private readonly int _width;
        private readonly int _height;

        public PopupActionResult(string url, int width = 400, int height = 450)
        {
            _url = url;
            _width = width;
            _height = height;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write("<script>" + 
                "window.open('" + _url +"','_blank', 'width=" + _width + ",height=" + _height + "');" + 
                "setInterval(function(){history.back()}, 3000);" +
                "</script>");
        }
    }
}