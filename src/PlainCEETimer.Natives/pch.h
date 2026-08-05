#ifndef PCH_H

#define PCH_H

#include "framework.h"
#include "resource.h"
#include <sdkddkver.h>

#define DeclDelegateType(Func)      using fn##Func = decltype(&Func)
#define DeclDelegateField(Func)     static fn##Func g_##Func = nullptr

#define cexport(ret)                extern "C" __declspec(dllexport) ret WINAPI
#define CastToP(t, v)               reinterpret_cast<t>(v)
#define CastToS(t, v)               static_cast<t>(v)

#define DetourBegin()	            DetourTransactionBegin(); DetourUpdateThread(GetCurrentThread())
#define DetourEnd()		            DetourTransactionCommit()
#define DETOUR_ARGS(original, hook) &(PVOID&)original, hook

#define DeclIatData(n, dn)          static IAT_HOOK_DATA<fn##n> IatHook##dn##n = {}
#define HookIat(args, data, original, hook)     \
            if (InitializeIatHook(args, data))  \
            {                                   \
                if (!original)                  \
                {                               \
                    original = data.OldFunc;    \
                }                               \
                ReplaceFunction(data, hook);    \
            }
#define UnhookIat(data)             RestoreFunction(data)

#define STRSAFE_DEFAULT             0

#endif 