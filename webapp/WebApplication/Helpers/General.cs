using K9.Base.DataAccessLayer.Enums;
using System;

namespace K9.WebApplication.Helpers
{
    public static class Methods
	{

	    public static EGender GetRandomGender()
	    {
	        var rand = new Random();
	        return (EGender)rand.Next(1, 2);
	    }

	}
}