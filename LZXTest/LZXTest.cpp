// LZXTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>

int main()
{
	int result = CreateLZX();
	printf("0x%08x\n", result);
    return 0;
}

