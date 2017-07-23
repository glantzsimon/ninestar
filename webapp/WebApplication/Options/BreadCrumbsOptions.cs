using System.Collections.Generic;
using K9.Globalisation;
using K9.WebApplication.Models;

namespace K9.WebApplication.Options
{
	public class BreadCrumbsOptions
	{
		private readonly List<Crumb> _crumbs;

		public BreadCrumbsOptions(List<Crumb> crumbs)
		{
			_crumbs = crumbs;
		}

		public BreadCrumbsOptions()
		{
			_crumbs = new List<Crumb>();
		}

		public List<Crumb> Crumbs
		{
			get
			{
				var crumbs = new List<Crumb>();
				crumbs.Add(new Crumb
				{
					Label = Dictionary.Home,
					Controller = "Home",
					Action = "Index"
				});

				crumbs.AddRange(_crumbs);
				return crumbs;
			}
		}
	}
}