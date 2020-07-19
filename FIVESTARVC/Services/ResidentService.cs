using FIVESTARVC.DAL;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using PagedList;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FIVESTARVC.Services
{
    public class ResidentService
    {
        public IPagedList<Resident> GetIndex(string searchString, string sortOrder, ResidentContext db, int? page)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            //var residents = db.Residents.AsNoTracking().AsEnumerable();
            var residents = db.Database.SqlQuery<Resident>("SELECT * FROM Person").ToList();


            if (!string.IsNullOrEmpty(searchString))
            {
                residents = residents.Where(s => CultureInfo.CurrentCulture.CompareInfo
                                    .IndexOf(s.ClearLastName, searchString, CompareOptions.IgnoreCase) >= 0
                                    || CultureInfo.CurrentCulture.CompareInfo
                                   .IndexOf(s.ClearFirstMidName, searchString, CompareOptions.IgnoreCase) >= 0).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    residents = residents.OrderByDescending(s => s.ClearLastName).ToList();
                    break;
                case "ServiceBranch":
                    residents = residents.OrderBy(s => s.ServiceBranch).ToList();
                    break;
                case "ServiceBranch_desc":
                    residents = residents.OrderByDescending(s => s.ServiceBranch).ToList();
                    break;
                default:
                    residents = residents.AsEnumerable().OrderBy(s => s.ClearLastName).ToList();
                    break;
            }

            return residents.ToPagedList(pageNumber, pageSize);
        }

        public Resident GetDetails(int? id, ResidentContext db)
        {
            Resident resident = db.Residents
                .Include("ProgramEvents")
                .Include("Benefit")
                .Where(r => r.ResidentID == id).SingleOrDefault();

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