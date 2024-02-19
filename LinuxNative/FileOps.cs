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
        IntPtr resultBuf = Marshal.AllocHGlobal(sz);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);
        IntPtr result = IntPtr.Zero;

        int n = LibC.getpwnam_r(username,
                                resultBuf,
                                buffer,
                                Convert.ToUInt64(buffer.Length),
                                ref result);
        if (n != 0)
        {
            Marshal.FreeHGlobal(resultBuf);
            resultBuf = IntPtr.Zero;
            Console.WriteLine(string.Format("getpwnam_r() returned {0}", n));
            return false;
        }

        if (result == IntPtr.Zero)
        {
            Marshal.FreeHGlobal(resultBuf);
            resultBuf = IntPtr.Zero;
            Console.WriteLine("getpwnam_r() found no results");
            return false;
        }

        LibC.StructPasswd entry =
            (LibC.StructPasswd)Marshal.PtrToStructure(resultBuf,
                                                      typeof(LibC.StructPasswd));
        uid = entry.pw_uid;

        Marshal.FreeHGlobal(resultBuf);
        resultBuf = IntPtr.Zero;

        return true;
    }

    /// <inheritdoc/>
    public bool GetGID(string groupname, out uint gid)
    {
        gid = 0;
        int sz = Marshal.SizeOf(typeof(LibC.StructGroup));
        IntPtr resultBuf = Marshal.AllocHGlobal(sz);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(1024);
        IntPtr result = IntPtr.Zero;

        int n = LibC.getgrnam_r(groupname,
                                resultBuf,
                                buffer,
                                Convert.ToUInt64(buffer.Length),
                                ref result);
        if (n != 0)
        {
            Marshal.FreeHGlobal(resultBuf);
            resultBuf = IntPtr.Zero;
            Console.WriteLine(string.Format("getgrnam_r() returned {0}", n));
            return false;
        }

        if (result == IntPtr.Zero)
        {
            Marshal.FreeHGlobal(resultBuf);
            resultBuf = IntPtr.Zero;
            Console.WriteLine("getgrnam_r() found no results");
            return false;
        }

        LibC.StructGroup entry =
            (LibC.StructGroup)Marshal.PtrToStructure(resultBuf,
                                                     typeof(LibC.StructGroup));
        gid = entry.gr_gid;

        Marshal.FreeHGlobal(resultBuf);
        resultBuf = IntPtr.Zero;

        return true;
    }

    /// <inheritdoc/>
    public bool Chown(string username, string groupname, string filepath, bool recurse = false)
    {
        uint uid = 0, gid = 0;
        if (GetUID(username, out uid) && GetGID(groupname, out gid))
        {
            int n = LibC.chown(filepath, uid, gid);
            return (n == 0);
        }
        return false;
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
    private void ProcessDirectory(string targetDirectory,
                                  uint mode,
                                  ref int errorCount)
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
            ProcessDirectory(subdirectory, mode, ref errorCount);
        }

        if (LibC.chmod(targetDirectory, mode) != 0)
        {
            ++errorCount;
        }
    }

    /// <inheritdoc/>
    public bool Chmod(string pathname, uint mode, bool recurse = false)
    {
        uint decMode = Oct2Dec(mode);

        if (recurse)
        {
            if (File.Exists(pathname))
            {
                return (LibC.chmod(pathname, decMode) == 0);
            }

            if (Directory.Exists(pathname))
            {
                int errorCount = 0;
                ProcessDirectory(pathname, decMode, ref errorCount);
                return (errorCount == 0);
            }

            return false;
        }

        return (LibC.chmod(pathname, decMode) == 0);
    }
}