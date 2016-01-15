#include "stdafx.h"

// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the LZX_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// LZX_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef LZX_EXPORTS
#define LZX_API __declspec(dllexport)
#else
#define LZX_API __declspec(dllimport)
#endif

typedef struct _LZX_DECOMPRESS {
	LONG WindowSize;
	LONG CpuType;
} LZX_DECOMPRESS, *PLZX_DECOMPRESS, *LPLZX_DECOMPRESS;

extern "C" {

	DWORD LDICreateDecompression(
		LPDWORD pcbDataBlockMax, // Max size of src blocks
		LPLZX_DECOMPRESS pvConfiguration,
		LPVOID pfnma,
		LPVOID pfnmf,
		LPDWORD pcbDstBufferMin, // Minimum size of dst buffer when calling decompress
		LPVOID* ppContext,
		LPVOID unknown
	);
	
	DWORD LDIDecompress(
		LPVOID context,
		LPBYTE pbSrc,
		WORD cbSrc,
		LPBYTE pdDst,
		LPDWORD pcbDecompressed
	);
	
	DWORD LDIDestroyDecompression(
		LPVOID context
	);

	FNALLOC(mem_alloc) {
		return malloc(cb);
	}

	FNFREE(mem_free) {
		free(pv);
	}

}

extern "C" {
	LZX_API DWORD LZX_CreateDecompression(LPVOID*, DWORD);
	LZX_API DWORD LZX_Decompress(LPVOID, LPBYTE, WORD, LPBYTE, LPDWORD);
	LZX_API DWORD LZX_DestroyDecompression(LPVOID);
}