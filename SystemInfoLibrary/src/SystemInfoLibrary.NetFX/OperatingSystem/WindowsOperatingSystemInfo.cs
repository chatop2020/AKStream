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
using System.Linq;
using System.Management;
using Microsoft.Win32;
using SystemInfoLibrary.Hardware;

namespace SystemInfoLibrary.OperatingSystem
{
    internal class WindowsOperatingSystemInfo : OperatingSystemInfo
    {
        private HardwareInfo _hardware;

        public Version _javaVersion;
        private ManagementBaseObject _win32_OperatingSystem;


        public WindowsOperatingSystemInfo()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                _win32_OperatingSystem = (from ManagementBaseObject os in searcher.Get() select os).FirstOrDefault();
            }
        }

        public override string Architecture => (String) _win32_OperatingSystem.GetPropertyValue("OSArchitecture");

        public override string Name =>
            $"{(String) _win32_OperatingSystem.GetPropertyValue("Caption")} SP{(UInt16) _win32_OperatingSystem.GetPropertyValue("ServicePackMajorVersion")}.{(UInt16) _win32_OperatingSystem.GetPropertyValue("ServicePackMinorVersion")}";

        public override Version JavaVersion
        {
            get
            {
                if (_javaVersion == null)
                {
                    try
                    {
                        var javaVersion = Architecture == "x86"
                            ? (string) Utils.GetRegistryValue(Registry.LocalMachine,
                                @"Software\JavaSoft\Java Runtime Environment", "CurrentVersion", "")
                            : (string) Utils.GetRegistryValue(Registry.LocalMachine,
                                @"Software\Wow6432Node\JavaSoft\Java Runtime Environment", "CurrentVersion", "");
                        _javaVersion = new Version(javaVersion);
                    }
                    catch
                    {
                        _javaVersion = new Version(0, 0);
                    }
                }

                return _javaVersion;
            }
        }

        public override HardwareInfo Hardware => _hardware ?? (_hardware = new WindowsHardwareInfo());


        public override OperatingSystemInfo Update()
        {
            _hardware = null;

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                _win32_OperatingSystem = (from ManagementBaseObject os in searcher.Get() select os).FirstOrDefault();
            }

            return this;
        }
    }
}