namespace Standard
{
    /// <summary>
    /// Filesystem path utility that supplements <see cref="System.IO.Path" />.
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// Remove any path root present in the path.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing path information.</param>
        /// <returns>The path with the root removed if it was present; path otherwise.</returns>
        /// <remarks>Unlike the <see cref="System.IO.Path"/> class the path is not checked for validity.</remarks>
        public static string RemoveRoot(string path)
        {
            string result = path;

            if (!string.IsNullOrEmpty(path))
            {
                if ((path[0] == '\\') || (path[0] == '/'))
                {
                    // UNC name ?
                    if ((path.Length > 1) && ((path[1] == '\\') || (path[1] == '/')))
                    {
                        int index = 2;
                        int elements = 2;

                        // Scan for two separate elements \\machine\share\restofpath
                        while ((index <= path.Length) &&
                            (((path[index] != '\\') && (path[index] != '/')) || (--elements > 0)))
                        {
                            index++;
                        }

                        index++;

                        if (index < path.Length)
                            result = path.Substring(index);
                        else
                            result = string.Empty;
                    }
                }
                else if ((path.Length > 1) && (path[1] == ':'))
                {
                    int dropCount = 2;
                    if ((path.Length > 2) && ((path[2] == '\\') || (path[2] == '/')))
                        dropCount = 3;

                    result = result.Remove(0, dropCount);
                }
            }
            return result;
        }
    }
}
