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
    /// <remarks></remarks>
    /// <param name="username"></param>
    /// <param name="groupname"></param>
    /// <param name="filepath"></param>
    /// <param name="recurse"></param>
    /// <returns></returns>
    bool Chown(string username, string groupname, string filepath, bool recurse = false);

    /// <summary>
    /// Changes the file mode bits of <paramref name="pathname"/> according to <paramref name="mode"/>.
    /// </summary>
    /// <remarks></remarks>
    /// <param name="pathname">The file or directory path to change.</param>
    /// <param name="mode">Octal number representing the bit pattern for the new mode bits. Ex. 755</param>
    /// <param name="recurse"></param>
    /// <returns></returns>
    bool Chmod(string pathname, uint mode, bool recurse = false);
}