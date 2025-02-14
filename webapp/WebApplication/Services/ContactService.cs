using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact> _contactsRepository;
        private readonly ILogger _logger;

        public ContactService(IRepository<Contact> contactsRepository, ILogger logger)
        {
            _contactsRepository = contactsRepository;
            _logger = logger;
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

        public bool Unsubscribe(string code)
        {
            var contact = _contactsRepository.Find(e => e.Name == code).FirstOrDefault();
            if (contact != null)
            {
                contact.IsUnsubscribed = true;
                _contactsRepository.Update(contact);
                return true;
            }

            return false;
        }
    }
}