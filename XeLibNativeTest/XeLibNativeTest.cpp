// XeLibNativeTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>
#include "../XeLibNative/XeLibNative.h"

#define CheckResult(x)	printf(x"(): 0x%08x\n", result); \
						if (result != 0) { \
							DebugBreak(); \
							return 0; \
						}

const char g_Kernel[] = "C:\\temp\\xboxkrnl.exe";

const char g_KernelCompressed[] = "C:\\temp\\xboxkrnl.lzx";

int main()
{
	FILE* fh = NULL;

	fopen_s(&fh, g_Kernel, "rb");

	LPBYTE uncompressedBuf = new BYTE[0x8000];

	if (fh) {
		fread(uncompressedBuf, 0x8000, 1, fh);
		fclose(fh);
	}
	else {
		DebugBreak();
		return 0;
	}

	LPVOID lci = NULL;
	DWORD result = NULL;
	DWORD minDecompressedBufLength = NULL;

	result = CreateCompression(&lci, 0x8000, &minDecompressedBufLength);

	CheckResult("CreateCompression");

	LPBYTE compressedBuf = new BYTE[minDecompressedBufLength];
	memset(compressedBuf, 0xDF, minDecompressedBufLength);

	DWORD compressedLength = NULL;

	result = Compress(lci, uncompressedBuf, 0x8000, compressedBuf, minDecompressedBufLength, &compressedLength);

	CheckResult("Compress");

	DestroyCompression(lci);

	fopen_s(&fh, g_KernelCompressed, "wb");

	if (fh) {
		fwrite(compressedBuf, compressedLength, 1, fh);
		fclose(fh);
	}
	else {
		DebugBreak();
		return 0;
	}

	system("pause");

    return 0;
}

