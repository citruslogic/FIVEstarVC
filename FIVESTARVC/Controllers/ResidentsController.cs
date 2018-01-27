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
using PagedList;
using FIVESTARVC.ViewModels;

namespace FIVESTARVC.Controllers
{
    public class ResidentsController : Controller
    {
        private ResidentContext db = new ResidentContext();


        // GET: Residents
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.BranchSortParm = sortOrder == "ServiceBranch" ? "ServiceBranch_desc" : "ServiceBranch";
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes, "ProgramTypeID", "ProgramDescription");
           

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var residents = from s in db.Residents
                            select s;


            if (!String.IsNullOrEmpty(searchString))
            {
                residents = residents.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    residents = residents.OrderByDescending(s => s.LastName);
                    break;
                case "ServiceBranch":
                    residents = residents.OrderBy(s => s.ServiceBranch);
                    break;
                case "ServiceBranch_desc":
                    residents = residents.OrderByDescending(s => s.ServiceBranch);
                    break;
                default:
                    residents = residents.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(residents.ToPagedList(pageNumber, pageSize));

        }

        // GET: Residents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Resident resident = db.Residents.Find(id);

            if (resident == null)
            {
                return HttpNotFound();
            }
            return View(resident);
        }

        // GET: Residents/Create
        public ActionResult Create()
        {

            //Call Get Assigned Room to get available rooms//

            GetAssignedRoom();
      
            return View();
        }

        // POST: Residents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,ServiceBranch,Rank")] Resident resident)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Residents.Add(resident);
                    
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */ )
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }


            return View(resident);
        }

        // GET: Residents/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Resident resident = db.Residents
            .Include(c => c.MilitaryCampaigns)
            .Where(c => c.ResidentID == id)
            .Single();

            PopulateAssignedCampaignData(resident);

            if (resident == null)
            {
                return HttpNotFound();
            }


            return View(resident);
        }

       

        // POST: Residents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, string[] selectedCampaigns)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var residentToUpdate = db.Residents
                .Include(p => p.ProgramEvents)
                .Include(c => c.MilitaryCampaigns)
                .Where(c => c.ResidentID == id)
                .Single();

            if (TryUpdateModel(residentToUpdate, "",
               new string[] { "LastName", "FirstMidName", "ServiceBranch", "MilitaryCampaigns" }))
            {
                try
                {

                    UpdateResidentCampaigns(selectedCampaigns, residentToUpdate);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateAssignedCampaignData(residentToUpdate);
            return View(residentToUpdate);
        }

        private void PopulateAssignedCampaignData(Resident resident)
        {
            var allMilitaryCampaigns = db.MilitaryCampaigns;
            var residentCampaigns = new HashSet<int>(resident.MilitaryCampaigns.Select(c => c.MilitaryCampaignID));
            var viewModel = new List<AssignedCampaignData>();
            foreach (var militaryCampaign in allMilitaryCampaigns)
            {
                viewModel.Add(new AssignedCampaignData
                {
                    MilitaryCampaignID = militaryCampaign.MilitaryCampaignID,
                    MilitaryCampaign = militaryCampaign.CampaignName,
                    Assigned = residentCampaigns.Contains(militaryCampaign.MilitaryCampaignID)
                });
            }
            ViewBag.Campaigns = viewModel;
        }

        private void UpdateResidentCampaigns(string[] selectedCampaigns, Resident residentToUpdate)
        {
            if (selectedCampaigns == null)
            {
                residentToUpdate.MilitaryCampaigns = new List<MilitaryCampaign>();
                return;
            }

            var selectedCampaignsHS = new HashSet<string>(selectedCampaigns);
            var residentCampaigns = new HashSet<int>
                (residentToUpdate.MilitaryCampaigns.Select(c => c.MilitaryCampaignID));

            foreach (var campaign in db.MilitaryCampaigns)
            {
                if (selectedCampaignsHS.Contains(campaign.MilitaryCampaignID.ToString()))
                {
                    if (!residentCampaigns.Contains(campaign.MilitaryCampaignID))
                    {
                        residentToUpdate.MilitaryCampaigns.Add(campaign);
                    }
                }
                else
                {
                    if (residentCampaigns.Contains(campaign.MilitaryCampaignID))
                    {
                        residentToUpdate.MilitaryCampaigns.Remove(campaign);
                    }
                }
            }
        }

       

        // GET: Residents/Delete/5
        [HttpGet]
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Resident resident = db.Residents.Find(id);

            if (resident == null)
            {
                return HttpNotFound();
            }
            return View(resident);
        }

        // POST: Residents/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Resident resident = db.Residents.Find(id);
                db.Residents.Remove(resident);
                db.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }


        public PartialViewResult NewCenterEvent(int id, int program, DateTime startDate, DateTime? endDate, Boolean completed)
        {
            ProgramEvent programEvent = new ProgramEvent()
            {
                ResidentID = id,
                ProgramTypeID = program,
                StartDate = startDate,
                EndDate = endDate,
                Completed = completed
                            
            };

            db.ProgramEvents.Add(programEvent);

            return PartialView(programEvent);
        }

       private void GetAssignedRoom ()
        {
            //Initialize the AssignedRoom ViewModel//
            var AvailRoom = db.Rooms;
            
            var viewModel = new List<AssignedRoom>();

            //Loop through the rooms and check to see if IsOccupied is checked or not//
            //if not checked, add it to the viewmodel//
            foreach (var Rooms in AvailRoom)
            {
                if (Rooms.IsOccupied == false)
                { 
                    viewModel.Add(new AssignedRoom
                    {
                        RoomNum = Rooms.RoomNum,
                        IsOccupied = Rooms.IsOccupied
                    });
                }
            }
            ViewBag.AssignRoom = viewModel;
        }


    }
}
