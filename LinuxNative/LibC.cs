using System.Runtime.InteropServices;

/// <summary>
/// This class provides some native standard C library functions.
/// </summary>
internal class LibC
{
    internal const string LIBC_DLL = "/lib/x86_64-linux-gnu/libc.so.6";

    /// <summary>
    /// The <c>struct passwd</c> from /usr/include/pwd.h.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=0)]
    public struct StructPasswd
    {
        public IntPtr pw_name;
        public IntPtr pw_passwd;
        public uint   pw_uid;
        public uint   pw_gid;
        public IntPtr pw_gecos;
        public IntPtr pw_dir;
        public IntPtr pw_shell;
    }

    /// <summary>
    /// The <c>struct group</c> from /usr/include/grp.h.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=0)]
    public struct StructGroup
    {
        public IntPtr gr_name;
        public IntPtr gr_passwd;
        public uint   gr_gid;
        public IntPtr gr_mem;
    }

    /// <summary>
    /// Gets password file entry.
    /// </summary>
    /// <remarks>man 3 getgrnam_r</remarks>
    /// <param name="name">The system username to lookup.</param>
    /// <param name="pwd">Pointer to a <c cref="StructPasswd">StructPasswd</c> heap alloc'd buffer in which the result will be stored.</param>
    /// <param name="buf">A heap alloc'd buffer in which the string fields pointed to by the members of the <c cref="StructPasswd">StructPasswd</c> will be stored.</param>
    /// <param name="buflen">The size of <paramref name="buf"/>.</param>
    /// <param name="result">A pointer to the result in case of success, or <c cref="IntPtr.Zero">IntPtr.Zero</c>.</param>
    /// <returns>
    /// 0 on success and sets <paramref name="result"/> to <paramref name="pwd"/>.
    /// If no matching password record is found, returns 0 and sets <paramref name="result"/> to <c cref="IntPtr.Zero">IntPtr.Zero</c>.
    /// In case of error, an error number is returned and sets <paramref name="result"/> to <c cref="IntPtr.Zero">IntPtr.Zero</c>.
    /// </returns>
    [DllImport(LIBC_DLL, CharSet = CharSet.Ansi, EntryPoint = "getpwnam_r")]
    public static extern int getpwnam_r(string       name,      // const char *
                                        [Out] IntPtr pwd,       // struct passwd *
                                        [Out] byte[] buf,       // char *
                                        ulong        buflen,    // size_t
                                        ref IntPtr   result);   // struct passwd **

    /// <summary>
    /// Gets group file entry.
    /// </summary>
    /// <remarks>man 3 getgrnam_r</remarks>
    /// <param name="name">The system username to lookup.</param>
    /// <param name="grp">Pointer to a <c cref="StructGroup">StructGroup</c> heap alloc'd buffer in which the result will be stored.</param>
    /// <param name="buf">A heap alloc'd buffer in which the string fields pointed to by the members of the <c cref="StructGroup">StructGroup</c> will be stored.</param>
    /// <param name="buflen">The size of <paramref name="buf"/>.</param>
    /// <param name="result">A pointer to the result in case of success, or <c cref="IntPtr.Zero">IntPtr.Zero</c>.</param>
    /// <returns>
    /// 0 on success and sets <paramref name="result"/> to <paramref name="grp"/>.
    /// If no matching group record is found, returns 0 and sets <paramref name="result"/> to <c cref="IntPtr.Zero">IntPtr.Zero</c>.
    /// In case of error, an error number is returned and sets <paramref name="result"/> to <c cref="IntPtr.Zero">IntPtr.Zero</c>.
    /// </returns>
    [DllImport(LIBC_DLL, CharSet = CharSet.Ansi, EntryPoint = "getgrnam_r")]
    public static extern int getgrnam_r(string       name,      // const char *
                                        [Out] IntPtr grp,       // struct group *
                                        [Out] byte[] buf,       // char *
                                        ulong        buflen,    // size_t
                                        ref IntPtr   result);   // struct group **

    /// <summary>
    /// Change ownership of a file.
    /// </summary>
    /// <remarks>man 2 chown</remarks>
    /// <param name="pathname">The file to change.</param>
    /// <param name="owner">The user ID of the new owner.</param>
    /// <param name="_group">The group ID of the new group.</param>
    /// <returns></returns>
    [DllImport(LIBC_DLL, CharSet = CharSet.Ansi, EntryPoint = "chown")]
    public static extern int chown(string pathname, uint owner, uint _group);

    /// <summary>
    /// Change permissions of a file.
    /// </summary>
    /// <remarks>man 2 chmod</remarks>
    /// <param name="pathname">The file to change.</param>
    /// <param name="mode">The new file mode bit mask.</param>
    /// <returns></returns>
    [DllImport(LIBC_DLL, CharSet = CharSet.Ansi, EntryPoint = "chmod")]
    public static extern int chmod(string pathname, uint mode);

    /// <summary>
    /// Get string describing error number.
    /// </summary>
    /// <remarks>man 3 strerror_r</remarks>
    /// <param name="errnum">The error number.</param>
    /// <param name="buf">A heap alloc'd buffer where the description will be stored.</param>
    /// <param name="buflen">The size of <paramref name="buf"/>.</param>
    /// <returns>
    /// 0 on success. On error, a (positive) error number is returned (since glibc 2.13),
    /// or -1 is returned and errno is set to indicate  the  error  (before  glibc 2.13).
    /// </returns>
    [DllImport(LIBC_DLL, CharSet = CharSet.Ansi, EntryPoint = "__xpg_strerror_r")]
    public static extern int strerror_r(int errnum, [Out] IntPtr buf, ulong buflen);
}