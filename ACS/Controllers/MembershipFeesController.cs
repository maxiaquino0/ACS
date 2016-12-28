using ACS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class MembershipFeesController : Controller
    {
        private ACSContext db = new ACSContext();

        // GET: MembershipFees
        public ActionResult ChangeMembershipFee()
        {
            var membershipFees = db.MembershipFee.ToList();
            var membershipFee = new MembershipFee();
            if (membershipFees.Count == 0)
            {
                membershipFee = new MembershipFee
                {
                    MembershipFeeID = 1,
                    Amount = 0 
                };
                db.MembershipFee.Add(membershipFee);
                db.SaveChanges();
            }
            else
            {
                membershipFee = membershipFees.First();
            }
            return View(membershipFee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeMembershipFee([Bind(Include = "MembershipFeeID,Amount")] MembershipFee membershipFee)
        {            
                if (ModelState.IsValid)
                {
                    db.Entry(membershipFee).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Sports");
                }            

            return View(membershipFee);
        }
    }
}