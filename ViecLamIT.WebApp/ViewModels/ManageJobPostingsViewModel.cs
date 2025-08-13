using System.Collections.Generic;
using ViecLamIT.Domain.Entities;

namespace ViecLamIT.WebApp.ViewModels
{
    public class ManageJobPostingsViewModel
    {
        public List<JobPosting> PendingJobs { get; set; }
        public List<JobPosting> ApprovedJobs { get; set; }
        public List<JobPosting> RejectedJobs { get; set; }
    }
}
