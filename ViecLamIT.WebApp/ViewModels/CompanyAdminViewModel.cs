namespace ViecLamIT.WebApp.ViewModels
{
    public class CompanyAdminViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ApprovedJobsCount { get; set; }
        public int TotalApplicationsCount { get; set; }

        public bool IsVerified { get; set; } // <-- THÊM DÒNG NÀY
    }
}
