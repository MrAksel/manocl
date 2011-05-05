namespace ManOCL.Native
{
    using System;

    public enum DeviceInfo : uint
    {
        AddressBits = 0x100d,
        Available = 0x1027,
        CompilerAvailable = 0x1028,
        DriverVersion = 0x102d,
        EndianLittle = 0x1026,
        ErrorCorrectionSupport = 0x1024,
        ExecutionCapabilities = 0x1029,
        Extensions = 0x1030,
        GlobalMemCacheLineSize = 0x101d,
        GlobalMemCacheSize = 0x101e,
        GlobalMemCacheType = 0x101c,
        GlobalMemSize = 0x101f,
        Image2DMaxHeight = 0x1012,
        Image2DMaxWidth = 0x1011,
        Image3DMaxDepth = 0x1015,
        Image3DMaxHeight = 0x1014,
        Image3DMaxWidth = 0x1013,
        ImageSupport = 0x1016,
        LocalMemSize = 0x1023,
        LocalMemType = 0x1022,
        MaxClockFrequency = 0x100c,
        MaxComputeUnits = 0x1002,
        MaxConstantArgs = 0x1021,
        MaxConstantBufferSize = 0x1020,
        MaxMemAllocSize = 0x1010,
        MaxParameterSize = 0x1017,
        MaxReadImageArgs = 0x100e,
        MaxSamplers = 0x1018,
        MaxWorkGroupSize = 0x1004,
        MaxWorkItemDimensions = 0x1003,
        MaxWorkItemSizes = 0x1005,
        MaxWriteImageArgs = 0x100f,
        MemBaseAddrAlign = 0x1019,
        MinDataTypeAlignSize = 0x101a,
        Name = 0x102b,
        Platform = 0x1031,
        PreferredVectorWidthChar = 0x1006,
        PreferredVectorWidthDouble = 0x100b,
        PreferredVectorWidthFloat = 0x100a,
        PreferredVectorWidthInt = 0x1008,
        PreferredVectorWidthLong = 0x1009,
        PreferredVectorWidthShort = 0x1007,
        Profile = 0x102e,
        ProfilingTimerResolution = 0x1025,
        QueueProperties = 0x102a,
        SingleFPConfig = 0x101b,
        Type = 0x1000,
        Vendor = 0x102c,
        VendorID = 0x1001,
        Version = 0x102f
    }
}
