using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace K9.SharedLibrary.Models
{
    public interface IDataSetsHelper
    {
        List<ListItem> GetDataSet<T>(bool refresh = false, string nameExpression = "Name", string valueExpression = "Name", bool includeDeleted = false, Type resourceType = null) where T : class, IObjectBase;
        List<ListItem> GetDataSetFromEnum<T>(bool refresh = false, Type resourceType = null);
        SelectList GetSelectList<T>(int? selectedId, bool refresh = false, string nameExpression = "Name", string valueExpression = "Name", bool includeDeleted = false, Type resourceType = null) where T : class, IObjectBase;
        SelectList GetSelectListFromEnum<T>(int selectedId, bool refresh = false, Type resourceType = null);
        string GetName<T>(int? selectedId, bool refresh = false, string nameExpression = "Name") where T : class, IObjectBase;
        void AddDataSetToCollection<T>(List<ListItem> items);
        string GetAllDataSetsJson();
    }
}