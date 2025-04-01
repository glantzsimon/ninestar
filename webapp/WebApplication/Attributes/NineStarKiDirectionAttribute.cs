using K9.WebApplication.Models;
using System;

namespace K9.WebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NineStarKiDirectionAttribute : Attribute
    {
        public ENineStarKiDirection DisplayDirection { get; set; }
    }
}
