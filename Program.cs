using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Win32;

namespace fakeXONAR
{
    class Program
    {
        private static readonly string asioName = "XONAR SOUND CARD(64)"; // use for name and description (64bit driver) //
        private static List<Dictionary<string, string>> asioEntries = new List<Dictionary<string, string>>();
        static void Main( string[] args )
        {
            try
            {
                RegistryKey reg = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("ASIO", true);
                string[] entries = reg.GetSubKeyNames();
                Console.WriteLine("ASIO Devices Entries : ");
                for (int i = 0; i < entries.Length; i++)
                {
                    if (entries[i].Equals(asioName)) continue;

                    asioEntries.Add(new Dictionary<string, string>
                    {
                        ["Name"] = entries[i],
                        ["CLSID"] = reg.OpenSubKey(entries[i]).GetValue("CLSID").ToString(),
                        ["Description"] = reg.OpenSubKey(entries[i]).GetValue("Description").ToString(),
                    } );
                    Console.WriteLine($"{i}: {entries[i]}");
                }
                Console.WriteLine();

            userInput:
                int selection = 0;
                try
                {
                    Console.Write("Choose which one to use : ");
                    selection = Int32.Parse(Console.ReadLine());
                    Console.WriteLine();

                    var entry = asioEntries[selection];
                    Console.WriteLine($"Selected Entry Info : \r\n" +
                        $"Name : {entry["Name"]}\r\n" +
                        $"CLSID : {entry["CLSID"]}\r\n" +
                        $"Description : {entry["Description"]}\r\n");

                    Console.WriteLine($"Trying to make fake {asioName} entry with above info");
                    reg.CreateSubKey(asioName);

                    RegistryKey fakeASIOEntry = reg.OpenSubKey(asioName, true);
                    fakeASIOEntry.SetValue("CLSID", entry["CLSID"]);
                    fakeASIOEntry.SetValue("Description", asioName);

                    Console.WriteLine("Done.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid Selection... Try again");
                    goto userInput;
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Invalid Selection... Try again");
                    goto userInput;
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Invalid Selection... Try again");
                    goto userInput;
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Permission Denied, make sure this application running with elevated privileges");
                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("There is no ASIO entries...");
            }
            catch (SecurityException)
            {
                Console.WriteLine("Permission Denied, make sure this application running with elevated privileges");
            }
        }
    }
}
