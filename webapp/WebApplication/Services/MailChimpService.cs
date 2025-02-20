using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using MailChimp.Net;
using MailChimp.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MailChimpConfiguration = K9.WebApplication.Config.MailChimpConfiguration;

namespace K9.WebApplication.Services
{
    public class MailChimpService : BaseService, IMailChimpService
    {
        private readonly IContactService _contactService;
        private readonly MailChimpConfiguration _mailChimpConfig;

        public MailChimpService(INineStarKiBasePackage packge, IOptions<MailChimpConfiguration> mailChimpConfig, IContactService contactService)
            : base(packge)
        {
            _contactService = contactService;
            _mailChimpConfig = mailChimpConfig.Value;
        }

        public void AddContact(string firstName, string lastName, string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new Exception("Email address cannot be empty");
            }

            var mailChimpManager = new MailChimpManager(_mailChimpConfig.MailChimpApiKey);

            mailChimpManager.Members.AddOrUpdateAsync(_mailChimpConfig.MailChimpListId, new Member
            {
                EmailAddress = emailAddress,
                Status = Status.Subscribed,
                StatusIfNew = Status.Subscribed,
                MergeFields = new Dictionary<string, object>
                {
                    {"FNAME", firstName},
                    {"LNAME", lastName},
                }
            });
        }

        public void AddContact(string name, string emailAddress)
        {
            var names = name.Split(' ');
            var firstName = names.FirstOrDefault().ToProperCase();
            var lastName = names.LastOrDefault().ToProperCase();
            lastName = lastName == firstName ? string.Empty : lastName;

            AddContact(firstName, lastName, emailAddress);
        }

        public void AddAllContacts()
        {
            foreach (var contact in _contactService.ListContacts())
            {
                AddContact(contact.FullName, contact.EmailAddress);
            }
        }
    }
}