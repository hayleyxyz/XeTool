// XeLibNative.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "XeLibNative.h"


LPVOID __cdecl mem_alloc(DWORD cb) {
	return malloc(cb);
}

VOID __cdecl mem_free(void* pv) {
	free(pv);
}