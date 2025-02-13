using System.IO;

namespace mxtrAutomation.Common.Utils
{
    public interface IFileUtils
    {
        FileStream GetFileStream(string filePath, FileMode fileMode, FileAccess fileAccess);
    }

    public class FileUtils : IFileUtils
    {
        public FileStream GetFileStream(string filePath, FileMode fileMode, FileAccess fileAccess)
        {
            return new FileStream(filePath, fileMode, fileAccess);
        }
    }
}
