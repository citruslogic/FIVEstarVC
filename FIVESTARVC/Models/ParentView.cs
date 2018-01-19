using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models

    //This model is in order to access different models from the same view.  The create view uses this model to input data to
    //multiple classes.
{
    public class ParentView
    {
        public Resident Resident { get; set; }


        public MilitaryCampaign MilitaryCampaign { get; set; }

        public Event Event { get; set; }
        
        public Room Room { get; set; }


    }
}