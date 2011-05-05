﻿using System;

namespace ManOCL
{
    public enum DeviceType : ulong
    {
        Default = 1L,
        CPU = 2L,
        GPU = 4L,
        Accelerator = 8L,
        All = 0xffffffffL
    }
}
