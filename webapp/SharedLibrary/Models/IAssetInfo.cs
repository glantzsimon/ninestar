

using System.IO;

namespace K9.SharedLibrary.Models
{
	public interface IAssetInfo
	{
		string PathOnDisk { get; }

		string FileName { get; }

		string Src { get; }

		FileInfo FileInfo { get; }

		ImageInfo ImageInfo { get; }

		string Extension { get; }

		bool IsImage();

		bool IsTextFile();

		string GetNameWithoutExtensions();

	}
}
