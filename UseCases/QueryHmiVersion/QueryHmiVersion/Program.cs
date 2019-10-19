namespace QueryHmiVersion
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var pathToAnyTcHmiDll = @"C:\TwinCAT\Functions\TE2000-HMI-Engineering\MSBuild\TcHmiCore.dll";

            Console.WriteLine("TwinCAT HMI Version:\t{0}", TcHmiDllHelper.GetVersion(pathToAnyTcHmiDll));
            Console.WriteLine("TwinCAT HMI Build:\t{0}", TcHmiDllHelper.RetrieveLinkerTimestamp(pathToAnyTcHmiDll));
            Console.WriteLine("Enter any key...");
            Console.ReadKey();
        }
    }
}
