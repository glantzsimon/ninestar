using System;
using System.Collections.Generic;

namespace K9.SharedLibrary.Models
{

	public interface IDataSets
	{
		IDictionary<Type, List<ListItem>> Collection { get; }
	}

}