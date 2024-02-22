namespace LinuxNative.Interfaces;

/// <summary>
/// Interface providing common system/filesystem operations.
/// </summary>
public interface IFileOps
{
    /// <summary>
    /// Gets the system user ID of <paramref name="username"/>.
    /// </summary>
    /// <param name="username">The system username to look up. Ex. root</param>
    /// <param name="uid"></param>
    /// <returns>true on success and sets <paramref name="uid"/>. false in case of error or if no record matching <paramref name="username"/> could be found.</returns>
    bool GetUID(string username, out uint uid);

    /// <summary>
    /// Gets the system group ID of <paramref name="groupname"/>.
    /// </summary>
    /// <param name="groupname">The system groupname to look up. Ex. root</param>
    /// <param name="gid"></param>
    /// <returns>true on success and sets <paramref name="gid"/>. false in case of error or if no record matching <paramref name="groupname"/> could be found.</returns>
    bool GetGID(string groupname, out uint gid);

    /// <summary>
    /// Changes the user and group ownership of <paramref name="filepath"/>.
    /// </summary>
    /// <remarks>
    /// It is possible for this function to return false, if <paramref name="recurse"/> was true, but it encountered a file for which
    /// the calling user/process did not have permission to change. In this case, all files that were able to be chown'd were probably
    /// chown'd, but false will be returned becasue it was unable to chown them all.
    /// </remarks>
    /// <param name="username">The username of the new owner.</param>
    /// <param name="groupname">The groupname of the new group.</param>
    /// <param name="filepath">The file or directory path to change.</param>
    /// <param name="recurse">If true, and <paramref name="filepath"/> is a directory, recurse through all subdirectories and files.</param>
    /// <returns></returns>
    bool Chown(string username, string groupname, string filepath, bool recurse = false);

    /// <summary>
    /// Changes the file mode bits of <paramref name="pathname"/> according to <paramref name="mode"/>.
    /// </summary>
    /// <remarks>
    /// It is possible for this function to return false, if <paramref name="recurse"/> was true, but it encountered a file for which
    /// the calling user/process did not have permission to change. In this case, all files that were able to be chmod'd were probably
    /// chmod'd, but false will be returned becasue it was unable to chmod them all.
    /// </remarks>
    /// <param name="pathname">The file or directory path to change.</param>
    /// <param name="mode">Octal number representing the bit pattern for the new mode bits. Ex. 755</param>
    /// <param name="recurse">If true, and <paramref name="pathname"/> is a directory, recurse through all subdirectories and files.</param>
    /// <param name="omitDirectories">Don't chmod directories when recursively chmod'ing.</param>
    /// <returns></returns>
    bool Chmod(string pathname, uint mode, bool recurse = false, bool omitDirectories = false);
}
