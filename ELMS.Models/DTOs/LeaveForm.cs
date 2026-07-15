using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ELMS.Commons.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ELMS.Models.DTOs
{
    public class LeaveForm
    {
        public LeaveForm()
        {
            LeaveTypeist = Enum.GetValues(typeof(LeaveType)).Cast<LeaveType>()
                .Select(x => new SelectListItem
                {
                    Text = x.ToString(),
                    Value = ((int)x).ToString()
                }
                );
        }
        public Int64? LeaveId { get; set; }

        [Required(ErrorMessage = "Please select leave type.")]
        public LeaveType? LeaveType { get; set; }   // Selected Role
        public IEnumerable<SelectListItem> LeaveTypeist { get; set; }
        [Required(ErrorMessage = "Please slect date.")]
        public DateTime? StartDate { get; set; } 

        [Required(ErrorMessage = "Please slect date.")]
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "Rease is required.")]
        public string Reason { get; set; }
       
        public IFormFile? Attchement { get; set; }
    }
}
