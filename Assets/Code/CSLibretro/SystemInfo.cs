﻿using System;
using System.Runtime.InteropServices;

namespace CSLibretro
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemInfo
    {
        //[MarshalAs(UnmanagedType.LPStr)] public string LibraryName; // <-- I still think there is a way to make this work (, CharSet = CharSet.Ansi)
        public IntPtr LibraryNamePointer;
        public IntPtr LibraryVersionPointer;
        public IntPtr ValidExtensionsPointer;
        public bool NeedFullpath;
        public bool BlockExtract;
        public string LibraryName;
        public string LibraryVersion;
        public string ValidExtensions;
    }
}
