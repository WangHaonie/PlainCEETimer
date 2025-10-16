#ifndef PCH_H
#define PCH_H
#include "framework.h"
#include "resource.h"
#include <sdkddkver.h>
#define cexport(ret) extern "C" __declspec(dllexport) ret WINAPI
#endif 