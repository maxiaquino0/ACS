using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ACS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ACS.Controllers
{
    [Authorize(Roles = "Administrator, Employee")]
    public class EmployeesController : Controller
    {
        private ACSContext db = new ACSContext();

        // para creacion de usuarios
        ApplicationDbContext dbu = new ApplicationDbContext();

        // GET: Employees
        public ActionResult Index()
        {
            var employees = db.Employees.Include(e => e.DocumentType);
            return View(employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,LastName,Name,DocumentNumber,DocumentTypeID,Address,Phone,Email")] Employee employee)
        {
            if (int.Parse(Request["DocumentTypeID"]) != 0)
            {
                if (ModelState.IsValid)
                {
                    db.Employees.Add(employee);

                    // Create user
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbu));
                    var user = new ApplicationUser
                    {
                        UserName = employee.Email,
                        Email = employee.Email
                    };
                    userManager.Create(user, employee.DocumentNumber);

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }else
            {
                ViewBag.Error = "You must select a Document Type.";
            }            
            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description", employee.DocumentTypeID);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description", employee.DocumentTypeID);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,LastName,Name,DocumentNumber,DocumentTypeID,Address,Phone,Email")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var list = GetDocumentType();
            ViewBag.DocumentTypeID = new SelectList(list, "DocumentTypeID", "Description", employee.DocumentTypeID);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
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
