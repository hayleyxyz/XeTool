// XeLibNativeTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <Windows.h>
#include <xcompress.h>

int (WINAPIV * __vsnprintf)(char *, size_t, const char*, va_list) = _vsnprintf;

// Compression block size
const DWORD     g_dwCompressionBlock = 0x8000;

// Sample file names and size
const CHAR*     g_strCompressibleFile = "c:\\temp\\compressible.bin";
const CHAR*     g_strUncompressibleFile = "c:\\temp\\uncompressible.bin";

const CHAR*     g_strCompressibleDestFile = "c:\\temp\\compressible.lzx";
const CHAR*     g_strUncompressibleDestFile = "c:\\temp\\uncompressible.lzx";

const CHAR*     g_strTransparentCompressedFile = "c:\\temp\\Compression.lzx";

const DWORD     g_dwFileSize = 0xFFFFF;

BOOL CompressFile(const CHAR* strInFileName, const CHAR* strOutFileName, DWORD dwBlockSize)
{
	HRESULT hr;
	BOOL fSucceeded = TRUE;
	HANDLE hInFile = INVALID_HANDLE_VALUE;
	HANDLE hOutFile = INVALID_HANDLE_VALUE;
	BYTE* pBlockSrc = NULL;
	XCOMPRESS_FILE_HEADER_LZXNATIVE FileHeader = { 0 };
	XCOMPRESS_BLOCK_HEADER_LZXNATIVE* pBlockHeader = NULL;
	XMEMCOMPRESSION_CONTEXT CompressionContext = { 0 };
	BOOL fContextCreated = FALSE;

	hInFile = CreateFile(strInFileName, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, NULL);
	if (hInFile == INVALID_HANDLE_VALUE)
	{
		fSucceeded = FALSE;
		goto BAIL_ON_FAILURE;
	}

	// Populate file header
	if (dwBlockSize < 32768)
		dwBlockSize = 32768;
	FileHeader.Common.Identifier = XCOMPRESS_FILE_IDENTIFIER_LZXNATIVE;
	FileHeader.Common.Version = XCOMPRESS_SET_FILE_VERSION(XCOMPRESS_LZXNATIVE_VERSION_MAJOR,
		XCOMPRESS_LZXNATIVE_VERSION_MINOR);
	FileHeader.CodecParams.WindowSize = dwBlockSize;
	FileHeader.CodecParams.CompressionPartitionSize = dwBlockSize;
	LARGE_INTEGER FileSize;
	GetFileSizeEx(hInFile, (LARGE_INTEGER*)&FileSize);
	FileHeader.UncompressedSizeLow = FileSize.LowPart;
	FileHeader.UncompressedSizeHigh = FileSize.HighPart;
	FileHeader.UncompressedBlockSize = dwBlockSize;

	// Create compression context
	hr = XMemCreateCompressionContext(XMEMCODEC_LZX, &FileHeader.CodecParams,
		FileHeader.ContextFlags, &CompressionContext);
	if (FAILED(hr))
	{
		fSucceeded = FALSE;
		goto BAIL_ON_FAILURE;
	}
	fContextCreated = TRUE;

	// Create block header to be twice as big as the block size.
	// This ensures that if by chance, the compressed data is larger
	// then the source data, the buffer will not have to be reallocated.
	pBlockHeader = (XCOMPRESS_BLOCK_HEADER_LZXNATIVE*)malloc((dwBlockSize * 2) + sizeof
		(XCOMPRESS_BLOCK_HEADER_LZXNATIVE));
	if (pBlockHeader == NULL)
	{
		fSucceeded = FALSE;
		goto BAIL_ON_FAILURE;
	}

	// Create source buffer
	pBlockSrc = (BYTE*)malloc(dwBlockSize);
	if (pBlockSrc == NULL)
	{
		fSucceeded = FALSE;
		goto BAIL_ON_FAILURE;
	}

	// Create output compressed file
	hOutFile = CreateFile(strOutFileName, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_FLAG_SEQUENTIAL_SCAN, NULL);
	if (hOutFile == INVALID_HANDLE_VALUE)
	{
		fSucceeded = FALSE;
		goto BAIL_ON_FAILURE;
	}

	// Start compressing the file. The file header will be written when
	// compression is completed.
	LARGE_INTEGER DistanceToMove;
	DistanceToMove.QuadPart = (LONGLONG)sizeof(XCOMPRESS_FILE_HEADER_LZXNATIVE);
	SetFilePointerEx(hOutFile, DistanceToMove, NULL, FILE_BEGIN);

	DWORD dwUncompressedBlockSize = 0;
	DWORD dwBytesWritten;
	ULARGE_INTEGER uBytesRemaining;
	ULARGE_INTEGER uCompressedSize;

	uCompressedSize.QuadPart = 0;

	for (uBytesRemaining.LowPart = FileHeader.UncompressedSizeLow,
		uBytesRemaining.HighPart = FileHeader.UncompressedSizeHigh;
		uBytesRemaining.QuadPart > 0ull;
		uBytesRemaining.QuadPart -= dwUncompressedBlockSize)
	{
		dwUncompressedBlockSize = (DWORD)min(uBytesRemaining.QuadPart,
			(ULONGLONG)FileHeader.UncompressedBlockSize);

		// Read uncompressed block
		DWORD dwBytesRead;
		if (!ReadFile(hInFile, pBlockSrc, dwUncompressedBlockSize, &dwBytesRead, NULL))
		{
			fSucceeded = FALSE;
			goto BAIL_ON_FAILURE;
		}

		// Compress block
		DWORD dwCompressedSize = dwBlockSize * 2;
		hr = XMemCompress(CompressionContext, pBlockHeader + 1, &dwCompressedSize, pBlockSrc,
			dwUncompressedBlockSize);
		if (FAILED(hr) || hr == XMCDERR_MOREDATA)
		{
			fSucceeded = FALSE;
			goto BAIL_ON_FAILURE;
		}

		// Update file header
		if (dwCompressedSize > FileHeader.CompressedBlockSizeMax)
			FileHeader.CompressedBlockSizeMax = dwCompressedSize;

		uCompressedSize.QuadPart += dwCompressedSize;

		// Write compressed block
		pBlockHeader->CompressedBlockSize = dwCompressedSize;

		if (!WriteFile(hOutFile, pBlockHeader, dwCompressedSize + sizeof(XCOMPRESS_BLOCK_HEADER_LZXNATIVE),
			&dwBytesWritten, NULL))
		{
			fSucceeded = FALSE;
			goto BAIL_ON_FAILURE;
		}
	}

	// Update file header

	FileHeader.CompressedSizeLow = uCompressedSize.LowPart;
	FileHeader.CompressedSizeHigh = uCompressedSize.HighPart;

	// Write the file header
	DistanceToMove.QuadPart = (LONGLONG)0;
	SetFilePointerEx(hOutFile, DistanceToMove, NULL, FILE_BEGIN);
	if (!WriteFile(hOutFile, &FileHeader, sizeof(XCOMPRESS_FILE_HEADER_LZXNATIVE), &dwBytesWritten, NULL))
	{
		fSucceeded = FALSE;
		goto BAIL_ON_FAILURE;
	}


BAIL_ON_FAILURE:
	if (pBlockHeader)
		free(pBlockHeader);

	if (pBlockSrc)
		free(pBlockSrc);

	if (fContextCreated)
		XMemDestroyCompressionContext(CompressionContext);

	if (hInFile != INVALID_HANDLE_VALUE)
		CloseHandle(hInFile);

	if (hOutFile != INVALID_HANDLE_VALUE)
	{
		CloseHandle(hOutFile);
		if (!fSucceeded)
			DeleteFile(strOutFileName);
	}
	return fSucceeded;
}

BOOL CreateFiles()
{
	printf("Creating test files...\n");

	HANDLE hFile;
	BYTE bBuffer[1024];

#if 0
	// Create compressible file
	hFile = CreateFile(g_strCompressibleFile, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_FLAG_SEQUENTIAL_SCAN,
		NULL);
	if (hFile == INVALID_HANDLE_VALUE)
	{
		return FALSE;
	}

	for (DWORD i = 0; i < g_dwFileSize; i += sizeof(bBuffer))
	{
		// Set all the bytes in the buffer the same for good compression
		memset(bBuffer, rand() % 0xFF, sizeof(bBuffer));

		DWORD dwWritten;
		if (!WriteFile(hFile, bBuffer, sizeof(bBuffer), &dwWritten, NULL))
		{
			CloseHandle(hFile);
			return FALSE;
		}
	}

	CloseHandle(hFile);
#endif

	// Create uncompressible file

	hFile = CreateFile(g_strUncompressibleFile, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, FILE_FLAG_SEQUENTIAL_SCAN,
		NULL);
	if (hFile == INVALID_HANDLE_VALUE)
	{
		return FALSE;
	}

	for (DWORD i = 0; i < g_dwFileSize; i += sizeof(bBuffer))
	{
		// Set the buffer to random values
		for (DWORD k = 0; k < sizeof(bBuffer); k++)
		{
			bBuffer[k] = (BYTE)(rand() % 0xFF);
		}

		DWORD dwWritten;
		if (!WriteFile(hFile, bBuffer, sizeof(bBuffer), &dwWritten, NULL))
		{
			CloseHandle(hFile);
			return FALSE;
		}
	}

	CloseHandle(hFile);

	return TRUE;
}

//--------------------------------------------------------------------------------------
// Name: GetFileSizeHelper
// Desc: Return the file size in bytes.
//--------------------------------------------------------------------------------------
DWORD GetFileSizeHelper(const CHAR* strFileName)
{
	HANDLE hFile = CreateFile(strFileName, GENERIC_READ, 0, NULL, OPEN_EXISTING, 0, NULL);
	if (hFile == INVALID_HANDLE_VALUE)
	{
		return 0;
	}

	LARGE_INTEGER FileSize;
	GetFileSizeEx(hFile, (LARGE_INTEGER*)&FileSize);

	CloseHandle(hFile);

	return FileSize.LowPart;
}

//--------------------------------------------------------------------------------------
// Name: RunTest
// Desc: Run compression and decompression test on test files.
//--------------------------------------------------------------------------------------
VOID RunTest(const CHAR* strInputFile, const CHAR* strDestinationFile)
{
	__int64 PerfFrequency, StartTime, EndTime;

	QueryPerformanceFrequency((LARGE_INTEGER*)&PerfFrequency);

	printf("\nCompressing file <%s>...\n", strInputFile);

	QueryPerformanceCounter((LARGE_INTEGER*)&StartTime);
	if (!CompressFile(strInputFile, strDestinationFile, g_dwCompressionBlock))
	{
		printf("Error: Compression failed.\n");
		return;
	}
	QueryPerformanceCounter((LARGE_INTEGER*)&EndTime);

	printf("Compression done. Time taken: %0.6f s. Compression: %0.2f%%.\n",
		(double)(EndTime - StartTime) / (double)PerfFrequency,
		100.0 * ((double)g_dwFileSize - (double)GetFileSizeHelper(strDestinationFile)) /
		(double)g_dwFileSize);

#if 0
	printf("Decompressing file <%s>...\n", strDestinationFile);

	QueryPerformanceCounter((LARGE_INTEGER*)&StartTime);
	if (!DecompressFile(strDestinationFile, strInputFile))
	{
		printf("Error: Decompression failed.\n");
		return;
	}
	QueryPerformanceCounter((LARGE_INTEGER*)&EndTime);

	printf("Decompression done. Time taken: %0.6f s.\n", (double)(EndTime - StartTime) / (double)
		PerfFrequency);


	QueryPerformanceFrequency((LARGE_INTEGER*)&PerfFrequency);

	printf("\nCompressing file (streaming) <%s>...\n", strInputFile);

	QueryPerformanceCounter((LARGE_INTEGER*)&StartTime);
	if (!CompressFileStreaming(strInputFile, strDestinationFile, g_dwCompressionBlock))
	{
		printf("Error: Compression failed.\n");
		return;
	}
	QueryPerformanceCounter((LARGE_INTEGER*)&EndTime);

	printf("Compression done. Time taken: %0.6f s. Compression: %0.2f%%.\n",
		(double)(EndTime - StartTime) / (double)PerfFrequency,
		100.0 * ((double)g_dwFileSize - (double)GetFileSizeHelper(strDestinationFile)) /
		(double)g_dwFileSize);

	printf("Decompressing file (streaming) <%s>...\n", strDestinationFile);

	QueryPerformanceCounter((LARGE_INTEGER*)&StartTime);
	if (!DecompressFileStreaming(strDestinationFile, strInputFile, g_dwCompressionBlock))
	{
		printf("Error: Decompression failed.\n");
		return;
	}
	QueryPerformanceCounter((LARGE_INTEGER*)&EndTime);

	printf("Decompression done. Time taken: %0.6f s.\n", (double)(EndTime - StartTime) / (double)
		PerfFrequency);
#endif
}

int main()
{
	// Create test files
	if (CreateFiles())
	{
		// Run compression tests
		RunTest(g_strCompressibleFile, g_strCompressibleDestFile);
		RunTest(g_strUncompressibleFile, g_strUncompressibleDestFile);
	}
	else
	{
		DebugBreak();
	}

    return 0;
}

