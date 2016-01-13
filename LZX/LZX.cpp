// LZX.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "LZX.h"

// This is an example of an exported function.
LZX_API DWORD LZX_Create(LPVOID* context)
{
	DWORD dataBlockMax = 0x8000;
	LZX_DECOMPRESS configuration;

	configuration.WindowSize = 0x20000;
	configuration.CpuType = 1;

	//LPVOID context = NULL;
	DWORD decompressedCount = 0;

	//LPVOID mem = VirtualAlloc(NULL, 0x3000, MEM_COMMIT, PAGE_READWRITE);

	DWORD result = LDICreateDecompression(&dataBlockMax, &configuration, Kmem_alloc, Kmem_free, &decompressedCount, context, NULL);

    return result;
}

LZX_API DWORD LZX_Decompress(LPVOID context, LPBYTE src, WORD srcLength, LPBYTE dst, LPDWORD dstLength) {
	return LDIDecompress(context, src, srcLength, dst, dstLength);
}

LZX_API DWORD LZX_Destroy(LPVOID context) {
	return LDIDestroyDecompression(context);
}