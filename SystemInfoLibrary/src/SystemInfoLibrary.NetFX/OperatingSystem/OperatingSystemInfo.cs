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

using SystemInfoLibrary.Hardware;

namespace SystemInfoLibrary.OperatingSystem
{
    public enum OperatingSystemType
    {
        Windows,
        Linux,
        MacOSX,
        BSD,
        WebAssembly,
        Solaris,
        Haiku,
        Unity5,
        Other
    }

    // TODO: Use https://github.com/Microsoft/omi
    public abstract class OperatingSystemInfo
    {
        // Mono and NET Core pre-defined, check https://github.com/dotnet/corefx/blob/master/src/Native/Unix/configure.cmake (search for PAL_UNIX_NAME)
        public static System.Runtime.InteropServices.OSPlatform FreeBSD = System.Runtime.InteropServices.OSPlatform.Create("FREEBSD");
        public static System.Runtime.InteropServices.OSPlatform NetBSD = System.Runtime.InteropServices.OSPlatform.Create("NETBSD");
        public static System.Runtime.InteropServices.OSPlatform WebAssembly = System.Runtime.InteropServices.OSPlatform.Create("WEBASSEMBLY");
        // Mono pre-defined https://github.com/mono/mono/blob/master/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs
        public static System.Runtime.InteropServices.OSPlatform Solaris = System.Runtime.InteropServices.OSPlatform.Create("SOLARIS");
        public static System.Runtime.InteropServices.OSPlatform OpenBSD = System.Runtime.InteropServices.OSPlatform.Create("OPENBSD");
        public static System.Runtime.InteropServices.OSPlatform AIX = System.Runtime.InteropServices.OSPlatform.Create("AIX");
        public static System.Runtime.InteropServices.OSPlatform HPUX = System.Runtime.InteropServices.OSPlatform.Create("HPUX");
        public static System.Runtime.InteropServices.OSPlatform HAIKU = System.Runtime.InteropServices.OSPlatform.Create("HAIKU");


        public OperatingSystemType OperatingSystemType
        {
            get
            {
#if UNITY_5
                return OperatingSystemType.Unity5;
#endif
                if (GetType() == typeof(BSDOperatingSystemInfo))
                    return OperatingSystemType.BSD;

                if (GetType() == typeof(LinuxOperatingSystemInfo))
                    return OperatingSystemType.Linux;

                if (GetType() == typeof(MacOSXOperatingSystemInfo))
                    return OperatingSystemType.MacOSX;

                if (GetType() == typeof(WindowsOperatingSystemInfo))
                    return OperatingSystemType.Windows;

                return OperatingSystemType.Other;
            }
        }

        /// <summary>
        /// Could be 16-bit 32-bit, 64-bit, ARM.
        /// </summary>
        public abstract string Architecture { get; }

        /// <summary>
        /// Operating System name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// .NET runtime.
        /// </summary>
        public virtual string Runtime => System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

        public bool IsMono => Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Java version.
        /// </summary>
        public abstract Version JavaVersion { get; }


        public abstract HardwareInfo Hardware { get; }


        public abstract OperatingSystemInfo Update();


        public static OperatingSystemInfo GetOperatingSystemInfo()
        {
#if UNITY_5
            return new UnityOperatingSystemInfo();
#endif
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(FreeBSD) ||
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(NetBSD) ||
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OpenBSD) ||
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(AIX) ||
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(HPUX))
                return new BSDOperatingSystemInfo();

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                return new LinuxOperatingSystemInfo();

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                return new MacOSXOperatingSystemInfo();

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                return new WindowsOperatingSystemInfo();

            return new OtherOperatingSystemInfo();
            //throw new NotSupportedException("Platform not supported!");
        }
    }
}