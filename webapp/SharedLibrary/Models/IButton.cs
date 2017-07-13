


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
		string IconCssClass { get; set; }
		EButtonType ButtonType { get; set; }
		string ButtonCssClass { get; }
	}
}
