using System.Collections.Generic;

namespace ViecLamIT.WebApp.ViewModels
{
    public class ManageCompaniesViewModel
    {
        public List<CompanyAdminViewModel> PendingCompanies { get; set; }
        public List<CompanyAdminViewModel> VerifiedCompanies { get; set; }
    }
}