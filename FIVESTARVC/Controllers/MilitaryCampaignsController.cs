using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;

namespace FIVESTARVC.Controllers
{
    public class MilitaryCampaignsController : Controller
    {
        private ResidentContext db = new ResidentContext();

        // GET: MilitaryCampaigns
        public ActionResult Index()
        {
            return View(db.MilitaryCampaigns.ToList());
        }

        // GET: MilitaryCampaigns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MilitaryCampaign militaryCampaign = db.MilitaryCampaigns.Find(id);
            if (militaryCampaign == null)
            {
                return HttpNotFound();
            }
            return View(militaryCampaign);
        }

        // GET: MilitaryCampaigns/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MilitaryCampaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MilitaryCampaignID,CampaignName")] MilitaryCampaign militaryCampaign)
        {
            if (ModelState.IsValid)
            {
                db.MilitaryCampaigns.Add(militaryCampaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(militaryCampaign);
        }

        // GET: MilitaryCampaigns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MilitaryCampaign militaryCampaign = db.MilitaryCampaigns.Find(id);
            if (militaryCampaign == null)
            {
                return HttpNotFound();
            }
            return View(militaryCampaign);
        }

        // POST: MilitaryCampaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MilitaryCampaignID,CampaignName")] MilitaryCampaign militaryCampaign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(militaryCampaign).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(militaryCampaign);
        }

        // GET: MilitaryCampaigns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MilitaryCampaign militaryCampaign = db.MilitaryCampaigns.Find(id);
            if (militaryCampaign == null)
            {
                return HttpNotFound();
            }
            return View(militaryCampaign);
        }

        // POST: MilitaryCampaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MilitaryCampaign militaryCampaign = db.MilitaryCampaigns.Find(id);
            db.MilitaryCampaigns.Remove(militaryCampaign);
            db.SaveChanges();
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
    }
}
