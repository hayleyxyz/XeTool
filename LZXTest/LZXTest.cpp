// LZXTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>
#include <intrin.h>

int main()
{
	WORD packedSize = 0;
	WORD unpackedSize = 0;
	DWORD unpackedSizeDw = 0;

	FILE* fh = NULL;

	fopen_s(&fh, "C:\\Users\\Oscar\\Desktop\\ShadowBoot\\Decrypted\\SE.bin", "rb");

	if (fh) {
		fseek(fh, 0x30, SEEK_SET);

		fread(&packedSize, 2, 1, fh);
		fread(&unpackedSize, 2, 1, fh);

		packedSize = _byteswap_ushort(packedSize);
		unpackedSize = _byteswap_ushort(unpackedSize);
		unpackedSizeDw = (unpackedSize & 0xffff);

		LPBYTE compressed = new BYTE[packedSize];
		LPBYTE decompressed = new BYTE[unpackedSize];

		fread(compressed, packedSize, 1, fh);
		fclose(fh);

		LPVOID context = NULL;
		DWORD result = LZX_CreateDecompression(&context, 0x8000);
		printf("LZX_CreateDecompression() - 0x%08x\n", result);

		result: LZX_Decompress(context, compressed, packedSize, decompressed, &unpackedSizeDw);
		printf("LZX_Decompress() - 0x%08x\n", result);

		result = LZX_DestroyDecompression(context);
		printf("LZX_DestroyDecompression() - 0x%08x\n", result);
	}
	else {
		__debugbreak();
	}

    return 0;
}

