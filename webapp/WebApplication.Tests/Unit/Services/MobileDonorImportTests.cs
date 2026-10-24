using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class MobileDonorImportTests
    {
        [Fact]
        public void ImportAvoidsExistingAddressesAndDuplicateDonationRecipients()
        {
            var repository = new Mock<IRepository<Contact>>();
            repository.Setup(e => e.List()).Returns(new List<Contact>
            { new Contact { EmailAddress = "existing@example.com", IsUnsubscribed = true } });
            List<Contact> created = null;
            repository.Setup(e => e.CreateBatch(It.IsAny<List<Contact>>()))
                .Callback<List<Contact>>(items => created = items);
            var package = new Mock<INineStarKiBasePackage>();
            package.SetupGet(e => e.ContactsRepository).Returns(repository.Object);
            var service = new ContactService(package.Object);
            var count = service.ImportContactsFromDonations(new[]
            {
                new Donation { Customer = "Existing", CustomerEmail = "EXISTING@example.com " },
                new Donation { Customer = "New Person", CustomerEmail = "new@example.com" },
                new Donation { Customer = "Duplicate", CustomerEmail = "NEW@example.com" },
                new Donation { Customer = "Missing Address", CustomerEmail = " " }
            });
            Assert.Equal(1, count);
            Assert.Single(created);
            var contact = created[0];
            Assert.Equal("New Person", contact.FullName);
            Assert.Equal("new@example.com", contact.EmailAddress);
            repository.Verify(e => e.Update(It.IsAny<Contact>()), Times.Never);
        }
    }
}
