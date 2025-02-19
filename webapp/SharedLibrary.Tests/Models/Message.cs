using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Routing;
using K9.SharedLibrary.Models;

namespace K9.SharedLibrary.Tests.Models
{
    public class Message : IUserData
    {
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
        public int Id { get; set; }
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

        public List<PropertyInfo> GetFileSourceProperties()
        {
            throw new NotImplementedException();
        }

        public RouteValueDictionary GetForeignKeyFilterRouteValues()
        {
            throw new NotImplementedException();
        }

        public int UserId { get; set; }
    }
}
