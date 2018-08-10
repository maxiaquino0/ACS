using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ACS.Models;
using ACS.ViewModels;

namespace ACS.Controllers
{
    [Authorize(Roles = "Administrator, Employee")]
    public class PartnersController : Controller
    {
        private ACSContext db = new ACSContext();

        // GET: Partners
        public ActionResult Index()
        {
            var partners = db.Partners.Include(p => p.DocumentType).Where(h => h.PartnerHeadOfFamilyID == 0);
            return View(partners.ToList());
        }

        // GET: IndexFamilyGroup
        public ActionResult IndexFamilyGroup(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var partners = db.Partners.Include(p => p.DocumentType).Where(h => h.PartnerHeadOfFamilyID == id);
            ViewBag.PartnerHeadOfFamilyID = id;
            return View(partners.ToList());
        }

        // GET: SportsList
        public ActionResult SportList(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var partnerSportsView = new PartnerSportView();
            var partner = db.Partners.Find(id);
            partnerSportsView.Sport = new Sport();
            partnerSportsView.Sports = new List<Sport>();
            partnerSportsView.Partner = new Partner {
                LastName = partner.LastName,
                Name = partner.Name,
                PartnerID = partner.PartnerID,
                PartnerHeadOfFamilyID = partner.PartnerHeadOfFamilyID
            };
            var sportsID = db.PartnerSport.Where(p => p.PartnerID == id).Select(s => s.SportID).ToList();
            foreach (var item in sportsID)
            {
                var sport = db.Sports.Find(item);
                partnerSportsView.Sports.Add(sport);
            }
            Session["partnerSportsView"] = partnerSportsView;

            return View(partnerSportsView);
        }

        //GET AddSport
        public ActionResult AddSport()
        {
            var listS = GetSports();
            ViewBag.SportID = new SelectList(listS, "SportID", "Description");
            return View();
        }

        [HttpPost]
        public ActionResult AddSport(Sport sport)
        {
            var partnerSportsView = Session["partnerSportsView"] as PartnerSportView;
            var sportID = int.Parse(Request["SportID"]);
            if (sportID == 0)
            {
                var list = GetSports();
                ViewBag.SportID = new SelectList(list, "SportID", "Description");
                ViewBag.Error = "You must select a Sport.";
                return View();
            }

            var sportF = db.Sports.Find(sportID);

            var partnerSportID = partnerSportsView.Sports.Find(s => s.SportID == sportF.SportID);
            if (partnerSportID == null)
            {
                partnerSportsView.Sports.Add(sportF);
                var partnerSport = new PartnerSport
                {
                    PartnerID = partnerSportsView.Partner.PartnerID,
                    SportID = sportF.SportID
                };
                db.PartnerSport.Add(partnerSport);
                db.SaveChanges();
            }
            

            

            return View("SportList", partnerSportsView);
        }

        // DeleteSport
        public ActionResult DeleteSport(int sportID, int partnerID)
        {
            if (sportID == 0 || partnerID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var partner = db.Partners.Find(partnerID);
            var sport = db.Sports.Find(sportID);
            var partnerSport = db.PartnerSport.First(p => p.SportID == sportID && p.PartnerID == partnerID);
            db.PartnerSport.Remove(partnerSport);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                
            }

            var partnerSportsView = new PartnerSportView();
            
            partnerSportsView.Sport = new Sport();
            partnerSportsView.Sports = new List<Sport>();
            partnerSportsView.Partner = new Partner
            {
                LastName = partner.LastName,
                Name = partner.Name,
                PartnerID = partner.PartnerID,
                PartnerHeadOfFamilyID = partner.PartnerHeadOfFamilyID
            };
            var sportsID = db.PartnerSport.Where(p => p.PartnerID == partnerID).Select(s => s.SportID).ToList();
            foreach (var item in sportsID)
            {
                var sportP = db.Sports.Find(item);
                partnerSportsView.Sports.Add(sportP);
            }
            Session["partnerSportsView"] = partnerSportsView;

            return View("SportList", partnerSportsView);

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
            return View("Details", partner);
        }

        // GET: Partners/Create
        public ActionResult Create()
        {
            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description");
            return View();
        }

        // GET: Partners/Create
        public ActionResult CreateFamily(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description");
            var partner = new Partner {
                PartnerHeadOfFamilyID = id
            };
            return View("Create", partner);
        }

        // POST: Partners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PartnerID,LastName,Name,DocumentNumber,DocumentTypeID,Phone,Address,EMail,PartnerHeadOfFamilyID")] Partner partner)
        {
            
            if (ModelState.IsValid)
            {
                if (partner.PartnerHeadOfFamilyID == null)
                {
                    partner.PartnerHeadOfFamilyID = 0;
                }                
                db.Partners.Add(partner);
                db.SaveChanges();
                if (partner.PartnerHeadOfFamilyID == 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("IndexFamilyGroup", new { id = partner.PartnerHeadOfFamilyID});
                }
                
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
        public ActionResult Edit([Bind(Include = "PartnerID,LastName,Name,DocumentNumber,DocumentTypeID,Phone,Address,EMail,PartnerHeadOfFamilyID")] Partner partner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(partner).State = EntityState.Modified;
                db.SaveChanges();
                if (partner.PartnerHeadOfFamilyID == 0)
                {
                    return RedirectToAction("Index", new { id = partner.PartnerHeadOfFamilyID });
                }
                else
                {
                    return RedirectToAction("IndexFamilyGroup", new { id = partner.PartnerHeadOfFamilyID });
                }                
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
            catch (Exception)
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

        public List<Sport> GetSports()
        {
            var list = db.Sports.ToList();
            list.Add(new Sport { SportID = 0, Description = "[Select a Sport]" });
            list = list.OrderBy(c => c.Description).ToList();
            return list;
        }
    }
}
