namespace K9.SharedLibrary.Models
{

	public enum EButtonType
	{
		Default,
		Primary,
		Success,
		Info,
		Warning,
		Danger,
		Link
	}

	public interface IButton
	{
		string Text { get; set; }
		string Action { get; set; }
		string Controller { get; set; }
        /// <summary>
        /// If true, the button is displayed in the main menu
        /// </summary>
	    bool IsMainMenuButton { get; set; }
		string IconCssClass { get; set; }
		EButtonType ButtonType { get; set; }
		string ButtonCssClass { get; }
	}
}
