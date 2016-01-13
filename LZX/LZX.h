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
} LZX_DECOMPRESS, *PLZX_DECOMPRESS;

extern "C" {

	DWORD LDICreateDecompression(
		DWORD* pcbDataBlockMax,
		LZX_DECOMPRESS* pvConfiguration,
		DWORD pfnma,
		DWORD pfnmf,
		VOID* pcbSrcBufferMin,
		DWORD* unknow,
		DWORD* pcbDecompressed
	);
	
	DWORD LDIDecompress(
		DWORD context,
		BYTE* pbSrc,
		WORD cbSrc,
		BYTE* pdDst,
		DWORD* pcbDecompressed
	);
	
	DWORD LDIDestroyDecompression(DWORD contect);

}

// This class is exported from the LZX.dll
class LZX_API CLZX {
public:
	CLZX(void);
	// TODO: add your methods here.
};

extern LZX_API int nLZX;

extern "C" {

	LZX_API int fnLZX(void);

}