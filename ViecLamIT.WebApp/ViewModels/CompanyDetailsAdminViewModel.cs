using System.Collections.Generic;
using ViecLamIT.Domain.Entities;

namespace ViecLamIT.WebApp.ViewModels
{
    public class CompanyDetailsAdminViewModel
    {
        // Tab 1: Thông tin công ty
        public string Name { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }

        // Thông tin người đại diện (Employer)
        public string RepresentativeName { get; set; }
        public string RepresentativeEmail { get; set; }
        public string RepresentativePhone { get; set; }


        // Tab 2: Thống kê hoạt động
        public int TotalJobs { get; set; }
        public int ApprovedJobs { get; set; }
        public int PendingJobs { get; set; }
        public int RejectedJobs { get; set; }
        public int TotalApplications { get; set; }
    }
}
