using DelegateDecompiler;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FIVESTARVC.Services
{
    public class ResidentService
    {
        public List<Resident> GetIndex(string searchString, string currentFilter, string sortOrder, int? page, ResidentContext db)
        {

            var residents = (from s in db.Residents
                             select s).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                residents = residents.Where(s => CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (s.ClearLastName, searchString, CompareOptions.IgnoreCase) >= 0
                                   || CultureInfo.CurrentCulture.CompareInfo.IndexOf
                                   (s.ClearFirstMidName, searchString, CompareOptions.IgnoreCase) >= 0).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    residents = residents.OrderBy(s => s.ClearLastName.Computed()).ToList();
                    break;
                case "ServiceBranch":
                    residents = residents.OrderBy(s => s.ServiceBranch).ToList();
                    break;
                case "ServiceBranch_desc":
                    residents = residents.OrderByDescending(s => s.ServiceBranch).ToList();
                    break;
                default:
                    residents = residents.OrderByDescending(s => s.ResidentID).ToList();
                    break;
            }

            return residents;
        }

        public Resident GetDetails(int? id, ResidentContext db)
        {
           
            Resident resident = db.Residents
                .Include("ProgramEvents")
                .Include("Benefit")
                .Where(r => r.ResidentID == id).Single();

            return resident;
        }

        public List<AssignedCampaignData> PopulateAssignedCampaignData(Resident resident, ResidentContext db)
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

            return viewModel;
        }
    }
}