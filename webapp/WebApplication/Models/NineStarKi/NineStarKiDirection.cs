namespace K9.WebApplication.Models
{
    public class NineStarKiDirection 
    {
        public string Name { get;  }
        public string Description { get;  }
        public ENineStarKiDirection Direction { get;  }

        public NineStarKiDirection(string name, string description, ENineStarKiDirection direction)
        {
            Name = name;
            Description = description;
            Direction = direction;
        }
    }
}