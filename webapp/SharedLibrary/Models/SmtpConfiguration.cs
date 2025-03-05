﻿
namespace K9.SharedLibrary.Models
{
	public class SmtpConfiguration
	{
		public string SmtpServer { get; set; }
		public int SmtpPort { get; set; } = 587;
	    public string SmtpUserId { get; set; }
		public string SmtpPassword { get; set; }
		public string SmtpFromEmailAddress { get; set; }
		public string SmtpFromDisplayName { get; set; }
	}
}
