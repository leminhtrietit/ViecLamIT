using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViecLamIT.Domain.Entities
{
    public class JobApplication
    {
        public int Id { get; set; }

        // Khóa ngoại đến bảng JobPostings
        public int JobPostingId { get; set; }
        [ForeignKey("JobPostingId")]
        public virtual JobPosting JobPosting { get; set; }

        // Khóa ngoại đến bảng AspNetUsers (Người ứng tuyển)
        public string ApplicantId { get; set; }
        [ForeignKey("ApplicantId")]
        public virtual ApplicationUser Applicant { get; set; }

        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } // Ví dụ: "Chờ duyệt", "Đã xem"...
    }
}