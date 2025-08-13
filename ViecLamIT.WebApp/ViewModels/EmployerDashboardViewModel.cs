using System.Collections.Generic;
using ViecLamIT.Domain.Entities;

namespace ViecLamIT.WebApp.ViewModels
{
    public class EmployerDashboardViewModel
    {
        public Company CompanyInfo { get; set; }
        public List<JobPosting> ApprovedJobs { get; set; }
        public List<JobPosting> PausedJobs { get; set; } // <-- THÊM DÒNG NÀY
        public List<JobPosting> PendingJobs { get; set; }
        public List<JobPosting> RejectedJobs { get; set; }
    }
}