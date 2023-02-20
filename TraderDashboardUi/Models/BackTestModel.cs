using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TraderDashboardUi.Models
{
    public class BackTestModel
    {
        [Required(ErrorMessage = "Select an Instrument")]
        public string Instrument { get; set; }
        public List<SelectListItem> Instruments { get; set; }
        [Required]
        [Display(Name = "BackTestStartDate")]
        public DateTime BackTestStartDate { get; set; }
        [Required]
        [Display(Name = "BackTestEndDate")]
        public DateTime BackTestEndDate { get; set; }
    }
}
