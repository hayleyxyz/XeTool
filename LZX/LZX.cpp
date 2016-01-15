// LZX.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "LZX.h"

LZX_API DWORD LZX_CreateDecompression(LPVOID* context, DWORD dataBlockMaxLength) {
	LZX_DECOMPRESS configuration;

	configuration.WindowSize = 0x20000;
	configuration.CpuType = 1;

	DWORD dstDataBlockMin = 0;

	DWORD result = LDICreateDecompression(&dataBlockMaxLength, &configuration, mem_alloc, mem_free, &dstDataBlockMin, context, NULL);

    return result;
}

LZX_API DWORD LZX_Decompress(LPVOID context, LPBYTE src, WORD srcLength, LPBYTE dst, LPDWORD dstLength) {
	return LDIDecompress(context, src, srcLength, dst, dstLength);
}

LZX_API DWORD LZX_DestroyDecompression(LPVOID context) {
	return LDIDestroyDecompression(context);
}