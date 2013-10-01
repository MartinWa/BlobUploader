using System;
using System.Text;
using System.Web.Mvc;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobUploader.Controllers
{
    [Authorize(Roles = "Uploader")]
    public class UploadController : Controller
    {
        public ActionResult Index()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));
            var client = storageAccount.CreateCloudBlobClient();
            var staticUploadUrl =
                client.GetContainerReference("public").GetBlockBlobReference("upload.html").Uri.AbsoluteUri;
            var container = client.GetContainerReference("upload");
            container.CreateIfNotExists();
            var blob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
            var sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Write,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30)
            });
            var uploadUri = blob.Uri.AbsoluteUri + sas;
            var uriInBytes = Encoding.Default.GetBytes(uploadUri);
            var base64Uri = Convert.ToBase64String(uriInBytes);
            ViewBag.Url = staticUploadUrl + "?sas=" + base64Uri;
            ViewBag.Width = 400;
            ViewBag.Height = 450;
            return View();
        }
    }
}
