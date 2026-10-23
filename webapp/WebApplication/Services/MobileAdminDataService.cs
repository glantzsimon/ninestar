using Autofac;
using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers.Html;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels.MobileApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace K9.WebApplication.Services
{
    // Only named, reviewed entity contracts exist here. No arbitrary CLR types,
    // tables, SQL or navigation properties can be selected by an API caller.
    public class MobileAdminDataService : BaseService, IMobileAdminDataService
    {
        private readonly INineStarKiPackage _package;
        private readonly IComponentContext _components;
        private readonly DbContext _database;
        private readonly Dictionary<string, Func<IEntityContract>> _catalog;

        public MobileAdminDataService(INineStarKiPackage package, IComponentContext components, DbContext database) : base(package)
        {
            _package = package; _components = components; _database = database;
            _catalog = new Dictionary<string, Func<IEntityContract>>(StringComparer.OrdinalIgnoreCase)
            {
                { "countries", () => Contract<Country>("Name,TwoLetterCountryCode,ThreeLetterCountryCode", "Name,TwoLetterCountryCode,ThreeLetterCountryCode") },
                { "roles", () => Contract<Role>("Name", "Name") },
                { "permissions", () => Contract<Permission>("Name", "Name") },
                { "role-permissions", () => Contract<RolePermission>("RoleId,PermissionId", "RoleId,PermissionId") },
                { "user-roles", () => Contract<UserRole>("UserId,RoleId", "UserId,RoleId") },
                { "contacts", () => Contract<Contact>("FullName,EmailAddress,PhoneNumber,CompanyName,IsUnsubscribed", "FullName,EmailAddress,PhoneNumber,CompanyName,IsUnsubscribed") },
                { "donations", () => Contract<Donation>("Customer,CustomerEmail,DonationAmount,DonatedOn,Currency,DonationDescription", "Customer,CustomerEmail,DonationAmount,DonatedOn,Currency,DonationDescription") },
                { "membership-options", () => Contract<MembershipOption>("Name,SubscriptionType,SubscriptionDetails,Price", "Name,SubscriptionType,SubscriptionDetails,Price") },
                { "user-memberships", () => Contract<UserMembership>("UserId,MembershipOptionId,StartsOn,EndsOn,IsAutoRenew,IsDeactivated,ComplementaryPersonalChartReadingCount,ComplementaryPredictionsReadingCount,ComplementaryCompatibilityReadingCount,ComplementaryKarmicReadingCount", "UserId,MembershipOptionId,StartsOn,EndsOn,IsAutoRenew,IsDeactivated,ComplementaryPersonalChartReadingCount,ComplementaryPredictionsReadingCount,ComplementaryCompatibilityReadingCount,ComplementaryKarmicReadingCount") },
                { "consultations", () => Contract<Consultation>("ContactId,ConsultationDuration,ScheduledOn,CompletedOn,SlotId", "ContactId,ConsultationDuration,CompletedOn") },
                { "user-consultations", () => Contract<UserConsultation>("UserId,ConsultationId", "UserId,ConsultationId") },
                { "promotions", () => Contract<Promotion>("Code,MembershipOptionId,Discount,IsReusable,SpecialPrice", "Code,MembershipOptionId,Discount,IsReusable,SpecialPrice") },
                { "user-promotions", () => Contract<UserPromotion>("UserId,PromotionId,SentOn,UsedOn", "UserId,PromotionId,SentOn,UsedOn") },
                { "email-templates", () => Contract<EmailTemplate>("Name,Subject,SystemEmailTemplate,HtmlBody,MembershipOptionId,PromotionId,UserId", "Name,Subject,SystemEmailTemplate,HtmlBody,MembershipOptionId,PromotionId,UserId") },
                { "email-queue", () => Contract<EmailQueueItem>("Type,EmailTemplateId,ContactId,UserId,Subject,Body,ScheduledOn,SentOn,IsProcessed,Result", "") },
                { "mailing-lists", () => Contract<MailingList>("Name,Details", "Name,Details") },
                { "mailing-list-users", () => Contract<MailingListUser>("MailingListId,UserId", "MailingListId,UserId") },
                { "mailing-list-contacts", () => Contract<MailingListContact>("MailingListId,ContactId", "MailingListId,ContactId") },
                { "system-settings", () => Contract<SystemSetting>("IsSendMembershipUpgradeReminders,IsPausedEmailJobQueue,IsEnabledAlchemy", "IsSendMembershipUpgradeReminders,IsPausedEmailJobQueue,IsEnabledAlchemy", false) },
                { "users", () => Contract<User>("Username,FirstName,LastName,FullName,EmailAddress,PhoneNumber,BirthDate,Gender,IsUnsubscribed,AccountActivated", "") }
            };
        }

        private IEntityContract Contract<T>(string read, string write, bool canDelete = true) where T : class, IObjectBase, new() =>
            new EntityContract<T>(_components.Resolve<IRepository<T>>(), _database, _package, read, write, canDelete);

        private IEntityContract Get(string entity)
        {
            if (!_package.UserService.UserIsAdmin(Current.UserId)) throw new UnauthorizedAccessException();
            if (entity == null || !_catalog.TryGetValue(entity, out var factory)) throw new KeyNotFoundException();
            return factory();
        }

        public object Catalog()
        {
            if (!_package.UserService.UserIsAdmin(Current.UserId)) throw new UnauthorizedAccessException();
            return _catalog.Keys.OrderBy(e => e).ToArray();
        }
        public object Schema(string entity) => Get(entity).Schema();
        public object List(string entity, int afterId, int limit) => Get(entity).List(afterId, limit);
        public object Find(string entity, int id) => Get(entity).Find(id);
        public object Save(string entity, MobileAdminWriteRequest request) => Get(entity).Save(request);
        public void Delete(string entity, int id) => Get(entity).Delete(id);

        public object Selection(string relationship, int ownerId) => Association(relationship, ownerId, null);

        public object ReplaceSelection(string relationship, MobileAssociationRequest request)
        {
            if (request?.SelectedIds == null || request.SelectedIds.Length > 1000 || request.SelectedIds.Any(e => e <= 0))
                throw new ArgumentException("invalid_selection");
            return Association(relationship, request.OwnerId, request.SelectedIds.Distinct().ToArray());
        }

        private object Association(string relationship, int ownerId, int[] selected)
        {
            if (!_package.UserService.UserIsAdmin(Current.UserId)) throw new UnauthorizedAccessException();
            if (ownerId <= 0) throw new ArgumentException("invalid_owner");
            // Explicit pairs mirror the existing EditMultiple screens. Automatic
            // mailing lists are calculated and cannot be assigned as stored rows.
            switch (relationship)
            {
                case "user-roles": return Associate<User, Role, UserRole>(ownerId, selected, e => e.UserId == ownerId, e => e.RoleId,
                    id => new UserRole { UserId = ownerId, RoleId = id });
                case "role-permissions": return Associate<Role, Permission, RolePermission>(ownerId, selected, e => e.RoleId == ownerId, e => e.PermissionId,
                    id => new RolePermission { RoleId = ownerId, PermissionId = id });
                case "user-mailing-lists": return Associate<User, MailingList, MailingListUser>(ownerId, selected, e => e.UserId == ownerId, e => e.MailingListId,
                    id => new MailingListUser { UserId = ownerId, MailingListId = id });
                case "contact-mailing-lists": return Associate<Contact, MailingList, MailingListContact>(ownerId, selected, e => e.ContactId == ownerId, e => e.MailingListId,
                    id => new MailingListContact { ContactId = ownerId, MailingListId = id });
                case "mailing-list-users": return Associate<MailingList, User, MailingListUser>(ownerId, selected, e => e.MailingListId == ownerId, e => e.UserId,
                    id => new MailingListUser { MailingListId = ownerId, UserId = id });
                case "mailing-list-contacts": return Associate<MailingList, Contact, MailingListContact>(ownerId, selected, e => e.MailingListId == ownerId, e => e.ContactId,
                    id => new MailingListContact { MailingListId = ownerId, ContactId = id });
                default: throw new KeyNotFoundException();
            }
        }

        private object Associate<TOwner, TTarget, TLink>(int ownerId, int[] selected,
            Expression<Func<TLink, bool>> predicate, Func<TLink, int> targetId, Func<int, TLink> create)
            where TOwner : class, IObjectBase, new()
            where TTarget : class, IObjectBase, new()
            where TLink : class, IObjectBase, new()
        {
            if (selected == null)
            {
                if (!_database.Set<TOwner>().Any(e => e.Id == ownerId && !e.IsDeleted)) throw new KeyNotFoundException();
                return new { OwnerId = ownerId, SelectedIds = _database.Set<TLink>().AsNoTracking()
                    .Where(e => !e.IsDeleted).Where(predicate).ToList().Select(targetId).Distinct().ToArray() };
            }
            using (var transaction = _database.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                if (!_database.Set<TOwner>().Any(e => e.Id == ownerId && !e.IsDeleted)) throw new KeyNotFoundException();
                if (_database.Set<TTarget>().Count(e => !e.IsDeleted && selected.Contains(e.Id)) != selected.Length)
                    throw new ArgumentException("invalid_selection_target");
                var repository = _components.Resolve<IRepository<TLink>>();
                var existing = repository.Find(predicate).Where(e => !e.IsDeleted).ToList();
                var existingIds = existing.Select(targetId).ToArray();
                repository.DeleteBatch(existing.Where(e => !selected.Contains(targetId(e))).ToList());
                repository.CreateBatch(selected.Except(existingIds).Select(create).ToList());
                transaction.Commit();
                return new { OwnerId = ownerId, SelectedIds = selected };
            }
        }

        private interface IEntityContract
        {
            object Schema(); object List(int afterId, int limit); object Find(int id);
            object Save(MobileAdminWriteRequest request); void Delete(int id);
        }

        private sealed class EntityContract<T> : IEntityContract where T : class, IObjectBase, new()
        {
            private readonly IRepository<T> _repository;
            private readonly DbContext _database;
            private readonly INineStarKiPackage _package;
            private readonly PropertyInfo[] _read;
            private readonly Dictionary<string, PropertyInfo> _write;
            private readonly bool _canDelete;

            public EntityContract(IRepository<T> repository, DbContext database, INineStarKiPackage package,
                string read, string write, bool canDelete)
            {
                _repository = repository; _database = database; _package = package;
                _read = Properties("Id," + read).ToArray();
                _write = Properties(write).ToDictionary(e => e.Name, StringComparer.OrdinalIgnoreCase);
                _canDelete = canDelete && _write.Count > 0;
            }

            private static IEnumerable<PropertyInfo> Properties(string names)
            {
                foreach (var name in names.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var property = typeof(T).GetProperty(name);
                    if (property == null) throw new InvalidOperationException("Unknown admin contract field: " + typeof(T).Name + "." + name);
                    yield return property;
                }
            }

            private object Project(T entity) => entity == null ? null : _read.ToDictionary(e => e.Name, e => e.GetValue(entity));

            public object Schema() => new
            {
                CanWrite = _write.Count > 0, CanDelete = _canDelete,
                Fields = _read.Select(e => new
                {
                    e.Name, Label = e.GetCustomAttribute<DisplayAttribute>()?.GetName(),
                    Type = (Nullable.GetUnderlyingType(e.PropertyType) ?? e.PropertyType).Name,
                    Writable = _write.ContainsKey(e.Name), Required = e.GetCustomAttribute<RequiredAttribute>() != null,
                    MaxLength = e.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength,
                    Options = MobileMetadataHelper.EnumOptions(Nullable.GetUnderlyingType(e.PropertyType) ?? e.PropertyType)
                }).ToArray()
            };

            public object List(int afterId, int limit)
            {
                if (afterId < 0 || limit < 1 || limit > 100) throw new ArgumentException("invalid_page");
                var rows = _database.Set<T>().AsNoTracking().Where(e => !e.IsDeleted && e.Id > afterId).OrderBy(e => e.Id).Take(limit).ToList();
                return new { Items = rows.Select(Project).ToArray(), NextAfterId = rows.LastOrDefault()?.Id, HasMore = rows.Count == limit };
            }

            public object Find(int id)
            {
                var entity = _repository.Find(id);
                if (entity == null || entity.IsDeleted) throw new KeyNotFoundException();
                return Project(entity);
            }

            public object Save(MobileAdminWriteRequest request)
            {
                if (_write.Count == 0) throw new NotSupportedException();
                if (request?.Fields == null || request.Fields.Count == 0) throw new ArgumentException("fields_required");
                using (var transaction = _database.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var entity = request.Id.HasValue ? _repository.Find(request.Id.Value) : new T();
                    if (entity == null) throw new KeyNotFoundException();
                    foreach (var field in request.Fields)
                    {
                        if (!_write.TryGetValue(field.Key, out var property) || !property.CanWrite) throw new ArgumentException("field_not_writable");
                        var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        if (!(type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) ||
                            type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan) || type == typeof(Guid)))
                            throw new ArgumentException("invalid_field_type");
                        var value = field.Value == null ? null : JToken.FromObject(field.Value).ToObject(property.PropertyType,
                            JsonSerializer.Create(new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None }));
                        if (type.IsEnum && value != null && !Enum.IsDefined(type, value)) throw new ArgumentException("invalid_enum");
                        property.SetValue(entity, value);
                    }
                    if (entity is SystemSetting && !request.Id.HasValue && _repository.GetCount() > 0)
                        throw new InvalidOperationException("settings_already_exist");
                    var promotion = entity as Promotion;
                    if (promotion != null)
                    {
                        promotion.Name = promotion.Code;
                        promotion.MembershipOption = _package.MembershipService.GetMembershipOption(promotion.MembershipOptionId);
                        if (promotion.MembershipOption == null || promotion.MembershipOption.IsFree ||
                            promotion.SpecialPrice < 0 || promotion.SpecialPrice >= promotion.MembershipOption.Price)
                            throw new ValidationException("invalid_promotion");
                    }
                    Validator.ValidateObject(entity, new ValidationContext(entity), true);
                    // Preserve the shared generic controller's curly-markup hook.
                    HtmlMarkupHelper.ConvertToHtml(ref entity);
                    if (request.Id.HasValue) _repository.Update(entity); else _repository.Create(entity);
                    var contact = entity as Contact;
                    if (contact != null)
                        _package.ContactService.EnableMarketingEmails(contact.Name, !contact.IsUnsubscribed);
                    transaction.Commit();
                    var settings = entity as SystemSetting;
                    if (settings != null) _package.DefaultValuesConfiguration.SystemSettings = settings;
                    return Project(entity);
                }
            }

            public void Delete(int id)
            {
                if (!_canDelete) throw new NotSupportedException();
                if (_repository.Find(id) == null) throw new KeyNotFoundException();
                _repository.Delete(id);
            }
        }
    }
}
