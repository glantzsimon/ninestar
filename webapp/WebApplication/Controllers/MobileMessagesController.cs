using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels.MobileApi;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/messages")]
    public class MobileMessagesController : MobileApiControllerBase
    {
        private readonly IRepository<Message> _messages;
        private readonly DbContext _database;
        public MobileMessagesController(INineStarKiPackage package, IRepository<Message> messages, DbContext database) : base(package)
        { _messages = messages; _database = database; }

        [HttpGet, Route("")]
        public ActionResult List(int afterId = 0, int limit = 50)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ModelState.IsValid || afterId < 0 || limit < 1 || limit > 100) return ApiError(400, "invalid_page");
            return ApiSuccess(_database.Set<Message>().AsNoTracking()
                .Where(e => !e.IsDeleted && e.UserId == id && e.Id > afterId).OrderBy(e => e.Id).Take(limit)
                .Select(e => new { e.Id, e.Subject, e.SentOn, e.MessageDirection, e.SentByUserId, e.SentToUserId }).ToArray());
        }

        [HttpGet, Route("{messageId:int}")]
        public ActionResult Find(int messageId)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var message = _messages.Find(messageId);
            if (message == null || message.IsDeleted || message.UserId != id) return ApiError(404, "message_not_found");
            return ApiSuccess(new { message.Id, message.Subject, HtmlBody = message.Body, message.SentOn,
                message.MessageDirection, message.SentByUserId, message.SentToUserId });
        }

        [HttpPost, Route("save")]
        public ActionResult Save(MobileMessageWriteRequest request)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_message");
            var message = request.Id.HasValue ? _messages.Find(request.Id.Value) : new Message
            {
                UserId = id, SentByUserId = id, SentToUserId = request.RecipientId,
                SentOn = GetUtcNow(), MessageDirection = EMessageDirection.Outbound
            };
            if (message == null || message.IsDeleted || message.UserId != id || message.SentByUserId != id)
                return ApiError(404, "message_not_found");
            if (Package.UserService.Find(message.SentToUserId) == null) return ApiError(400, "invalid_recipient");
            message.Subject = request.Subject;
            message.Body = HttpUtility.HtmlEncode(request.Body).Replace("\r\n", "\n").Replace("\n", "<br />");
            if (request.Id.HasValue) _messages.Update(message); else _messages.Create(message);
            // Mirrors the existing per-user repository CRUD. It does not imply
            // an email notification or create a separate recipient inbox copy.
            return ApiSuccess(new { message.Id });
        }

        [HttpPost, Route("delete")]
        public ActionResult Delete(int messageId)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var message = _messages.Find(messageId);
            if (message == null || message.IsDeleted || message.UserId != id) return ApiError(404, "message_not_found");
            _messages.Delete(messageId);
            return ApiSuccess(new { Deleted = true });
        }
    }
}
