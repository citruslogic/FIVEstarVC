using FIVESTARVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FIVESTARVC.ViewModels
{
    public class CustomEvent
    {

        public List<TempProgramEvent> programEvents { get; set; }
    }
}