/*
 * Little Software Stats - .NET Library
 * Copyright (C) 2008-2012 Little Apps (http://www.little-apps.org)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SystemInfoLibrary
{
    internal static class Utils
    {
        public static string FilterCPUName(string name)
        {
            return name
                .Replace("(TM)", "")
                .Replace("(tm)", "")
                .Replace("(R)", "")
                .Replace("(r)", "");
        }

        public static string GetCommandExecutionOutput(string command, string arguments)
        {
            try
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        FileName = command,
                        Arguments = arguments,
                    }
                };

                proc.Start();

                var output = proc.StandardOutput.ReadToEnd();
                if (string.IsNullOrEmpty(output))
                    output = proc.StandardError.ReadToEnd();
                return output;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static object GetRegistryValue(RegistryKey regRoot, string regPath, string valueName,
            object defaultValue = null)
        {
            var value = defaultValue;

            using (var regKey = regRoot.OpenSubKey(regPath))
            {
                if (regKey != null)
                    value = defaultValue != null
                        ? regKey.GetValue(valueName, defaultValue)
                        : regKey.GetValue(valueName);
            }

            return value;
        }


        [DllImport("libc", EntryPoint = "uname")]
        public static extern int UName(IntPtr unameStruct);

        #region OS X

        [DllImport("libc", EntryPoint = "sysctlbyname")]
        private static extern int SysCtlByName([MarshalAs(UnmanagedType.LPStr)] string propName, IntPtr value,
            IntPtr oldLen, IntPtr newP, uint newLen);

        [DllImport("libc", EntryPoint = "getpagesize")]
        public static extern int GetPageSize();

        public static IntPtr GetSysCtlPropertyPtr(string propName)
        {
            try
            {
                var strLength = Marshal.AllocHGlobal(sizeof(int));
                SysCtlByName(propName, IntPtr.Zero, strLength, IntPtr.Zero, 0);
                var length = Marshal.ReadInt32(strLength);

                if (length == 0)
                {
                    Marshal.FreeHGlobal(strLength);
                    return IntPtr.Zero;
                }

                var strPtr = Marshal.AllocHGlobal(length);
                SysCtlByName(propName, strPtr, strLength, IntPtr.Zero, 0);

                Marshal.FreeHGlobal(strLength);

                return strPtr;
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        public static string GetSysCtlPropertyString(string propName)
        {
            var ptr = GetSysCtlPropertyPtr(propName);

            return ptr == IntPtr.Zero ? "Unknown" : Marshal.PtrToStringAnsi(ptr);
        }

        public static short GetSysCtlPropertyInt16(string propName)
        {
            var ptr = GetSysCtlPropertyPtr(propName);

            return ptr == IntPtr.Zero ? (short)0 : Marshal.ReadInt16(ptr);
        }

        public static int GetSysCtlPropertyInt32(string propName)
        {
            var ptr = GetSysCtlPropertyPtr(propName);

            return ptr == IntPtr.Zero ? 0 : Marshal.ReadInt32(ptr);
        }

        public static long GetSysCtlPropertyInt64(string propName)
        {
            var ptr = GetSysCtlPropertyPtr(propName);

            return ptr == IntPtr.Zero ? (long)0 : Marshal.ReadInt64(ptr);
        }

        #endregion OS X
    }
}