// LZX.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "LZX.h"


// This is an example of an exported variable
LZX_API int nLZX=0;

// This is an example of an exported function.
LZX_API int fnLZX(void)
{
	DWORD pcbDataBlockMax = 0x8000;
	LZX_DECOMPRESS pvConfiguration;

	pvConfiguration.WindowSize = 0x20000;
	pvConfiguration.CpuType = 1;

	DWORD unknown = 0;
	DWORD pcbDecompressed = 0;

	DWORD result = LDICreateDecompression(&pcbDataBlockMax, &pvConfiguration, NULL, NULL, (void*)0x370000, &unknown, &pcbDecompressed);

    return result;
}

// This is the constructor of a class that has been exported.
// see LZX.h for the class definition
CLZX::CLZX()
{
    return;
}
