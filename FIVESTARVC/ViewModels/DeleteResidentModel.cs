using FIVESTARVC.Models;
using System.Collections.Generic;

namespace FIVESTARVC.ViewModels
{
    public class DeleteResidentModel
	{
        public bool ToDelete { get; set; }

        public bool ToRestore { get; set; }

        public int ResidentID { get; set; }

        public string Fullname { get; set; }
    }
}
