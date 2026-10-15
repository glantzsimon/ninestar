using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class MailingListService : BaseService, IMailingListService
    {
        private readonly IRepository<MailingList> _mailingListsRepository;
        private readonly IRepository<MailingListUser> _mailingListUsersRepository;
        private readonly IRepository<UserMembership> _userMembershipsRepository;
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;

        public MailingListService(INineStarKiBasePackage my, IRepository<MailingList> mailingListsRepository, IRepository<MailingListUser> mailingListUsersRepository,
            IRepository<UserMembership> userMembershipsRepository, IRepository<MembershipOption> membershipOptionsRepository)
            : base(my)
        {
            _mailingListsRepository = mailingListsRepository;
            _mailingListUsersRepository = mailingListUsersRepository;
            _userMembershipsRepository = userMembershipsRepository;
            _membershipOptionsRepository = membershipOptionsRepository;
        }

        public MailingList Find(int id, bool includeUsers = false)
        {
            MailingList list = null;

            if (id <= MailingList.AllUsersId)
            {
                list = GetAutoList(id);
            }
            else
            {
                list = _mailingListsRepository.Find(id);
                if (includeUsers)
                    list.Users = GetUsersForMailingList(id);
            }
            
            return list;
        }

        public List<MailingList> List(bool includeUsers = false)
        {
            var lists = _mailingListsRepository.List()
                .OrderBy(e => e.Name)
                .Select(e =>
                {
                    if (includeUsers)
                        e.Users = GetUsersForMailingList(e.Id);

                    return e;
                })
                .ToList();

            return lists;
        }

        public List<MailingList> ListAll()
        {
            var lists = _mailingListsRepository.List()
                .OrderBy(e => e.Name)
                .Select(e =>
                {
                    e.Users = GetUsersForMailingList(e.Id);
                    return e;
                })
                .ToList();

            lists.InsertRange(0, GetAutoLists());

            return lists;
        }

        public List<User> GetUsersForMailingList(int id)
        {
            var userIds = _mailingListUsersRepository.Find(e => e.MailingListId == id).Select(e => e.UserId).ToList();
            return My.UsersRepository.Find(e => userIds.Contains(e.Id))
                .OrderBy(e => e.FirstName).ThenBy(e => e.LastName)
                .ToList();
        }

        public List<ListItem> GetMailingListListItems()
        {
            return new List<ListItem>(GetAutoLists(false).Concat(List()).Select(e =>
            {
                return new ListItem(e.Id, e.Name, e.Name);
            }));
        }

        private List<MailingList> GetAutoLists(bool includeUsers = true)
        {
            var ids = new[]
            {
                MailingList.FreeUsersId, MailingList.PaidUsersId,
                MailingList.BaseUsersId + (int)MembershipOption.ESubscriptionType.WeeklyPlatinum,
                MailingList.BaseUsersId + (int)MembershipOption.ESubscriptionType.MonthlyPlatinum,
                MailingList.BaseUsersId + (int)MembershipOption.ESubscriptionType.AnnualPlatinum,
                MailingList.BaseUsersId + (int)MembershipOption.ESubscriptionType.LifeTimePlatinum,
                MailingList.AllUsersId
            };
            return ids.Select(id => includeUsers ? GetAutoList(id) : new MailingList
            {
                Id = id, Name = GetAutoListName(id), IsSystemStandard = true
            }).ToList();
        }

        private MailingList GetAutoList(int id)
        {
            if (id == MailingList.FreeUsersId) return GetAllUsersOnFreeMembership();
            if (id == MailingList.PaidUsersId) return GetAllUsersOnPaidMembership();
            if (id == MailingList.AllUsersId) return GetAllUsers();
            foreach (var type in new[]
            {
                MembershipOption.ESubscriptionType.WeeklyPlatinum,
                MembershipOption.ESubscriptionType.MonthlyPlatinum,
                MembershipOption.ESubscriptionType.AnnualPlatinum,
                MembershipOption.ESubscriptionType.LifeTimePlatinum
            })
            {
                if (id == MailingList.BaseUsersId + (int)type)
                    return GetAllUsersPerSubscriptionType(type);
            }
            return null;
        }

        private static string GetAutoListName(int id)
        {
            if (id == MailingList.FreeUsersId) return "Users on Free Membership";
            if (id == MailingList.PaidUsersId) return "Users on Full Membership";
            if (id == MailingList.AllUsersId) return "All Users";
            var type = (MembershipOption.ESubscriptionType)(id - MailingList.BaseUsersId);
            return $"Users on {type.ToString().SplitOnCapitalLetter()}";
        }

        private MailingList GetAllUsers()
        {
            return new MailingList
            {
                Id = MailingList.AllUsersId,
                Name = GetAutoListName(MailingList.AllUsersId),
                IsSystemStandard = true,
                Users = My.UsersRepository.List()
                    .OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList()
            };
        }

        private MailingList GetAllUsersOnPaidMembership()
        {
            var listTimeMembership =
                _membershipOptionsRepository.Find(e => e.SubscriptionType == MembershipOption.ESubscriptionType.LifeTimePlatinum).FirstOrDefault();

            var fullMembershipIds = _membershipOptionsRepository.Find(e => e.SubscriptionType > MembershipOption.ESubscriptionType.Free)
                .Select(e => e.Id)
                .ToList();

            var paidUserMemberships = _userMembershipsRepository.Find(
                e => fullMembershipIds.Contains(e.MembershipOptionId) &&
                     (e.StartsOn <= DateTime.Today && DateTime.Today <= e.EndsOn || e.MembershipOptionId == listTimeMembership.Id) && !e.IsDeactivated).ToList();

            var paidUserIds = paidUserMemberships.Select(e => e.UserId).ToList();

            return new MailingList
            {
                Id = MailingList.PaidUsersId,
                Name = GetAutoListName(MailingList.PaidUsersId),
                IsSystemStandard = true,
                Users = My.UsersRepository.Find(e => paidUserIds.Contains(e.Id))
                    .OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList()
            };
        }

        private MailingList GetAllUsersPerSubscriptionType(MembershipOption.ESubscriptionType subscriptionType)
        {
            var membership =
                _membershipOptionsRepository.Find(e => e.SubscriptionType == subscriptionType).FirstOrDefault();

            var membershipIds = _membershipOptionsRepository.Find(e => e.SubscriptionType > MembershipOption.ESubscriptionType.Free)
                .Select(e => e.Id)
                .ToList();

            var subscribedMembers = _userMembershipsRepository.Find(
                e => membershipIds.Contains(e.MembershipOptionId) &&
                     (e.StartsOn <= DateTime.Today && DateTime.Today <= e.EndsOn || e.MembershipOptionId == membership.Id) && !e.IsDeactivated).ToList();

            var userIds = subscribedMembers.Select(e => e.UserId).ToList();

            return new MailingList
            {
                Id = MailingList.BaseUsersId + (int)subscriptionType,
                Name = GetAutoListName(MailingList.BaseUsersId + (int)subscriptionType),
                IsSystemStandard = true,
                Users = My.UsersRepository.Find(e => userIds.Contains(e.Id))
                    .OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList()
            };
        }

        private MailingList GetAllUsersSignedUpInLastTwoWeeks()
        {
            var freeMembership =
                _membershipOptionsRepository.Find(e => e.SubscriptionType == MembershipOption.ESubscriptionType.Free).FirstOrDefault();
            
            var paidUserIds = GetAllUsersOnPaidMembership().Users.Select(e => e.Id).ToList();

            var signedUpInLastTwoWeeks = _userMembershipsRepository.Find(
                e => !paidUserIds.Contains(e.UserId) && e.MembershipOptionId == freeMembership.Id &&
                     e.StartsOn >= DateTime.Today.AddDays(-14) && DateTime.Today <= e.EndsOn && !e.IsDeactivated).ToList();

            var userIds = signedUpInLastTwoWeeks.Select(e => e.UserId).ToList();

            return new MailingList
            {
                Id = MailingList.SignedUpInLastTwoWeeksId,
                Name = "Users Signed Up in Last Two Weeks",
                IsSystemStandard = true,
                Users = My.UsersRepository.Find(e => userIds.Contains(e.Id))
                    .OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList()
            };
        }

        private MailingList GetAllUsersOnFreeMembership()
        {
            var freeMembership =
                _membershipOptionsRepository.Find(e => e.SubscriptionType == MembershipOption.ESubscriptionType.Free).FirstOrDefault();
            
            var paidUserIds = GetAllUsersOnPaidMembership().Users.Select(e => e.Id).ToList();

            var freeMemberships = _userMembershipsRepository.Find(
                e => !paidUserIds.Contains(e.UserId) && e.MembershipOptionId == freeMembership.Id &&
                     e.StartsOn <= DateTime.Today && DateTime.Today <= e.EndsOn && !e.IsDeactivated).ToList();

            var freeMembershipUserIds = freeMemberships.Select(e => e.UserId).ToList();

            return new MailingList
            {
                Id = MailingList.FreeUsersId,
                Name = GetAutoListName(MailingList.FreeUsersId),
                IsSystemStandard = true,
                Users = My.UsersRepository.Find(e => freeMembershipUserIds.Contains(e.Id))
                    .OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList()
            };
        }
    }
}