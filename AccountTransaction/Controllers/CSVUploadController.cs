using AccountTransaction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AccountTransaction.Controllers
{
    public class CSVUploadController : Controller
    {
        // GET: CSVUpload
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost, ActionName("Upload")]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase uploadFile)
        {
            //var fileName = Path.GetFileName(uploadFile.FileName);
            var output = "";
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {

                output = UploadManager.ProcessBulkCopy(uploadFile);

                ViewBag.output = uploadFile.FileName + output;

            }
            else
            {
                throw new NullReferenceException();
            }


            return View("Upload");

        }
    }
}