// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the XELIBNATIVE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// XELIBNATIVE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef XELIBNATIVE_EXPORTS
#define XELIBNATIVE_API __declspec(dllexport)
#else
#define XELIBNATIVE_API __declspec(dllimport)
#endif

typedef LPVOID (__cdecl *pfnma)(DWORD);

typedef VOID (__cdecl *pfnmf)(LPVOID);

typedef DWORD (__cdecl *pfnlzx_output_callback)(LPVOID, LPBYTE, DWORD, DWORD);

extern "C" {
	
	XELIBNATIVE_API DWORD __cdecl LCICreateCompression(
		LPDWORD pcbDataBlockMax,
		LPVOID pvConfiguration,
		pfnma pfnma,
		pfnmf pfnmf,
		LPDWORD pcbDstBufferMin,
		LPVOID* pmchHandle,
		pfnlzx_output_callback pfnlzx_output_callback,
		LPVOID fci_data
	);
	
	XELIBNATIVE_API DWORD __cdecl LCICompress(
		LPVOID hmc,
		LPBYTE pbSrc,
		DWORD cbSrc,
		LPBYTE pbDst, // Not used
		DWORD cbDst,
		LPDWORD pcbResult
	);

	XELIBNATIVE_API DWORD __cdecl LCIFlushCompressorOutput(
		LPVOID hmc
	);

	XELIBNATIVE_API DWORD __cdecl LCIDestroyCompression(
		LPVOID hmc
	);

	XELIBNATIVE_API DWORD __cdecl LDICreateDecompression(
		LPDWORD pcbDataBlockMax,
		LPVOID pvConfiguration,
		LPVOID pfnma,
		LPVOID pfnmf,
		LPVOID pcbSrcBufferMin,
		LPVOID* ppContext,
		LPDWORD pcbDecompressed
	);

	XELIBNATIVE_API DWORD __cdecl LDIDecompress(
		LPVOID hmd,
		LPBYTE pbSrc,
		DWORD cbSrc,
		LPBYTE pdDst,
		LPDWORD pcbDecompressed
	);

	XELIBNATIVE_API DWORD __cdecl LDIDestroyDecompression(
		LPVOID hmd
	);
}

LPVOID __cdecl mem_alloc(DWORD cb) {
	return malloc(cb);
}

VOID __cdecl mem_free(void* pv) {
	free(pv);
}
