// XeLibNative.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "XeLibNative.h"

typedef DWORD (__cdecl *LDIDecompress_fn)(LPVOID hmd, LPBYTE pbSrc, DWORD cbSrc, LPBYTE pdDst, LPDWORD pcbDecompressed);

XELIBNATIVE_API DWORD __cdecl LDIDecompress2(LPVOID hmd, LPBYTE pbSrc, DWORD cbSrc, LPBYTE pdDst, LPDWORD pcbDecompressed) {
	HMODULE mod = GetModuleHandle("XeLibNative.dll");

	LDIDecompress_fn fn;
	fn = (LDIDecompress_fn)GetProcAddress(mod, "LDIDecompress");

	fprintf(stdout, "hmd: 0x%08x\n", hmd);

	DWORD result = fn(hmd, pbSrc, cbSrc, pdDst, pcbDecompressed);

	return result;
}