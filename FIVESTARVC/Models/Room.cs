using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIVESTARVC.Models
{
    public class Room
    {
        
        public int RoomID { get; set; }

        [Display(Name = "Room Number")]
        public int RoomNum { get; set; }

        [Display(Name = "Is Occupied")]
        public bool IsOccupied { get; set; }
        public string WingName { get; set; }

        //public IList<Room> RoomList { get; set; }
        //public IList<SelectListItem> RoomListSelectListItem 
        //    {
        //         get
           
        //        {
        //           var list = (from item in RoomList
        //                       select new SelectListItem()
        //                       {
        //                           Text = item.customerID.ToString(CultureInfo.InvariantCulture),
        //                           Value = item.selectedCustomer.ToString(CultureInfo.InvariantCulture)
        //                       }).ToList();
        //                return list;
        //    }
        //        set{}
        //} 
    }
}

