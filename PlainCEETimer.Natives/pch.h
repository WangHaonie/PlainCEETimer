#ifndef PCH_H
#define PCH_H
#include "framework.h"
#endif 
#include <sdkddkver.h>
#define cexport extern "C" __declspec(dllexport)
#define stdcall WINAPI