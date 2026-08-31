#ifndef PCH_H

#define PCH_H

#include "framework.h"
#include "resource.h"
#include <sdkddkver.h>

#define LIBRARYNAME 				L"PlainCEETimer.Natives.dll"

#define COMCTL32_DLL                "comctl32.dll"
#define COMDLG32_DLL                "comdlg32.dll"
#define USER32_DLL                  "user32.dll"
#define UXTHEME_DLL                 "uxtheme.dll"

#define ORD2STR(ord)                MAKEINTRESOURCEA(ord)

#define DeclDelegateType(Func)      using fn##Func = decltype(&Func)
#define DeclDelegateField(Func)     static fn##Func g_##Func = nullptr

#define NATIVES_EXPORT              extern "C"
#define NATIVESAPI                  __stdcall
#define CastP(t, v)                 reinterpret_cast<t>(v)
#define CastS(t, v)                 static_cast<t>(v)

#define DetourBegin()	            DetourTransactionBegin(); DetourUpdateThread(GetCurrentThread())
#define DetourEnd()		            DetourTransactionCommit()
#define DETOUR_ARGS(original, hook) &(PVOID&)original, hook
#define DetourHook(o, h)            DetourAttach(DETOUR_ARGS(o, h))
#define DetourUnhook(o, h)          DetourDetach(DETOUR_ARGS(o, h))

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

#define nameof(obj)                 #obj

#endif 