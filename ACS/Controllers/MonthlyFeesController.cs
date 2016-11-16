﻿using ACS.Models;
using ACS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ACS.Controllers
{
    public class MonthlyFeesController : Controller
    {
        private ACSContext db = new ACSContext();

        // GET: MonthlyFees
        public ActionResult Index(int id)
        {
            var partner = db.Partners.Find(id);
            if (partner == null)
            {
                ViewBag.Error = "Cliente no existente.";
                return RedirectToAction("Index", "Customers");
            }
            Session["partnerID"] = id;
            
            ViewBag.PartnerFullName = partner.FullName.ToString();

            var monthlyFeeList = db.MonthlyFees.Where(m => m.PartnerID == id).ToList();

            return View(monthlyFeeList);
        }

        // GET: MonthlyFees
        public ActionResult NewMonthlyFee()
        {
            var partnerID = Session["partnerID"];
            
            var partner = db.Partners.Find(partnerID);
            if(partner == null)
            {
                return HttpNotFound();
            }

            var monthlyFeeView = new MonthlyFeeView();
            monthlyFeeView.Partner = partner;
            monthlyFeeView.Sports = new List<Sport>();
            
            Session["monthlyFeeView"] = monthlyFeeView;

            return View(monthlyFeeView);
        }

        // POST: NewMonthlyFee
        [HttpPost]
        public ActionResult NewMonthlyFee(MonthlyFeeView monthlyFeeView)
        {
            monthlyFeeView = Session["monthlyFeeView"] as MonthlyFeeView;

            if(monthlyFeeView.Sports.Count == 0)
            {
                ViewBag.Error = "You need to add a product.";
                return View(monthlyFeeView);
            }

            int monthlyFeeID = 0;
            using(var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var monthlyFee = new MonthlyFee
                    {
                        PartnerID = monthlyFeeView.Partner.PartnerID,
                        Period = DateTime.Now,
                        MonthlyFeeStatus = MonthlyFeeStatus.Debe
                    };
                    db.MonthlyFees.Add(monthlyFee);
                    db.SaveChanges();

                    monthlyFeeID = db.MonthlyFees.Select(m => m.MonthlyFeeID).Max();

                    foreach (var item in monthlyFeeView.Sports)
                    {
                        var monthlyFeeDetail = new MonthlyFeeDetail
                        {
                            MonthlyFeeID = monthlyFeeID,
                            SportID = item.SportID,
                            Description = item.Description,
                            FeeAmount = item.FeeAmount
                        };
                        db.MonthlyFeeDetails.Add(monthlyFeeDetail);
                        db.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ViewBag.Error = "Error: " + ex.Message;
                    return View(monthlyFeeView);
                }
            }
            

            TempData["Message"] = string.Format("La cuota de {0}, numero {1} fue guardada OK.", monthlyFeeView.Partner.FullName, monthlyFeeID);
            
            var partner = db.Partners.Find(Session["partnerID"]);
            monthlyFeeView = new MonthlyFeeView();
            monthlyFeeView.Partner = partner;
            monthlyFeeView.Sports = new List<Sport>();

            Session["monthlyFeeView"] = monthlyFeeView;

            return RedirectToAction("Index", "MonthlyFees", new { id = partner.PartnerID });
        }

        // GET: AddSport
        public ActionResult AddSport()
        {
            var listS = GetSports();
            ViewBag.SportID = new SelectList(listS, "SportID", "Description");
            return View();
        }

        [HttpPost]
        public ActionResult AddSport(Sport sport)
        {
            var monthlyFeeView = Session["monthlyFeeView"] as MonthlyFeeView;
            var sportID = int.Parse(Request["SportID"]);
            if(sportID == 0)
            {
                var list = GetSports();
                ViewBag.SportID = new SelectList(list, "SportID", "Description");
                ViewBag.Error = "You must select a Sport.";
                return View();
            }

            var sportF = db.Sports.Find(sportID);

            monthlyFeeView.Sports.Add(sportF);
            
            return View("NewMonthlyFee", monthlyFeeView);
        }

        // GET: Partners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonthlyFee monthlyFee = db.MonthlyFees.Find(id);
            if (monthlyFee == null)
            {
                return HttpNotFound();
            }
            var partnerID = Session["partnerID"];
            MonthlyFeeViewDetail monthlyFeeViewDetail = new MonthlyFeeViewDetail();
            monthlyFeeViewDetail.Partner = db.Partners.Find(partnerID);
            monthlyFeeViewDetail.MonthlyFeeDetails = db.MonthlyFeeDetails.Where(m => m.MonthlyFeeID == id).ToList();
            decimal montoTotal = 0;
            foreach(var item in monthlyFeeViewDetail.MonthlyFeeDetails)
            {
                montoTotal += item.FeeAmount;
            }
            monthlyFeeViewDetail.MonthlyFeeDetails.Add(new MonthlyFeeDetail { MonthlyFeeID = 0, MonthlyFeeDetailID = 0 , Description = "Total amount: ", FeeAmount=montoTotal});

            Session["monthlyFeeViewDetail"] = monthlyFeeViewDetail;
            return View(monthlyFeeViewDetail);
        }

        [HttpPost]
        public ActionResult Details(MonthlyFeeViewDetail monthlyFeeViewDetail)
        {
            monthlyFeeViewDetail = Session["monthlyFeeViewDetail"] as MonthlyFeeViewDetail;

            if (monthlyFeeViewDetail == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var monthlyFeeID = 0;
            for (int i = 0; i < (monthlyFeeViewDetail.MonthlyFeeDetails.Count()-1); i++)
            {
                monthlyFeeID = monthlyFeeViewDetail.MonthlyFeeDetails[i].MonthlyFeeID;
            }

            

            MonthlyFee monthlyFee = db.MonthlyFees.Find(monthlyFeeID);
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    monthlyFee.MonthlyFeeStatus = MonthlyFeeStatus.Pagada;
                    db.SaveChanges();
                    TempData["Message"] = string.Format("La cuota numero {0} fue pagada.", monthlyFee.MonthlyFeeID);
                    transaction.Commit();

                    return RedirectToAction("Index", "MonthlyFees", new { id = monthlyFee.PartnerID });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = string.Format("Error: ", ex);
                    return RedirectToAction("Index", "MonthlyFees", new { id = monthlyFee.PartnerID });
                }
            }

        }
        
        public List<Sport> GetSports()
        {
            var list = db.Sports.ToList();
            list.Add(new Sport { SportID = 0, Description = "[Select a Sport]" });
            list = list.OrderBy(c => c.Description).ToList();
            return list;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}