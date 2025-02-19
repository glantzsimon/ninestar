using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Enums;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Routing;

namespace K9.SharedLibrary.Tests.Models
{
    public class NewsItem : IObjectBase
    {

        public NewsItem()
        {
            this.InitFileSources();
        }

        private int _id;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    this.InitFileSources();
                }
            }
        }

        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public string GetForeignKeyName()
        {
            throw new NotImplementedException();
        }

        public string GetLocalisedDescription()
        {
            throw new NotImplementedException();
        }

        public string GetLocalisedPropertyValue(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void UpdateAuditFields()
        {
            throw new NotImplementedException();
        }

        public void UpdateName()
        {
            throw new NotImplementedException();
        }

        public RouteValueDictionary GetForeignKeyFilterRouteValues()
        {
            throw new NotImplementedException();
        }

        public DateTime PublishedOn { get; set; }

        public string PublishedBy { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        [FileSourceInfo("Images/news/upload", Filter = EFilesSourceFilter.Images)]
        public FileSource ImageFileSource { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public bool IsSystemStandard { get; set; }
        public string CreatePermissionName { get; }
        public string EditPermissionName { get; }
        public string DeletePermissionName { get; }
        public string ViewPermissionName { get; }
        public bool IsSelected { get; set; }
        public string Description { get; }
    }
}
