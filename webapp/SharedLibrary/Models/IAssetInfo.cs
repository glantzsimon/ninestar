

using System.IO;

namespace K9.SharedLibrary.Models
{
	public interface IAssetInfo
	{
		string PathOnDisk { get; }

		string FileName { get; }

	    string FileNameWithoutExtension { get; }

        string ShortFileName { get; }

        string Src { get; }

	    string FontAweSomeFileTypeClass { get; }

	    FileInfo FileInfo { get; }

		ImageInfo ImageInfo { get; }

		string Extension { get; }

	    string ExtensionNoDot { get; }

	    bool IsImage();

	    bool IsVideo();

	    bool IsAudio();

	    bool IsDocument();

        bool IsTextFile();

	}
}
