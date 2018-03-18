using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using FIVESTARVC.DAL;
using FIVESTARVC.Validators;
using DelegateDecompiler;
using System.Globalization;

namespace FIVESTARVC.Models
{
    public class StateTerritory
    {
        [Key]
        public int StateTerritoryID { get; set;}

        [Display(Name = "State or Territory")]
        public string State { get; set; }
        public string Region { get; set; }


    }
}