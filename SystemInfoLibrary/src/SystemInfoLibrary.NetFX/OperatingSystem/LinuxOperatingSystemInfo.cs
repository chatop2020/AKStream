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
using System.Text.RegularExpressions;
using SystemInfoLibrary.Hardware;

namespace SystemInfoLibrary.OperatingSystem
{
    internal class LinuxOperatingSystemInfo : UnixOperatingSystemInfo
    {
        private HardwareInfo _hardware;

        private string _java;
        private string _unameM;

        private string _unameRS;

        private string UnameM => string.IsNullOrEmpty(_unameM)
            ? (_unameM = Utils.GetCommandExecutionOutput("uname", "-m"))
            : _unameM;

        private string UnameRS => string.IsNullOrEmpty(_unameRS)
            ? (_unameRS = Utils.GetCommandExecutionOutput("uname", "-rs"))
            : _unameRS;

        private string Java => string.IsNullOrEmpty(_java)
            ? (_java = Utils.GetCommandExecutionOutput("java", "-version"))
            : _java;


        public override string Architecture
        {
            get
            {
                if (UnameM.Contains("i386") || UnameM.Contains("i686"))
                    return "32-bit";
                if (UnameM.Contains("x86_64"))
                    return "64-bit";
                return "Unknown";
            }
        }

        public override string Name => UnameRS.Replace("\n", "");

        public override Version JavaVersion
        {
            get
            {
                try
                {
                    var matches = new Regex(@"java version\s*""(.*)""").Matches(Java);
                    return new Version(matches[0].Groups[1].Value.Replace("_", "."));
                }
                catch
                {
                    return new Version(0, 0);
                }
            }
        }

        public override HardwareInfo Hardware => _hardware ?? (_hardware = new LinuxHardwareInfo());

        public override OperatingSystemInfo Update()
        {
            _hardware = null;

            _unameM = Utils.GetCommandExecutionOutput("uname", "-m");
            _unameRS = Utils.GetCommandExecutionOutput("uname", "-rs");
            _java = Utils.GetCommandExecutionOutput("java", "-version");

            return this;
        }
    }
}