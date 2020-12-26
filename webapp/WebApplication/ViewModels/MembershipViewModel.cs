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

        public MembershipModel FreeMembershipModel =>
            MembershipModels.FirstOrDefault(e => e.MembershipOption.IsFree);

        public int MonthlyMaxNumberOfProfileReadings =>
            MonthlyMembershipModel?.MembershipOption?.MaxNumberOfProfileReadings ?? 50;

        public int MonthlyMaxNumberOfCompatibilityReadings =>
            MonthlyMembershipModel?.MembershipOption?.MaxNumberOfCompatibilityReadings ?? 20;

        public int FreeMaxNumberOfProfileReadings =>
            FreeMembershipModel?.MembershipOption?.MaxNumberOfProfileReadings ?? 5;

        public int FreeMaxNumberOfCompatibilityReadings =>
            FreeMembershipModel?.MembershipOption?.MaxNumberOfCompatibilityReadings ?? 2;
    }
}