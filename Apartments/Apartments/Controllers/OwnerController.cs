using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Apartments.Controllers
{
    public class OwnerController : Controller
    {
        private ModelContainer db = new ModelContainer();
        ~OwnerController()
        {
            db.Dispose();
        }


        // GET: Owner
        public ActionResult Index()
        {
            return View(db.Owners);
        }

        // GET: Owner/Details/5
        public ActionResult Details(int? id)
        {
            return CommonAction(id);
        }

        // GET: Owner/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Owner/Create
        [HttpPost]
        public ActionResult Create(Owner owner)
        {
            if (ModelState.IsValid)
            {
                db.Owners.Add(owner);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(owner);
        }

        // GET: Owner/Edit/5
        public ActionResult Edit(int? id)
        {
            return CommonAction(id);
        }

        // POST: Owner/Edit/5
        [HttpPost]
        public ActionResult Edit(int id)
        {
            var owner = db.Owners.Find(id);
            if (TryUpdateModel(
                owner,
                "",
                new string[]
                {
                    nameof(Owner.FirstName),
                    nameof(Owner.LastName)
                }
                ))
            {
                db.Entry(owner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(owner);
        }

        // GET: Owner/Delete/5
        public ActionResult Delete(int? id)
        {
            return CommonAction(id);
        }

        private ActionResult CommonAction(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var owner = db.Owners
                .SingleOrDefault(o => o.IDOwner == id);
            if (owner == null)
            {
                return HttpNotFound();
            }
            return View(owner);
        }

        // POST: Owner/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            db.Apartments.RemoveRange(db.Apartments.Where(
                a => a.IDApartment == id));
            db.Owners.Remove(db.Owners.Find(id));
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
