using System.Buffers;
using System.Runtime.InteropServices;
using LinuxNative.Interfaces;

///<inheritdoc cref="IFileOps"/>
public class FileOps : IFileOps
{
    /// <inheritdoc/>
    public bool GetUID(string username, out uint uid)
    {
        uid = 0;
        int sz = Marshal.SizeOf(typeof(LibC.StructPasswd));
        IntPtr resultBuf;
        try
        {
            resultBuf = Marshal.AllocHGlobal(sz);
        }
        catch (Exception)
        {
            return false;
        }
        byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);
        IntPtr result = IntPtr.Zero;

        int n = LibC.getpwnam_r(username,
                                resultBuf,
                                buffer,
                                Convert.ToUInt64(buffer.Length),
                                ref result);
        if ((n != 0) || (result == IntPtr.Zero))
        {
            Marshal.FreeHGlobal(resultBuf);
            ArrayPool<byte>.Shared.Return(buffer);
            Console.WriteLine((n != 0) ? string.Format("getpwnam_r() returned {0}", n) : "getpwnam_r() found no results");
            return false;
        }

        bool retval = false;
        try
        {
#pragma warning disable CS8605 // Unboxing a possibly null value.
            LibC.StructPasswd entry =
                (LibC.StructPasswd)Marshal.PtrToStructure(resultBuf,
                                                          typeof(LibC.StructPasswd));
#pragma warning restore CS8605 // Unboxing a possibly null value.
            uid = entry.pw_uid;
            retval = true;
        }
        catch (Exception)
        {
        }

        Marshal.FreeHGlobal(resultBuf);
        ArrayPool<byte>.Shared.Return(buffer);

        return retval;
    }

    /// <inheritdoc/>
    public bool GetGID(string groupname, out uint gid)
    {
        gid = 0;
        int sz = Marshal.SizeOf(typeof(LibC.StructGroup));
        IntPtr resultBuf;
        try
        {
            resultBuf = Marshal.AllocHGlobal(sz);
        }
        catch (Exception)
        {
            return false;
        }
        byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);
        IntPtr result = IntPtr.Zero;

        int n = LibC.getgrnam_r(groupname,
                                resultBuf,
                                buffer,
                                Convert.ToUInt64(buffer.Length),
                                ref result);
        if ((n != 0) || (result == IntPtr.Zero))
        {
            Marshal.FreeHGlobal(resultBuf);
            ArrayPool<byte>.Shared.Return(buffer);
            Console.WriteLine((n != 0) ? string.Format("getgrnam_r() returned {0}", n) : "getgrnam_r() found no results");
            return false;
        }

        bool retval = false;
        try
        {
#pragma warning disable CS8605 // Unboxing a possibly null value.
            LibC.StructGroup entry =
                (LibC.StructGroup)Marshal.PtrToStructure(resultBuf,
                                                         typeof(LibC.StructGroup));
#pragma warning restore CS8605 // Unboxing a possibly null value.
            gid = entry.gr_gid;
            retval = true;
        }
        catch (Exception)
        {
        }

        Marshal.FreeHGlobal(resultBuf);
        ArrayPool<byte>.Shared.Return(buffer);

        return retval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="gid"></param>
    /// <param name="targetDirectory"></param>
    /// <param name="errorCount"></param>
    private void RecurseChown(uint uid,
                              uint gid,
                              string targetDirectory,
                              ref int errorCount)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
        {
            if (LibC.chown(fileName, uid, gid) != 0)
            {
                ++errorCount;
            }
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
        {
            RecurseChown(uid, gid, subdirectory, ref errorCount);
        }

        if (LibC.chown(targetDirectory, uid, gid) != 0)
        {
            ++errorCount;
        }
    }

    /// <inheritdoc/>
    public bool Chown(string username, string groupname, string filepath, bool recurse = false)
    {
        uint uid = 0, gid = 0;
        if (!(GetUID(username, out uid) && GetGID(groupname, out gid)))
        {
            return false;
        }

        if (recurse)
        {
            if (File.Exists(filepath))
            {
                return LibC.chown(filepath, uid, gid) == 0;
            }

            if (Directory.Exists(filepath))
            {
                int errorCount = 0;
                RecurseChown(uid, gid, filepath, ref errorCount);
                return errorCount == 0;
            }

            return false;
        }

        return LibC.chown(filepath, uid, gid) == 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="octal"></param>
    /// <returns></returns>
    private uint Oct2Dec(uint octal)
    {
        uint dec = 0, BASE = 1, temp = octal;
        while (temp > 0)
        {
            uint lastDigit = temp % 10;
            temp /= 10;
            dec += lastDigit * BASE;
            BASE *= 8;
        }
        return dec;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetDirectory"></param>
    /// <param name="mode"></param>
    /// <param name="errorCount"></param>
    /// <param name="omitDirectories"></param>
    private void RecurseChmod(string targetDirectory,
                              uint mode,
                              ref int errorCount,
                              bool omitDirectories)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
        {
            if (LibC.chmod(fileName, mode) != 0)
            {
                ++errorCount;
            }
        }

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
        {
            RecurseChmod(subdirectory, mode, ref errorCount, omitDirectories);
        }

        if (!omitDirectories && (LibC.chmod(targetDirectory, mode) != 0))
        {
            ++errorCount;
        }
    }

    /// <inheritdoc/>
    public bool Chmod(string pathname, uint mode, bool recurse = false, bool omitDirectories = false)
    {
        uint decMode = Oct2Dec(mode);

        if (recurse)
        {
            if (File.Exists(pathname))
            {
                return LibC.chmod(pathname, decMode) == 0;
            }

            if (Directory.Exists(pathname))
            {
                int errorCount = 0;
                RecurseChmod(pathname, decMode, ref errorCount, omitDirectories);
                return errorCount == 0;
            }

            return false;
        }

        return LibC.chmod(pathname, decMode) == 0;
    }
}