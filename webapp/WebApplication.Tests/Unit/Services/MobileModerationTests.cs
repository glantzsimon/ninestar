using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class MobileModerationTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void OnlyAdministratorsCanDeleteAnotherAuthorsComment(bool administrator)
        {
            var previous = HttpContext.Current;
            try
            {
                HttpContext.Current = Helpers.Helpers.CreateHttpContextWithSession();
                SessionHelper.SetCurrentUserId(42);
                var comments = new Mock<IRepository<ArticleComment>>();
                comments.Setup(e => e.Find(9)).Returns(new ArticleComment { Id = 9, UserId = 43 });
                var likes = new Mock<IRepository<Like>>();
                likes.Setup(e => e.Find(It.IsAny<Expression<Func<Like, bool>>>())).Returns(new List<Like>());
                var users = new Mock<IUserService>();
                users.Setup(e => e.UserIsAdmin(42)).Returns(administrator);
                var service = new ArticlesService(new Mock<INineStarKiBasePackage>().Object,
                    null, null, null, comments.Object, null, null, likes.Object, null, users.Object);
                if (administrator) service.DeleteComment(9);
                else Assert.Throws<UnauthorizedAccessException>(() => service.DeleteComment(9));
                comments.Verify(e => e.Delete(9), administrator ? Times.Once() : Times.Never());
                likes.Verify(e => e.DeleteBatch(It.IsAny<List<Like>>()), administrator ? Times.Once() : Times.Never());
            }
            finally { HttpContext.Current = previous; }
        }

        [Fact]
        public void ACommentAuthorCannotApproveTheirOwnComment()
        {
            var previous = HttpContext.Current;
            try
            {
                HttpContext.Current = Helpers.Helpers.CreateHttpContextWithSession();
                SessionHelper.SetCurrentUserId(42);
                var comments = new Mock<IRepository<ArticleComment>>();
                comments.Setup(e => e.Find(9)).Returns(new ArticleComment { Id = 9, UserId = 42 });
                var service = new ArticlesService(new Mock<INineStarKiBasePackage>().Object,
                    null, null, null, comments.Object, null, null, null, null, new Mock<IUserService>().Object);
                Assert.Throws<UnauthorizedAccessException>(() => service.ApproveComment(9));
                comments.Verify(e => e.Update(It.IsAny<ArticleComment>()), Times.Never);
            }
            finally { HttpContext.Current = previous; }
        }
    }
}
