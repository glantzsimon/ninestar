using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact> _contactsRepository;
        private readonly ILogger _logger;
        private readonly IRepository<User> _usersRepository;

        public ContactService(IRepository<Contact> contactsRepository, ILogger logger, IRepository<User> usersRepository)
        {
            _contactsRepository = contactsRepository;
            _logger = logger;
            _usersRepository = usersRepository;
        }

        public Contact GetOrCreateContact(string stripeCustomerId, string fullName, string emailAddress, string phoneNumber = "", int? userId = null)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                try
                {
                    var existingCustomer = _contactsRepository.Find(_ => (!string.IsNullOrEmpty(stripeCustomerId) && _.StripeCustomerId == stripeCustomerId) || _.EmailAddress == emailAddress).FirstOrDefault();
                    if (existingCustomer == null)
                    {
                        _contactsRepository.Create(new Contact
                        {
                            StripeCustomerId = stripeCustomerId,
                            FullName = string.IsNullOrEmpty(fullName) ? emailAddress : fullName,
                            EmailAddress = emailAddress,
                            PhoneNumber = phoneNumber
                        });
                        return _contactsRepository.Find(e => e.EmailAddress == emailAddress).FirstOrDefault();
                    }

                    var isUpdated = false;
                    if (existingCustomer.FullName != fullName)
                    {
                        existingCustomer.FullName = fullName;
                        isUpdated = true;
                    }

                    if (existingCustomer.EmailAddress != emailAddress)
                    {
                        existingCustomer.EmailAddress = emailAddress;
                        isUpdated = true;
                    }

                    if (userId != null && existingCustomer.UserId != userId)
                    {
                        existingCustomer.UserId = userId;
                        isUpdated = true;
                    }

                    if (isUpdated)
                    {
                        _contactsRepository.Update(existingCustomer);
                    }

                    return existingCustomer;
                }
                catch (Exception e)
                {
                    _logger.Error($"ContactService => CreateCustomer => {e.GetFullErrorMessage()}");
                    throw;
                }
            }

            _logger.Error($"ContactService => CreateCustomer => Email Address is Empty");
            return null;
        }

        public Contact Find(int id)
        {
            return _contactsRepository.Find(id);
        }

        public Contact Find(string emailAddress)
        {
            return _contactsRepository.Find(e => e.EmailAddress == emailAddress).FirstOrDefault();
        }

        public List<Contact> ListContacts()
        {
            return _contactsRepository.List().OrderBy(e => e.FullName).ToList();
        }

        public void EnableMarketingEmails(string externalId, bool value = true)
        {
            if (externalId != null)
            {
                var contact = _contactsRepository.Find(e => e.Name == externalId).FirstOrDefault();
                if (contact == null)
                {
                    _logger.Log(LogLevel.Error, $"ContactService => EnableMarketingEmails => Contact with External Id: {externalId} not found");
                    throw new Exception("Contact not found");
                }

                try
                {
                    contact.IsUnsubscribed = !value;
                    _contactsRepository.Update(contact);
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error,
                        $"ContactService => EnableMarketingEmails => Could not update contact => ContactId: {contact.Id} Error => {e.GetFullErrorMessage()}");
                    throw;
                }

                var user = _usersRepository.Find(e => e.EmailAddress == contact.EmailAddress).FirstOrDefault();
                if (user != null)
                {
                    user.IsUnsubscribed = !value;
                    try
                    {
                        _usersRepository.Update(user);
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogLevel.Error,
                            $"ContactService => EnableMarketingEmails => Could not update user => UserId: {user.Id} => Error: {e.GetFullErrorMessage()}");
                        throw;
                    }
                }
            }
        }

        public bool AreMarketingEmailsEnableForContact(int id)
        {
            var contact = _contactsRepository.Find(id);
            if (contact == null)
            {
                _logger.Log(LogLevel.Error, $"ContactService => AreMarketingEmailsEnableForContact => Contact with ContactId: {id} not found");
                throw new Exception("Contact not found");
            }
            return !contact.IsUnsubscribed;
        }
    }
}