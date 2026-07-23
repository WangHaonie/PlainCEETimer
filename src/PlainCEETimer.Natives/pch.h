#ifndef PCH_H
#define PCH_H
#include "framework.h"
#include "resource.h"
#include <sdkddkver.h>
#define cexport(ret) extern "C" __declspec(dllexport) ret WINAPI
#define CastToP(t, v) reinterpret_cast<t>(v)
#define CastToS(t, v) static_cast<t>(v)
#endif 