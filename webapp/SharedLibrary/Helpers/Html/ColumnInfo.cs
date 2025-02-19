using K9.SharedLibrary.Extensions;

namespace K9.SharedLibrary.Helpers.Html
{
    public class ColumnInfo
    {

        public string ColumnName { get; set; }
        public Enums.EColumnType ColumnType { get; set; }
        public string ColumnDisplayText { get; set; }

        public ColumnInfo() { }

        public ColumnInfo(string name)
        {
            ColumnName = name;
            ColumnDisplayText = name.SplitOnCapitalLetter();
        }

        public ColumnInfo(string name, string displayText)
        {
            ColumnName = name;
            ColumnDisplayText = displayText;
        }

        public ColumnInfo(Enums.EColumnType type)
        {
            ColumnType = type;
        }

        public ColumnInfo(Enums.EColumnType type, string name)
        {
            ColumnName = name;
            ColumnDisplayText = name.SplitOnCapitalLetter();
            ColumnType = type;
        }

        public ColumnInfo(Enums.EColumnType type, string name, string displayText)
        {
            ColumnName = name;
            ColumnDisplayText = displayText;
            ColumnType = type;
        }
    }
}