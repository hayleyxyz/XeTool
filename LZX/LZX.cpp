// LZX.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "LZX.h"


// This is an example of an exported variable
LZX_API int nLZX=0;

// This is an example of an exported function.
LZX_API int fnLZX(void)
{
    return 42;
}

// This is the constructor of a class that has been exported.
// see LZX.h for the class definition
CLZX::CLZX()
{
    return;
}
