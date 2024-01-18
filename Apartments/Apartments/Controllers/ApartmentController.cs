using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

namespace Apartments.Controllers
{
    public class ApartmentController : Controller
    {
        private ModelContainer db = new ModelContainer();

        ~ApartmentController() 
        {
            db.Dispose();
        }
        // GET: Apartment
        public ActionResult Index()
        {
            return View(db.Apartments);
        }

        // GET: Apartment/Details/5
        public ActionResult Details(int? id)
        {
            return CommonAction(id);
        }

        // GET: Apartment/Create
        public ActionResult Create()
        {
            ViewBag.Owners = new SelectList(db.Owners, "IDOwner", "FullName");
            return View();
        }

        // POST: Apartment/Create
        [HttpPost]
        public ActionResult Create(Apartment apartment, IEnumerable<HttpPostedFileBase> uploadedFiles)
        {
            
            ModelState.Remove("UploadedFiles");
            if (ModelState.IsValid)
                {      
                    apartment.UploadedFiles = new List<UploadedFile>();
                    AddFiles(apartment, uploadedFiles);
                    db.Apartments.Add(apartment);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            System.Diagnostics.Debug.WriteLine("ModelState is not valid. Errors:");
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Key: {key}");

                    foreach (var error in state.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
            }



            System.Diagnostics.Debug.WriteLine("Processing over due to ModelState not valid");
            return View(apartment);
            
        }

       

        // GET: Apartment/Edit/5
        public ActionResult Edit(int? id)
        {
            return CommonAction(id);
        }

        // POST: Apartment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, IEnumerable<HttpPostedFileBase> uploadedFiles)
        {
            var apartment = db.Apartments.Find(id);
            if (TryUpdateModel(
                    apartment,
                    "",
                    new string[]
                    {
                        nameof(Apartment.Address),
                        nameof(Apartment.City),
                        nameof(Apartment.Contact)
                    }
                ))
            {
                AddFiles(apartment, uploadedFiles);
                db.Entry(apartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(apartment);
        }

        // GET: Apartment/Delete/5
        public ActionResult Delete(int? id)
        {
            return CommonAction(id);
        }

        private ActionResult CommonAction(int? id)
        {
            if (id==null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var apartment = db.Apartments
                .Include(a => a.UploadedFiles)
                .SingleOrDefault(a => a.IDApartment == id);
            if (apartment == null)
            {
                return HttpNotFound();
            }
            return View(apartment);
        }

        // POST: Apartment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            db.UploadedFiles.RemoveRange(db.UploadedFiles.Where(
                f=>f.ApartmentIDApartment==id));
            db.Apartments.Remove(db.Apartments.Find(id));
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public FileContentResult GetImage(int id)
        {
            var file = db.UploadedFiles.Find(id);
            if (file != null)
            {
                return File(file.Content, file.ContentType);
            }
            return null;
        }

        private void AddFiles(Apartment apartment, IEnumerable<HttpPostedFileBase> uploadedFiles)
        {
            try
            {
                foreach (var file in uploadedFiles)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var picture = new UploadedFile
                        {
                            Name = file.FileName,
                            ContentType = file.ContentType
                        };
                        using (var reader = new BinaryReader(file.InputStream))
                        {
                            picture.Content = reader.ReadBytes(file.ContentLength);
                        }
                        apartment.UploadedFiles.Add(picture);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in AddFiles: {ex.Message}");
                throw;
            }
        }

    }
}
