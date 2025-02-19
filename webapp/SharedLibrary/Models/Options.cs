
namespace K9.SharedLibrary.Models
{
	public class Options<T> : IOptions<T> where T : class
	{
		private readonly T _value;

		public Options(T value)
		{
			_value = value;
		}

		public T Value
		{
			get
			{
				return _value;
			}
		}
	}
}
