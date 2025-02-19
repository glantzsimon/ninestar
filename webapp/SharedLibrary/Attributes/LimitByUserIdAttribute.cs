using System;

namespace K9.SharedLibrary.Attributes
{
	/// <summary>
	/// Limit the data the user can see by the value of the UserId column
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class LimitByUserIdAttribute : System.Attribute
	{

	}
}
