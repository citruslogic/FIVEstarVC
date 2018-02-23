﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using FIVESTARVC.DAL;
using DelegateDecompiler;

namespace FIVESTARVC.Models
{
   

    public class Resident
    {
        private ResidentContext db = new ResidentContext();

        public int ResidentID { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }
        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Birthdate { get; set; }
        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }

        [Display(Name = "In Veterans Court?")]
        public Boolean InVetCourt { get; set; }
        [Display(Name = "Room Number")]
        [ForeignKey("Room")]
        public int? RoomID { get; set; }
        [Display(Name = "Note")]
        [StringLength(150)]
        public string Note { get; set; }

        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual ICollection<ProgramEvent> ProgramEvents { get; set; }
        public virtual Room Room { get; set; }

        [ForeignKey("Benefit")]
        public int? BenefitID { get; set; }

        public virtual Benefit Benefit { get; set; }


        /* return the age of a veteran (in years) and do not store in the database. */
        public int Age
        { 
            get
            {
                TimeSpan span = DateTime.Now - Birthdate.GetValueOrDefault(DateTime.Now);
                DateTime age = DateTime.MinValue + span;

                return age.Year - 1;
            }
        }

        //[Computed]
        public Boolean IsCurrent()
        {
                var current = db.ProgramEvents;

                int ID = ResidentID;

                Boolean internalBool = false;

                foreach (var ProgramEvent in current)
                {
                    if (ID == ProgramEvent.ResidentID)
                    {
                        if (ProgramEvent.ProgramTypeID == 7 //admission
                        || ProgramEvent.ProgramTypeID == 9 //re-admit
                        || ProgramEvent.ProgramTypeID == 5)
                        {
                            internalBool = true;
                        }

                        if (ProgramEvent.ProgramTypeID == 2 //graduation
                        || ProgramEvent.ProgramTypeID == 12 //discharge
                        || ProgramEvent.ProgramTypeID == 13 //discharge
                        || ProgramEvent.ProgramTypeID == 14)
                        {
                            internalBool = false;
                        }
                    }
                }
                return internalBool;
            }
        }

    }

