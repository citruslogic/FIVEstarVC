﻿using DelegateDecompiler;
using FIVESTARVC.DAL;
using FIVESTARVC.Helpers;
using FIVESTARVC.Models;
using FIVESTARVC.ViewModels;
using Microsoft.Linq.Translations;
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
            
            var residents = db.Residents.AsNoTracking().AsEnumerable();

            if (!string.IsNullOrEmpty(searchString))
            {
                residents = residents.Where(s => CultureInfo.CurrentCulture.CompareInfo
                                    .IndexOf(s.ClearLastName, searchString, CompareOptions.IgnoreCase) >= 0
                                    || CultureInfo.CurrentCulture.CompareInfo
                                   .IndexOf(s.ClearFirstMidName, searchString, CompareOptions.IgnoreCase) >= 0) ;
            }

            switch (sortOrder)
            {
                case "name_desc":
                    residents = residents.OrderByDescending(s => s.ClearLastName);
                    break;
                case "ServiceBranch":
                    residents = residents.OrderBy(s => s.ServiceBranch);
                    break;
                case "ServiceBranch_desc":
                    residents = residents.OrderByDescending(s => s.ServiceBranch);
                    break;
                default:
                    residents = residents.AsEnumerable().OrderBy(s => s.ClearLastName);
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