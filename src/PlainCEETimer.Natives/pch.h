#ifndef PCH_H
#define PCH_H
#include "framework.h"
#endif 
#include <sdkddkver.h>
#include "resource.h"
#define cexport(type) extern "C" __declspec(dllexport) type WINAPI