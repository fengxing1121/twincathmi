namespace QueryHmiVersion
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Diagnostics;

    public static class TcHmiDllHelper
    {
        public static Version GetVersion(string pathToDll)
        {
            try
            {
                if (!File.Exists(pathToDll)) return null;

                var productVersion = FileVersionInfo.GetVersionInfo(pathToDll).ProductVersion;

                if (string.IsNullOrEmpty(productVersion))
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(pathToDll);
                        var ver = assembly.GetName().Version;
                        if (ver != null)
                            return new Version(ver.ToString());
                    }
                    catch
                    {
                        // ignore
                    }

                    try
                    {
                        var assembly = Assembly.Load(File.ReadAllBytes(pathToDll));
                        var ver = assembly.GetName().Version;
                        if (ver != null)
                            return new Version(ver.ToString());
                    }
                    catch
                    {
                        // ignore
                    }
                }
                else
                {
                    return new Version(productVersion);
                }
            }
            catch
            {
                // ignore
            }

            return null;
        }

        public static DateTime RetrieveLinkerTimestamp(string pathToDll)
        {
            var filePath = pathToDll;
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var b = new byte[2048];
            Stream s = null;

            try
            {
                s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                s?.Close();
            }

            var i = BitConverter.ToInt32(b, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(b, i + linkerTimestampOffset);
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.ToLocalTime();
            return dt;
        }
    }
}
