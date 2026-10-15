using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class ServicePerformanceTests
    {
        [Fact]
        public void MailingListSelectorDoesNotLoadRecipientsOrMemberships()
        {
            var package = new Mock<INineStarKiBasePackage>(MockBehavior.Strict);
            var lists = new Mock<IRepository<MailingList>>(MockBehavior.Strict);
            var listUsers = new Mock<IRepository<MailingListUser>>(MockBehavior.Strict);
            var memberships = new Mock<IRepository<UserMembership>>(MockBehavior.Strict);
            var options = new Mock<IRepository<MembershipOption>>(MockBehavior.Strict);
            lists.Setup(e => e.List()).Returns(new List<MailingList>
            {
                new MailingList { Id = 10, Name = "Saved list" }
            });
            var service = new MailingListService(package.Object, lists.Object, listUsers.Object,
                memberships.Object, options.Object);

            var items = service.GetMailingListListItems();

            Assert.Equal(8, items.Count);
            lists.Verify(e => e.List(), Times.Once);
            // Strict repositories reject any recipient/membership queries.
        }

        [Fact]
        public void FindingAllUsersListDoesNotBuildOtherLists()
        {
            var users = new Mock<IRepository<User>>(MockBehavior.Strict);
            users.Setup(e => e.List()).Returns(new List<User>
            {
                new User { Id = 1, FirstName = "Zed", LastName = "Last" },
                new User { Id = 2, FirstName = "Amy", LastName = "First" }
            });
            var package = new Mock<INineStarKiBasePackage>(MockBehavior.Strict);
            package.SetupGet(e => e.UsersRepository).Returns(users.Object);
            var service = new MailingListService(package.Object,
                new Mock<IRepository<MailingList>>(MockBehavior.Strict).Object,
                new Mock<IRepository<MailingListUser>>(MockBehavior.Strict).Object,
                new Mock<IRepository<UserMembership>>(MockBehavior.Strict).Object,
                new Mock<IRepository<MembershipOption>>(MockBehavior.Strict).Object);

            var list = service.Find(MailingList.AllUsersId);

            Assert.Equal("All Users", list.Name);
            Assert.True(list.IsSystemStandard);
            Assert.Equal(new[] { 2, 1 }, list.Users.Select(e => e.Id));
            Assert.Null(service.Find(MailingList.SignedUpInLastTwoWeeksId));
            users.Verify(e => e.List(), Times.Once);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void WeeklyBookingsBatchConsultationsAndLoadSelectedBookingOnce(bool noSlots)
        {
            var previous = HttpContext.Current;
            HttpContext.Current = Helpers.Helpers.CreateHttpContextWithSession();
            try
            {
                var selected = new Consultation { Id = 42 };
                var booked = new Consultation { Id = 100, SlotId = 1 };
                var slots = noSlots ? new List<Slot>() : new List<Slot>
                {
                    new Slot { Id = 1, StartsOn = DateTimeOffset.UtcNow.AddDays(5) },
                    new Slot { Id = 2, StartsOn = DateTimeOffset.UtcNow.AddDays(6) }
                };
                var users = new Mock<IRepository<User>>(MockBehavior.Strict);
                users.Setup(e => e.Find(8)).Returns(new User { Id = 8 });
                var package = new Mock<INineStarKiBasePackage>(MockBehavior.Strict);
                package.SetupGet(e => e.UsersRepository).Returns(users.Object);
                package.SetupGet(e => e.DefaultValuesConfiguration).Returns(new DefaultValuesConfiguration());
                var consultations = new Mock<IRepository<Consultation>>(MockBehavior.Strict);
                consultations.Setup(e => e.Find(42)).Returns(selected);
                consultations.Setup(e => e.Find(It.IsAny<Expression<Func<Consultation, bool>>>()))
                    .Returns((Expression<Func<Consultation, bool>> predicate) =>
                        new List<Consultation> { booked, new Consultation { Id = 101, SlotId = 999 } }
                            .Where(predicate.Compile()).ToList());
                var slotRepository = new Mock<IRepository<Slot>>(MockBehavior.Strict);
                slotRepository.Setup(e => e.Find(It.IsAny<Expression<Func<Slot, bool>>>()))
                    .Returns(slots);
                var booking = new UserConsultation { ConsultationId = 42, UserId = 8 };
                var bookings = new Mock<IRepository<UserConsultation>>(MockBehavior.Strict);
                bookings.Setup(e => e.Find(It.IsAny<Expression<Func<UserConsultation, bool>>>()))
                    .Returns((Expression<Func<UserConsultation, bool>> predicate) =>
                        new List<UserConsultation> { booking }.Where(predicate.Compile()).ToList());
                var service = new ConsultationService(package.Object,
                    new Mock<IContactService>(MockBehavior.Strict).Object,
                    new Mock<IEmailTemplateService>(MockBehavior.Strict).Object,
                    consultations.Object, bookings.Object, slotRepository.Object);

                var result = service.GetAllSlotsAndBookings(DateTime.Today.AddDays(5), 42);

                Assert.Equal(slots.Count, result.Count);
                consultations.Verify(e => e.Find(It.IsAny<Expression<Func<Consultation, bool>>>()),
                    noSlots ? Times.Never() : Times.Once());
                bookings.Verify(e => e.Find(It.IsAny<Expression<Func<UserConsultation, bool>>>()),
                    noSlots ? Times.Never() : Times.Once());
                users.Verify(e => e.Find(8), noSlots ? Times.Never() : Times.Once());
                if (!noSlots)
                {
                    Assert.Same(booked, result[0].Consultation);
                    Assert.Null(result[1].Consultation);
                    Assert.All(result, e =>
                    {
                        Assert.Same(booking, e.UserConsultation);
                        Assert.Equal("Europe/London", e.Slot.UserTimeZone);
                    });
                    Assert.Equal("Europe/London", booked.UserTimeZone);
                    Assert.Equal(package.Object.DefaultValuesConfiguration.CurrentTimeZone, booked.MyTimeZone);
                }
            }
            finally
            {
                HttpContext.Current = previous;
            }
        }
    }
}
