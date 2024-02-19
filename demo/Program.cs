// See https://aka.ms/new-console-template for more information
using LinuxNative.Interfaces;

uint uid = 0;
string user = "root";
IFileOps fops = new FileOps();
if (fops.GetUID(user, out uid))
{
    Console.WriteLine("{0}'s uid: {1}", user, uid);
}

if (!fops.Chmod("/home/jlebowski/csharp/cave", 644, true, true))
{
    Console.WriteLine("Hmmm... chmod failed.");
}
