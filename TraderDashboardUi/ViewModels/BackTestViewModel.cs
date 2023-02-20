using System;
using System.ComponentModel.DataAnnotations;

namespace TraderDashboardUi.ViewModels
{
    public class BackTestViewModel
    {
        [Required(ErrorMessage = "Select an Instrument")]
        public string Instrument { get; set; }
        [Required(ErrorMessage = "Select a Start Date")]
        public DateTime BackTestStartDate { get; set; }
        [Required(ErrorMessage = "Select a End Date")]
        public DateTime BackTestEndDate { get; set; }
    }
}
