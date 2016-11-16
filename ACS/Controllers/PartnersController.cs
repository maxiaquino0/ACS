using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ACS.Models;

namespace ACS.Controllers
{
    public class PartnersController : Controller
    {
        private ACSContext db = new ACSContext();

        // GET: Partners
        public ActionResult Index()
        {
            var partners = db.Partners.Include(p => p.DocumentType);
            return View(partners.ToList());
        }

        // GET: Partners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partner partner = db.Partners.Find(id);
            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        // GET: Partners/Create
        public ActionResult Create()
        {
            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description");
            return View();
        }

        // POST: Partners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PartnerID,LastName,Name,DocumentNumber,DocumentTypeID,Phone,Address,EMail")] Partner partner)
        {
            if (ModelState.IsValid)
            {
                db.Partners.Add(partner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description", partner.DocumentTypeID);
            return View(partner);
        }

        // GET: Partners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partner partner = db.Partners.Find(id);
            if (partner == null)
            {
                return HttpNotFound();
            }
            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description", partner.DocumentTypeID);
            return View(partner);
        }

        // POST: Partners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PartnerID,LastName,Name,DocumentNumber,DocumentTypeID,Phone,Address,EMail")] Partner partner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(partner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description", partner.DocumentTypeID);
            return View(partner);
        }

        // GET: Partners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partner partner = db.Partners.Find(id);
            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        // POST: Partners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Partner partner = db.Partners.Find(id);
            db.Partners.Remove(partner);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                
            }
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<DocumentType> GetDocumentType()
        {
            var list = db.DocumentTypes.ToList();
            list.Add(new DocumentType { DocumentTypeID = 0, Description = "[Select a Document Type]" });
            list = list.OrderBy(c => c.Description).ToList();
            return list;
        }
    }
}
