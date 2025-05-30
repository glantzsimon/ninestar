﻿using K9.DataAccessLayer.Models;
using System.Data.Entity;

namespace K9.DataAccessLayer.Database
{
    public class LocalDb : Base.DataAccessLayer.Database.Db
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<MembershipOption> MembershipOptions { get; set; }
        public DbSet<UserMembership> UserMemberships { get; set; }
        public DbSet<Promotion> PromoCodes { get; set; }
        public DbSet<UserPromotion> UserPromoCodes { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<UserConsultation> UserConsultations { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<UserOTP> UserOtps { get; set; }
        public DbSet<EmailQueueItem> EmailQueueItems { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<MailingList> MailingLists { get; set; }
        public DbSet<MailingListUser> MailingListUsers { get; set; }
        public DbSet<MailingListContact> MailingListContacts { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<TimeZone> TimeZones { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }
        public DbSet<ArticleComment> ArticleComments { get; set; }
        public DbSet<Like> ArticleCommentLikes { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
    }
}
