using System.Collections.Generic;
using System.IO;

namespace Backup.Utils
{
    public class ExcludeUtil
    {
        /// <summary>
        /// Returns true if the given file should be excluded from the backup, else false.
        /// </summary>
        /// <param name="filePath">the file path to check</param>
        /// <param name="excludePaths">all exclude paths of the current used backup profile</param>
        /// <returns>true if the file should be excluded, else false</returns>
        public bool ShouldFileBeExcluded(string filePath, IList<string> excludePaths)
        {
            // full path is listed in exclude paths
            if (excludePaths.Contains(filePath))
            {
                return true;
            }
            
            // relative path is listed in exclude paths
            if (ContainsExcludedRelativePath(filePath, excludePaths))
            {
                return true;
            }
            
            // file extension is listed in exclude paths
            if (ContainsExcludedFileExtension(filePath, excludePaths))
            {
                return true;
            }

            // none of the above exclude checks did pass, thus the file should not be excluded
            return false;
        }
        
        /// <summary>
        /// Returns true if the given file is on a relative path that should be excluded, else false.
        /// </summary>
        /// <param name="filePath">the file path to check</param>
        /// <param name="excludePaths">all exclude paths (path entries like "*/file" and "*\file" is checked here)</param>
        /// <returns>true if the file should be excluded, else false</returns>
        private bool ContainsExcludedRelativePath(string filePath, IList<string> excludePaths)
        {
            // go through each exclude path definition
            foreach (string excludePath in excludePaths)
            {
                // "*/<file path>" in excludePath shows that a file should be ignored,
                // everything after the star character should be checked against the given file path ending
                char sep = Path.DirectorySeparatorChar;
                if (excludePath.StartsWith($"*{sep}") && filePath.EndsWith(excludePath.Substring(1)))
                {
                    //Logger.LogInfo("Exclude relative file path: {0}", filePath);
                    return true;
                }
            }

            // file extension should not be ignored
            return false;
        }

        /// <summary>
        /// Returns true if the given file has an extension that should be excluded, else false.
        /// </summary>
        /// <param name="filePath">the file path to check</param>
        /// <param name="excludePaths">all exclude paths (path entries like "*.extension" is checked here)</param>
        /// <returns>true if the file should be excluded, else false</returns>
        private bool ContainsExcludedFileExtension(string filePath, IList<string> excludePaths)
        {
            // go through each exclude path definition
            foreach (string excludePath in excludePaths)
            {
                // "*." in excludePath shows that a file extension should be ignored,
                // everything after the star character should be checked against the given file path ending
                if (excludePath.StartsWith("*.") && filePath.EndsWith(excludePath.Substring(1)))
                {
                    //Logger.LogInfo("Exclude file because of extension: {0}", filePath);
                    return true;
                }
            }

            // file extension should not be ignored
            return false;
        }

        /// <summary>
        /// Returns true if the given directory should be excluded from the backup, else false.
        /// </summary>
        /// <param name="dirPath">the directory path to check</param>
        /// <param name="excludePaths">all exclude paths of the current used backup profile</param>
        /// <returns>true if the directory should be excluded, else false</returns>
        public bool ShouldDirectoryBeExcluded(string dirPath, IList<string> excludePaths)
        {
            // full path is listed in exclude paths
            if (excludePaths.Contains(dirPath))
            {
                return true;
            }

            // directory is listed via wildcard in exclude paths
            if (ContainsExcludedDirectoryWildcard(dirPath, excludePaths))
            {
                return true;
            }
            
            // none of the above exclude checks did pass, thus the directory should not be excluded
            return false;
        }
        
        /// <summary>
        /// Returns true if the given directory should be excluded, else false.
        /// </summary>
        /// <param name="dirPath">the directory path to check</param>
        /// <param name="excludePaths">all exclude paths (path entries like "*/dir/*" or "*\dir\* is checked here)</param>
        /// <returns>true if the directory should be excluded, else false</returns>
        private bool ContainsExcludedDirectoryWildcard(string dirPath, IList<string> excludePaths)
        {
            //Logger.LogInfo("Check dir: {0}", dirPath);
            
            // go through each exclude path definition
            foreach (string excludePath in excludePaths)
            {
                // "*/<dir>/*" or "*\<dir>\*" (depending on the directory separator char) in excludePath shows that 
                // a the sub dir <dir> should be ignored
                char sep = Path.DirectorySeparatorChar;
                if (excludePath.StartsWith($"*{sep}") && excludePath.EndsWith($"{sep}*"))
                {
                    // everything between the prefix "*/" (or "*\") and the suffix "/*" (or "\*") should be checked 
                    // against the dir path;
                    // example: the current path might be "<parent>/exclude" and the wildcard for excluding this dir
                    //          would be "*/exclude/*" then, so ends with "$exclude" has to be checked where $ is the
                    //          platform dependent directory separator char
                    int directoryNameLength = excludePath.Length - 4;
                    string excludeDir = Path.DirectorySeparatorChar + excludePath.Substring(2, directoryNameLength);
                    if (dirPath.EndsWith(excludeDir))
                    {
                        //Logger.LogInfo("Exclude directory because of wildcard: {0}", dirPath);
                        return true;
                    }
                }
            }

            // file extension should not be ignored
            return false;
        }
    }
}