using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCUserRole.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MVCUserRole.Controllers
{
    public class FilleController : Controller
    {
        private AttendanceDBEntities2 db = new AttendanceDBEntities2();
        private ApplicationDbContext con = new ApplicationDbContext();
        // GET: Filles
        public ActionResult Index()
        {
            
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(con));
            var userId = User.Identity.GetUserId();
            var s = UserManager.GetRoles(userId);
            if (s[0].ToString() == "Admin")
            {
                var show = (from saw in db.Fille select saw).ToList();
                ViewBag.tampil = show;
                ViewBag.owner = "Admin";
            }
            else
            {
                var tampil = (from sho in db.Fille where sho.UserId == userId select sho).ToList();
                ViewBag.tampil = tampil;
                ViewBag.owner = "Bukan";
            }

            //return View(db.Fille.ToList());
            return View();
        }

        // GET: Filles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fille fille = db.Fille.Find(id);
            if (fille == null)
            {
                return HttpNotFound();
            }
            return View(fille);
        }

        // GET: Filles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Filles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Fille filee , HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string[] AllowedFilesExtentions = new string[] { ".jpg", ".gif", ".png", ".pdf" };
                int MaxContentLength = 1024 * 1024 * 3;// 3 MB

                if (file == null)
                {
                    ModelState.AddModelError("", "Please upload your file");
                }
                //else if (!AllowedFilesExtentions.Contains(file.ContentType))
                //{
                //    ModelState.AddModelError("FileName", "Please choose either a jpg, gif, png or pdf");
                //}
                else if (file.ContentLength > MaxContentLength)
                {
                    ModelState.AddModelError("", "Your file is too large, maximum allowed size is " + MaxContentLength + " MB");
                }
                else if (file.ContentLength > 0 && file.ContentLength < MaxContentLength)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    file.SaveAs(path);
                    

                    db.Fille.Add(new Fille
                    {
                        
                        UserId = User.Identity.GetUserId(),
                            FileId = filee.FileId,
                            FileName = file.FileName,
                            FilePath = "~/Images/" + file.FileName
                        });

                        //db.Fille.Add(fille);
                        db.SaveChanges();
                        ModelState.Clear();
                        ViewBag.Message = "file uploaded successfully";
                    
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        // GET: Filles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fille fille = db.Fille.Find(id);
            if (fille == null)
            {
                return HttpNotFound();
            }
            return View(fille);
        }

        // POST: Filles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FileId,FileName,FilePath")] Fille fille)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fille).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fille);
        }

        // GET: Filles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fille fille = db.Fille.Find(id);
            if (fille == null)
            {
                return HttpNotFound();
            }
            return View(fille);
        }

        // POST: Filles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fille fille = db.Fille.Find(id);
            db.Fille.Remove(fille);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //status approval
        public ActionResult Approve(int id)
        {
            Fille file = db.Fille.Find(id);

            if (file.Approval == true)
            {
                file.Approval = false;
                ViewBag.status = "false";
            }
            else
            {
                file.Approval = true;
                ViewBag.status = "true";
            }

            db.Entry(file).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        //Download File
        public ActionResult Download(int id)
        {
            var download = (from down in db.Fille where down.FileId == id select down).SingleOrDefault();
            var fileName = Path.GetFileName(download.FileName);

            if (download != null)
            {
                Response.AddHeader("content-disposition", "inline; filename=" + download.FileName);
                return File(download.FilePath, "Application/octet-stream");
            }
            else
            {
                return null;
            }
            
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
