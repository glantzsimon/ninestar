using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Packages;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class ContactService : BaseService, IContactService
    {
        public ContactService(INineStarKiBasePackage my)
            : base(my)
        {
        }

        public Contact GetOrCreateContact(string stripeCustomerId, string fullName, string emailAddress, string phoneNumber = "", int? userId = null)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                try
                {
                    var existingCustomer = My.ContactsRepository.Find(_ => (!string.IsNullOrEmpty(stripeCustomerId) && _.StripeCustomerId == stripeCustomerId) || _.EmailAddress == emailAddress).FirstOrDefault();
                    if (existingCustomer == null)
                    {
                        My.ContactsRepository.Create(new Contact
                        {
                            StripeCustomerId = stripeCustomerId,
                            FullName = string.IsNullOrEmpty(fullName) ? emailAddress : fullName,
                            EmailAddress = emailAddress,
                            PhoneNumber = phoneNumber
                        });
                        return My.ContactsRepository.Find(e => e.EmailAddress == emailAddress).FirstOrDefault();
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
                        My.ContactsRepository.Update(existingCustomer);
                    }

                    return existingCustomer;
                }
                catch (Exception e)
                {
                    My.Logger.Error($"ContactService => CreateCustomer => {e.GetFullErrorMessage()}");
                    throw;
                }
            }

            My.Logger.Error($"ContactService => CreateCustomer => Email Address is Empty");
            return null;
        }

        public Contact Find(int id)
        {
            return My.ContactsRepository.Find(id);
        }

        public Contact Find(string emailAddress)
        {
            return My.ContactsRepository.Find(e => e.EmailAddress == emailAddress).FirstOrDefault();
        }

        public List<Contact> ListContacts()
        {
            return My.ContactsRepository.List().OrderBy(e => e.FullName).ToList();
        }

        public void EnableMarketingEmails(string externalId, bool value = true)
        {
            if (externalId != null)
            {
                var contact = My.ContactsRepository.Find(e => e.Name == externalId).FirstOrDefault();
                if (contact == null)
                {
                    My.Logger.Log(LogLevel.Error, $"ContactService => EnableMarketingEmails => Contact with External Id: {externalId} not found");
                    throw new Exception("Contact not found");
                }

                try
                {
                    contact.IsUnsubscribed = !value;
                    My.ContactsRepository.Update(contact);
                }
                catch (Exception e)
                {
                    My.Logger.Log(LogLevel.Error,
                        $"ContactService => EnableMarketingEmails => Could not update contact => ContactId: {contact.Id} Error => {e.GetFullErrorMessage()}");
                    throw;
                }

                var user = My.UsersRepository.Find(e => e.EmailAddress == contact.EmailAddress).FirstOrDefault();
                if (user != null)
                {
                    user.IsUnsubscribed = !value;
                    try
                    {
                        My.UsersRepository.Update(user);
                    }
                    catch (Exception e)
                    {
                        My.Logger.Log(LogLevel.Error,
                            $"ContactService => EnableMarketingEmails => Could not update user => UserId: {user.Id} => Error: {e.GetFullErrorMessage()}");
                        throw;
                    }
                }
            }
        }

        public bool AreMarketingEmailsEnableForContact(int id)
        {
            var contact = My.ContactsRepository.Find(id);
            if (contact == null)
            {
                My.Logger.Log(LogLevel.Error, $"ContactService => AreMarketingEmailsEnableForContact => Contact with ContactId: {id} not found");
                throw new Exception("Contact not found");
            }
            return !contact.IsUnsubscribed;
        }
    }
}