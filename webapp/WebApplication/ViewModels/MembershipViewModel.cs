using K9.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.ViewModels
{
    public class MembershipViewModel
    {
        public List<MembershipModel> MembershipModels { get; set; }

        public MembershipModel MonthlyMembershipModel =>
            MembershipModels.FirstOrDefault(e => e.MembershipOption.IsMonthly);

        public int MaxNumberOfProfileReadings =>
            MonthlyMembershipModel?.MembershipOption?.MaxNumberOfProfileReadings ?? 50;

        public int MaxNumberOfCompatibilityReadings =>
            MonthlyMembershipModel?.MembershipOption?.MaxNumberOfCompatibilityReadings ?? 20;
    }
}