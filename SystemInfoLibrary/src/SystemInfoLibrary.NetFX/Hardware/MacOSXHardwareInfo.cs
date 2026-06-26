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
using System.Collections.Generic;
using System.Linq;
using SystemInfoLibrary.Hardware.GPU;

namespace SystemInfoLibrary.Hardware
{
    internal class MacOSXHardwareInfo : BSDHardwareInfo
    {
        private IList<GPUInfo> _GPUs;

        public override IList<GPUInfo> GPUs
        {
            get
            {
                if (_GPUs == null)
                {
                    var chipsetVendors = Utils.GetCommandExecutionOutput("system_profiler",
                            "SPDisplaysDataType | grep 'Chipset Model' | awk -F \": \" '{ print $2 }'")
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    var vrams = Utils.GetCommandExecutionOutput("system_profiler",
                            "SPDisplaysDataType | grep 'VRAM' | awk -F \": \" '{ print $2 }'")
                        .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    var zip = chipsetVendors.Zip(vrams, (chipsetVendor, vram) => new[] { chipsetVendor, vram })
                        .ToArray();

                    _GPUs = zip.Select(info => (GPUInfo)new MacOSXGPUInfo(info)).ToList();
                }

                return _GPUs;
            }
        }
    }
}