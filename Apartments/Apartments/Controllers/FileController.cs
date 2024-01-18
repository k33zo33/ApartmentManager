﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Apartments.Controllers
{
    public class FileController : Controller
    {
        private ModelContainer db = new ModelContainer();
        ~FileController() 
        {
            if (db!=null)
            {
                db.Dispose();
            }
        }
        // GET: File
        public ActionResult Index(int id)
        {
            var uploadedFile = db.UploadedFiles.Find(id);
            return File(uploadedFile.Content, uploadedFile.ContentType);
        }
        public ActionResult Delete(int id) 
        {
            db.UploadedFiles.Remove(db.UploadedFiles.Find(id));
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.AbsolutePath);

        }
    }
}