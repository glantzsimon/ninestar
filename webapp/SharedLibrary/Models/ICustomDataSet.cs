using System.Collections.Generic;
using System.Data.Entity;

namespace K9.SharedLibrary.Models
{
    public interface ICustomDataSet<T>
    {
        string Key { get; }
        List<ListItem> GetData(DbContext db);
    }
}
