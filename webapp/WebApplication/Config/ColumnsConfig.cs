using K9.SharedLibrary.Models;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Config
{
    public class ColumnsConfig : IColumnsConfig
    {
        public List<string> ColumnsToIgnore
        {
            get
            {
                return 
                    typeof(IAuditable).GetProperties().Where(e => e.Name != "CreatedOn").Select(p => p.Name).Concat(
                        typeof(IPermissable).GetProperties().Select(p => p.Name)).ToList();
            }
        }
    }
}