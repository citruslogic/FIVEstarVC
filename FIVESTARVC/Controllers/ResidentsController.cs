using DelegateDecompiler;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.Services;
using FIVESTARVC.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace FIVESTARVC.Controllers
{
    //[Authorize]
    [Authorize(Roles = "RTS-Group")]
    public class ResidentsController : Controller
    {
        private readonly ResidentContext db = new ResidentContext();
        private readonly ResidentService residentService = new ResidentService();

        // GET: Residents
        [HttpGet]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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

            var residents = await Task.Run(() => residentService.GetIndex(searchString, sortOrder, db, page)).ConfigureAwait(false);

            return View(residents);
        }

        // GET: Residents/Details/5
        [HttpGet]
        public ActionResult Details(int? id, int fromPage = 1)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dateFirstAdmitted = db.ProgramEvents
                                            .AsNoTracking()
                                            .Include(r => r.Resident).Where(r => r.ResidentID == id)
                                            .Include(t => t.ProgramType).Where(p => p.ProgramTypeID == 2 || p.ProgramTypeID == 1).ToList()
                                            .OrderBy(d => d.ClearStartDate.Computed())
                                            .Select(s => s.ClearStartDate.Computed())
                                            .FirstOrDefault().ToLongDateString();

            if (!dateFirstAdmitted.Contains("0001"))
            {
                ViewBag.DateFirstAdmitted = dateFirstAdmitted;
            }
            else
            {
                // draw from the re-admittance date, ask for a readmission if there are no admittance dates. 
                ViewBag.DateFirstAdmitted = db.ProgramEvents
                                            .AsNoTracking()
                                            .Include(r => r.Resident).Where(r => r.ResidentID == id)
                                            .Include(t => t.ProgramType).Where(p => p.ProgramTypeID == 3).ToList()
                                            .OrderBy(d => d.ClearStartDate.Computed())
                                            .Select(s => s.ClearStartDate.Computed())
                                            .FirstOrDefault().ToLongDateString() + " (readmit)";
            }

            var resident = residentService.GetDetails(id, db);
            resident.FromPage = fromPage;
            var assignedCampaigns = residentService.PopulateAssignedCampaignData(resident, db);

            if (resident == null)
            {
                return HttpNotFound();
            }

            ViewBag.Campaigns = assignedCampaigns;

            return View(resident);
        }

        // GET: Residents/Create
        [HttpGet]
        public ActionResult Create()
        {
            ResidentIncomeModel residentIncomeModel = new ResidentIncomeModel();

            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", residentIncomeModel.StateTerritoryID);

            ViewBag.AdmissionType = new SelectList(db.ProgramTypes
                .Where(t => t.EventType == EnumEventType.ADMISSION), "ProgramTypeID", "ProgramDescription", 2);

            ViewBag.ReferralID = new SelectList(db.Referrals, "ReferralID", "ReferralName", 1);

            var allMilitaryCampaigns = db.MilitaryCampaigns;
            var viewModel = new List<AssignedCampaignData>();

            foreach (var militaryCampaign in allMilitaryCampaigns)
            {
                viewModel.Add(new AssignedCampaignData
                {
                    MilitaryCampaignID = militaryCampaign.MilitaryCampaignID,
                    MilitaryCampaign = militaryCampaign.CampaignName,
                    Assigned = false
                });
            }

            ViewBag.Campaigns = viewModel.OrderBy(i => i.MilitaryCampaign).ToList();

            return View(new ResidentIncomeModel());
        }

        // POST: Residents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResidentIncomeModel residentIncomeModel, string[] selectedCampaigns,
            int AdmissionType)
        {
            ViewBag.AdmissionType = new SelectList(db.ProgramTypes
                .Where(t => t.EventType == EnumEventType.ADMISSION), "ProgramTypeID", "ProgramDescription", AdmissionType);

            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", residentIncomeModel.StateTerritoryID);

            ViewBag.ReferralID = new SelectList(db.Referrals, "ReferralID", "ReferralName", residentIncomeModel.ReferralID);

            var allMilitaryCampaigns = db.MilitaryCampaigns;
            var viewModel = new List<AssignedCampaignData>();

            foreach (var militaryCampaign in allMilitaryCampaigns)
            {
                viewModel.Add(new AssignedCampaignData
                {
                    MilitaryCampaignID = militaryCampaign.MilitaryCampaignID,
                    MilitaryCampaign = militaryCampaign.CampaignName,
                    Assigned = false
                });
            }

            ViewBag.Campaigns = viewModel.OrderBy(i => i.MilitaryCampaign).ToList();

            Resident resident = new Resident
            {
                ClearFirstMidName = residentIncomeModel.FirstMidName,
                ClearLastName = residentIncomeModel.LastName,
                Gender = residentIncomeModel.Gender,
                Ethnicity = residentIncomeModel.Ethnicity,
                Religion = residentIncomeModel.Religion,
                ClearBirthdate = residentIncomeModel.Birthdate,
                ServiceBranch = residentIncomeModel.ServiceBranch,
                MilitaryDischarge = residentIncomeModel.DischargeStatus,
                InVetCourt = residentIncomeModel.InVetCourt,
                IsNoncombat = residentIncomeModel.IsNoncombat,
                StateTerritoryID = residentIncomeModel.StateTerritoryID,
                ReferralID = residentIncomeModel.ReferralID,
                Note = residentIncomeModel.Note,
                MilitaryCampaigns = new List<MilitaryCampaign>(),
                ProgramEvents = new List<ProgramEvent>(),
                Benefit = new Benefit(),
            };

            if (string.IsNullOrEmpty(residentIncomeModel.ReferralOther))
            {
                resident.OptionalReferralDescription = residentIncomeModel.ReferralOther;
            }


            Benefit benefit = new Benefit
            {
                DisabilityPercentage = residentIncomeModel.DisabilityPercentage,
                DisabilityAmount = residentIncomeModel.DisabilityAmount,
                SSI = residentIncomeModel.SSI,
                SSDI = residentIncomeModel.SSDI,
                FoodStamp = residentIncomeModel.FoodStamp,
                OtherDescription = residentIncomeModel.OtherDescription,
                Other = residentIncomeModel.Other,
                TotalBenefitAmount = residentIncomeModel.TotalBenefitAmount
            };

            try
            {
                if (ModelState.IsValid)
                {
                    if (!IsDuplicateResident(residentIncomeModel))
                    {
                        db.Residents.Add(resident);

                        var admitEvent = new ProgramEvent
                        {
                            ProgramTypeID = AdmissionType,
                            ClearStartDate = residentIncomeModel.AdmitDate,

                        };

                        resident.ProgramEvents.Add(admitEvent);

                        db.Benefits.Add(benefit);
                        resident.Benefit = benefit;

                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", "This resident may already exist in the system. Please check their full name, birthdate, and service branch. Try to add another resident to continue.");
                        return View(residentIncomeModel);
                    }
                }
            }
            catch (DataException dex)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError(dex.Message, "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            if (TryUpdateModel(residentIncomeModel, "",
                 new string[] { "LastName", "FirstMidName", "Ethnicity", "StateTerritoryID", "ReferralID", "Gender", "Religion", "ClearBirthdate", "ServiceBranch", "Note", "IsNoncombat", "InVetCourt",
                     "Benefit", "MilitaryCampaigns", "TotalBenefitAmount" }))
            {
                try
                {
                    UpdateResidentCampaigns(selectedCampaigns, resident);
                    db.SaveChanges();

                    TempData["UserMessage"] = residentIncomeModel.LastName + " has been admitted into your center.  ";

                    return RedirectToAction("Manage", new RouteValueDictionary(
                        new { controller = "ProgramEvents", action = "Manage", Id = resident.ResidentID, FromPage = 1 }));
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(residentIncomeModel);
        }

        public bool IsDuplicateResident(ResidentIncomeModel resident)
        {

            /* Check for the possible pre-existence of the resident in the system. */
            if (db.Residents.AsNoTracking().ToList().Any(r => r.ClearFirstMidName.Contains(resident.FirstMidName)
                && r.ClearLastName.Contains(resident.LastName)
                && r.ClearBirthdate?.Date == resident.Birthdate.Date
                && r.ServiceBranch == resident.ServiceBranch))
            {

                // Found a match.
                return true;

            }

            return false;
        }

        // GET: Residents/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id, int? fromPage)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Resident resident = db.Residents
                .Include(c => c.MilitaryCampaigns)
                .Include(b => b.Benefit)
                .Include(s => s.StateTerritory)
                .Include(r => r.Referral)
                .Include(p => p.ProgramEvents.Select(e => e.ProgramType))
                .Where(c => c.ResidentID == id)
                .Single();


            ViewBag.Campaigns = residentService.PopulateAssignedCampaignData(resident, db).OrderBy(i => i.MilitaryCampaign).ToList();
            ViewBag.DischargeInfo = resident.ProgramEvents
                                        .Where(i => i.ProgramType.EventType == EnumEventType.DISCHARGE)
                                        .OrderByDescending(i => i.ProgramEventID).FirstOrDefault(); 

            if (resident == null)
            {
                return HttpNotFound();
            }

            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", resident.StateTerritoryID);
            ViewBag.ReferralID = new SelectList(db.Referrals, "ReferralID", "ReferralName", resident.ReferralID);

            resident.FromPage = fromPage;
            return View(resident);
        }

        // POST: Residents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, int? fromPage, string[] selectedCampaigns, bool? Readmit, string readmitDate)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Resident residentToUpdate = db.Residents
                .Include(c => c.MilitaryCampaigns)
                .Include(b => b.Benefit)
                .Include(r => r.Referral)
                .Where(c => c.ResidentID == id)
                .Single();

            ViewBag.DischargeInfo = residentToUpdate.ProgramEvents
                                        .Where(i => i.ProgramType.EventType == EnumEventType.DISCHARGE)
                                        .OrderByDescending(i => i.ProgramEventID).FirstOrDefault();

            ViewBag.StateTerritoryID = new SelectList(db.States, "StateTerritoryID", "State", residentToUpdate.StateTerritoryID);
            ViewBag.ReferralID = new SelectList(db.Referrals, "ReferralID", "ReferralName", residentToUpdate.ReferralID);
            ViewBag.Campaigns = residentService.PopulateAssignedCampaignData(residentToUpdate, db).OrderBy(i => i.MilitaryCampaign).ToList();

            if (TryUpdateModel(residentToUpdate, "",
               new string[] { "ClearLastName", "ClearFirstMidName", "Gender", "Religion", "Ethnicity", "StateTerritoryID", "ReferralID", "AgeAtRelease",
                   "OptionalReferralDescription", "ClearBirthdate", "ServiceBranch", "Note", "InVetCourt", "IsNoncombat", "Benefit", "MilitaryCampaigns",
                   "TotalBenefitAmount" }))
            {
                try
                {
                    UpdateResidentCampaigns(selectedCampaigns, residentToUpdate);

                    if (Readmit.HasValue)
                    {
                        if (Readmit == true)
                        {
                            if (string.IsNullOrEmpty(readmitDate))
                            {
                                ModelState.AddModelError("", "Readmission Date is required if you are readmitting this resident.");
                                return View(residentToUpdate);
                            }
                            var date = DateTime.Parse(readmitDate);
                           
                            var ev = residentToUpdate.ProgramEvents
                                .LastOrDefault(i => i.ProgramType.EventType == EnumEventType.DISCHARGE);    // Emergency Discharge to be readmited.

                            if (date < ev.ClearStartDate)
                            {
                                ModelState.AddModelError("", "Readmission date cannot be prior to last discharge date, " + ev.ClearStartDate.ToShortDateString());
                                return View(residentToUpdate);
                            }

                            ev.ClearEndDate = date;
                            residentToUpdate.AgeAtRelease = 0;      // reset the age at release for the readmitted resident.

                            db.Entry(ev).State = EntityState.Modified;

                            var readmitEvent = new ProgramEvent
                            {
                                ProgramTypeID = 3,
                                ClearStartDate = date
                            };

                            residentToUpdate.ProgramEvents.Add(readmitEvent);
                        }
                    }

                    db.SaveChanges();

                    TempData["UserMessage"] = residentToUpdate.ClearLastName + " has been updated.  ";

                    return RedirectToAction("Index", new { page = fromPage });
                }
                catch (DataException dex)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError(dex.Message, "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            residentToUpdate.FromPage = fromPage;
            return View(residentToUpdate);
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

        [HttpGet]
        public ActionResult AddReferral()
        {
            Referral referral = new Referral();

            return PartialView(referral);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReferral(Referral model)
        {
            if (ModelState.IsValid)
            {
                db.Referrals.Add(model);
                db.SaveChanges();
                TempData["UserMessage"] = "A new referral option has been added.  ";

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save the new referral option. Check the form input and try again.");
            }

            return PartialView(model);
        }
        // GET: Residents/AddCampaign/5
        [HttpGet]
        public ActionResult AddCampaign()
        {
            MilitaryCampaign NewCampaign = new MilitaryCampaign();

            return PartialView(NewCampaign);
        }

        // POST: Residents/AddCampaign/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCampaign(MilitaryCampaign model)
        {
            if (ModelState.IsValid)
            {
                db.MilitaryCampaigns.Add(model);
                db.SaveChanges();
                TempData["UserMessage"] = "A new campaign has been added.  ";

                return RedirectToAction("Index");
            }
            else
            {
                // Failed
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return PartialView(model);
        }

        [HttpGet]
        public JsonResult IsCampaignNameExist(string CampaignName, int? MilitaryCampaignID)
        {
            var validateName = db.MilitaryCampaigns.FirstOrDefault
                                (x => x.CampaignName == CampaignName && x.MilitaryCampaignID != MilitaryCampaignID);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Residents/Delete
        [HttpGet]
        public ActionResult ConfirmDelete(int? fromPage)
        {
            var selectableResidents = new List<DeleteResidentModel>();

            var residentsMarkedForDeletion = db.Residents
                .AsNoTracking()
                .ToList()
                .Where(i => i.ToDelete)
                .ToList();

            foreach (var residentToDelete in residentsMarkedForDeletion)
            {
                selectableResidents.Add(new DeleteResidentModel
                {
                    ResidentID = residentToDelete.ResidentID,
                    Fullname = residentToDelete.Fullname
                });
            }

            ViewData["page"] = fromPage;
            return View(selectableResidents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(List<DeleteResidentModel> residents)
        {
            foreach (var resident in residents)
            {
                var residentToDelete = db.Residents.Find(resident.ResidentID);

                if (residentToDelete != null && resident.ToDelete == true)
                {
                    db.Entry(residentToDelete).State = EntityState.Deleted;
                } else if (residentToDelete != null && resident.ToRestore == true)
                {
                    residentToDelete.ToDelete = false;
                } 
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Residents/Undelete
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var residentToDelete = db.Residents
                .Include(p => p.ProgramEvents)
                .Where(r => r.ResidentID == id)
                .Single();

            return PartialView("Delete", residentToDelete);
        }

        // POST: Residents/Readmit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Resident model)
        {
            try
            {
                var residentToDelete = db.Residents.Find(model.ResidentID);

                if (residentToDelete != null)
                {
                    residentToDelete.ToDelete = true;
                    db.SaveChanges();
                    TempData["UserMessage"] = model.Fullname + " has been marked for deletion from your center.  ";
                } else
                {
                    TempData["UserMessage"] = "Could not find the resident, " + model.Fullname + ", in the system.";
                    return RedirectToAction("Index");
                }

                
            }
            catch (DataException/* dex */)
            {
                TempData["UserMessage"] = "Failed to mark the resident for deletion from the center.";

                return RedirectToAction("Delete", new { id = model.ResidentID, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Undelete(int? id)
        {
            try
            {
                var residentToUndelete = db.Residents.Find(id);

                if (residentToUndelete != null)
                {
                    residentToUndelete.ToDelete = false;
                    db.SaveChanges();
                }
            } catch (DataException /* dex */)
            {
                TempData["UserMessage"] = "Could not unmark resident for deletion from the center.";

                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }


        // GET: Residents/Discharge/5
        [HttpGet]
        public ActionResult Discharge(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var residentToDischarge = db.Residents
                .Include(p => p.ProgramEvents.Select(j => j.ProgramType))
                .Where(r => r.ResidentID == id)
                .Single();

            var dischargeModel = new DischargeViewModel
            {
                ResidentID = residentToDischarge.ResidentID,
                DischargeDate = null,
                FullName = residentToDischarge.Fullname,
                Birthdate = residentToDischarge.ClearBirthdate?.ToShortDateString(),
                Note = residentToDischarge.Note,
                ServiceBranch = residentToDischarge.ServiceBranch,
                LastAdmitted = residentToDischarge.ProgramEvents.LastOrDefault(i => i.ProgramType.EventType == EnumEventType.ADMISSION).ClearStartDate.Date
            };

            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes
                .Where(t => t.EventType == EnumEventType.DISCHARGE), "ProgramTypeID", "ProgramDescription");

            return View("Discharge", dischargeModel);
        }

        // POST: Residents/Discharge/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Discharge(DischargeViewModel model)
        {
            ViewBag.ProgramTypeID = new SelectList(db.ProgramTypes
                .Where(t => t.EventType == EnumEventType.DISCHARGE), "ProgramTypeID", "ProgramDescription");


            if (ModelState.IsValid)
            {
                Resident residentToDischarge = db.Residents
                .Include(p => p.ProgramEvents.Select(i => i.ProgramType))
                .Where(r => r.ResidentID == model.ResidentID)
                .Single();

                // To close admission events
                var ev = residentToDischarge.ProgramEvents
                    .LastOrDefault(i => i.ProgramType.EventType == EnumEventType.ADMISSION);

                // Set age at release for resident for reporting more accurate cumulative age.
                residentToDischarge.AgeAtRelease = residentToDischarge.Age;
                    
                ev.ClearEndDate = model.DischargeDate;
                db.Entry(ev).State = EntityState.Modified;

                // Discharge event
                residentToDischarge.ProgramEvents.Add(new ProgramEvent
                {
                    ProgramTypeID = model.ProgramTypeID,
                    ClearStartDate = model.DischargeDate.Value
                });

                if (residentToDischarge.ActualDaysStayed == null)
                {
                    residentToDischarge.ActualDaysStayed = residentToDischarge.DaysInCenter;
                }
                else
                {
                    residentToDischarge.ActualDaysStayed += residentToDischarge.DaysInCenter;
                }

                db.SaveChanges();
                TempData["UserMessage"] = residentToDischarge.ClearLastName + " has been discharged from your center.  ";
                return RedirectToAction("Index");
            } 

            return View("Discharge", model);
        }

        // GET: Residents/Readmit/5
        [HttpGet]
        public ActionResult Readmit(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Discharge failed. Try again, and if the problem persists see your system administrator.";
            }

            var residentToReadmit = db.Residents
                .Include(p => p.ProgramEvents)
                .Where(r => r.ResidentID == id)
                .Single();

            if (residentToReadmit == null)
            {
                return HttpNotFound();
            }

            return PartialView("_Readmit", residentToReadmit);
        }

        // POST: Residents/Readmit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Readmit(int id, string ReadmitDate)
        {
            var date = DateTime.Parse(ReadmitDate);

            try
            {
                Resident residentToReadmit = db.Residents
                .Include(p => p.ProgramEvents)
                .Where(r => r.ResidentID == id)
                .Single();

                // To close discharge events
                var ev = residentToReadmit.ProgramEvents
                    .Where(i => i.ProgramType?.EventType == EnumEventType.DISCHARGE)
                    .LastOrDefault();
                if (ev != null)
                {
                    ev.ClearEndDate = DateTime.Parse(ReadmitDate);
                    db.Entry(ev).State = EntityState.Modified;
                }

                var admitEvent = new ProgramEvent
                {
                    ProgramTypeID = 3,
                    ClearStartDate = date
                };

                residentToReadmit.ProgramEvents.Add(admitEvent);
                residentToReadmit.AgeAtRelease = 0;     // resident is readmitted, reset Age At Release.

                db.SaveChanges();

                TempData["UserMessage"] = residentToReadmit.ClearLastName + " has been readmitted from your center.  ";
            }
            catch (DataException/* dex */)
            {
                TempData["UserMessage"] = "Failed to readmit the resident to the center.";

                return RedirectToAction("Readmit", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult GetRegionName(string id)
        {
            var RegionName = db.States.Find(int.Parse(id)).Region;

            return Json(RegionName, JsonRequestBehavior.AllowGet);
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
