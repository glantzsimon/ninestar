using System;

namespace K9.WebApplication.Models
{
    public class BioRhythmsResultSet
    {
        public BioRhythmsResultSet()
        {
            BioRhythms = new BioRhythmsModel();
            NineStarKiBioRhythms = new BioRhythmsModel();
        }

        public BioRhythmsResultSet(NineStarKiModel nineStarKiModel, DateTime? selectedDate = null)
        {
            BioRhythms = new BioRhythmsModel(nineStarKiModel, selectedDate);
            NineStarKiBioRhythms = new BioRhythmsModel(nineStarKiModel, selectedDate);
        }
     
        public BioRhythmsModel BioRhythms { get; set; }
        
        public BioRhythmsModel NineStarKiBioRhythms { get; set; }
    }
}