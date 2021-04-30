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

using System.Collections.Generic;
using SystemInfoLibrary.Hardware.CPU;
using SystemInfoLibrary.Hardware.GPU;
using SystemInfoLibrary.Hardware.RAM;

namespace SystemInfoLibrary.Hardware
{
    internal class BSDHardwareInfo : HardwareInfo
    {
        private IList<CPUInfo> _CPUs;

        private RAMInfo _RAM;

        public override IList<CPUInfo> CPUs =>
            _CPUs ?? (_CPUs = new List<CPUInfo> {new BSDCPUInfo()}); // We'll assume only one physical CPU is supported

        public override IList<GPUInfo> GPUs => new List<GPUInfo>();
        public override RAMInfo RAM => _RAM ?? (_RAM = new BSDRAMInfo());
    }
}