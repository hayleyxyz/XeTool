// LZX.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "LZX.h"

// This is an example of an exported function.
LZX_API DWORD CreateLZX(VOID)
{
	DWORD pcbDataBlockMax = 0x8000;
	LZX_DECOMPRESS pvConfiguration;

	pvConfiguration.WindowSize = 0x20000;
	pvConfiguration.CpuType = 1;

	DWORD unknown = 0;
	DWORD pcbDecompressed = 0;

	LPVOID mem = VirtualAlloc(NULL, 0x3000, MEM_COMMIT, PAGE_READWRITE);

	DWORD result = LDICreateDecompression(&pcbDataBlockMax, &pvConfiguration, Kmem_alloc, Kmem_free, mem, &unknown, &pcbDecompressed);

    return result;
}
