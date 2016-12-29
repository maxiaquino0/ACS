using ACS.Models;
using ACS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ACS.Controllers
{
    [Authorize(Roles = "Administrator, Employee")]
    public class MonthlyFeesController : Controller
    {
        private ACSContext db = new ACSContext();

        // GET: MonthlyFees
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var partner = db.Partners.Find(id);
            if (partner == null)
            {
                ViewBag.Error = "Socio no existente.";
                return RedirectToAction("Index", "Partners");
            }
            Session["partnerHeadOfFamilyID"] = id;
            
            ViewBag.PartnerFullName = partner.FullName.ToString();

            var monthlyFeeList = db.MonthlyFees.Where(m => m.PartnerID == id).ToList().OrderByDescending(mf => mf.Period);

            return View(monthlyFeeList);
        }

        // NewMonthlyFees
        public ActionResult NewMonthlyFee()
        {
            
            var partnerHeadOfFamilyID = int.Parse(Session["partnerHeadOfFamilyID"].ToString());

            var partner = db.Partners.Find(partnerHeadOfFamilyID);
            if(partner == null)
            {
                return HttpNotFound();
            }
            var monthlyFeeList = db.MonthlyFees.Where(p => p.PartnerID == partnerHeadOfFamilyID).ToList();
            var monthlyFee = new MonthlyFee();
            foreach (var item in monthlyFeeList)
            {
                if (item.Period.Year == DateTime.Now.Year && item.Period.Month == DateTime.Now.Month)
                {
                    monthlyFee = item;
                }
            }

            if (monthlyFee.MonthlyFeeID != 0)
            {
                TempData["Message"] = string.Format("La cuota para éste período ya fue generada.");
                return RedirectToAction("Index", "MonthlyFees", new { id = partner.PartnerID });
            }
            var monthlyFeeID = 0;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var membershipFee = db.MembershipFee.ToList();
                    if (membershipFee.Count == 0)
                    {
                        TempData["Message"] = string.Format("Debe cargar un monto de cuota social primero.");
                        return RedirectToAction("Index", "MonthlyFees", new { id = partner.PartnerID });
                    }
                    var membershipFeeAmount = db.MembershipFee.First().Amount;   
                    monthlyFee = new MonthlyFee
                    {
                        PartnerID = partnerHeadOfFamilyID,
                        Period = DateTime.Now,
                        MonthlyFeeStatus = MonthlyFeeStatus.Debe,
                        MembershipFeeAmount = membershipFeeAmount
                    };
                    db.MonthlyFees.Add(monthlyFee);
                    db.SaveChanges();
                    monthlyFeeID = db.MonthlyFees.Select(m => m.MonthlyFeeID).Max();

                    var partners = db.Partners.Where(p => p.PartnerHeadOfFamilyID == partnerHeadOfFamilyID).ToList();

                    foreach (var item in partners)
                    {
                        var partnersports = db.PartnerSport.Where(p => p.PartnerID == item.PartnerID).ToList();
                        foreach (var partnersport in partnersports)
                        {
                            var sport = db.Sports.First(s => s.SportID == partnersport.SportID);
                            var monthlyFeeDetail = new MonthlyFeeDetail {
                                MonthlyFeeID = monthlyFeeID,
                                PartnerID = partnersport.PartnerID,
                                SportID = sport.SportID,
                                Description = sport.Description,
                                FeeAmount = sport.FeeAmount
                            };
                            db.MonthlyFeeDetails.Add(monthlyFeeDetail);
                            db.SaveChanges();
                        }
                        
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ViewBag.Error = "Error: " + ex.Message;
                    return View("Index", partner.PartnerID);
                }
            }

            TempData["Message"] = string.Format("La cuota de {0}, numero {1} fue guardada OK.", partner.FullName, monthlyFeeID);

            return RedirectToAction("Index", "MonthlyFees", new { id = partner.PartnerID });
        }

        
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

            var monthlyFeeDetails = new MonthlyFeeViewDetail();
            monthlyFeeDetails.MonthlyFee = monthlyFee;
            monthlyFeeDetails.Partner = db.Partners.Find(monthlyFee.PartnerID);
            monthlyFeeDetails.MonthlyFeeDetails = db.MonthlyFeeDetails.Where(m => m.MonthlyFeeID == id).ToList();
            
            decimal montoTotal = 0;
            foreach(var item in monthlyFeeDetails.MonthlyFeeDetails)
            {
                montoTotal += item.FeeAmount;
            }

            monthlyFeeDetails.MonthlyFeeDetails.Add(new MonthlyFeeDetail { Description = "Membership Fee", FeeAmount = monthlyFee.MembershipFeeAmount });
            montoTotal += monthlyFee.MembershipFeeAmount;

            monthlyFeeDetails.MonthlyFeeDetails.Add(new MonthlyFeeDetail { MonthlyFeeID = 0, MonthlyFeeDetailID = 0 , Description = "Total amount: ", FeeAmount=montoTotal});
            Session["monthlyFeeViewDetail"] = monthlyFeeDetails;

            return View(monthlyFeeDetails);
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
            for (int i = 0; i < (monthlyFeeViewDetail.MonthlyFeeDetails.Count()-2); i++)
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