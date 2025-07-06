#include "pch.h"
#include "DisplayHelper.h"
#include <vector>
#include <map>

using namespace std;

void EnumSystemDisplays(EnumDisplayProc lpfnEnum)
{
    if (lpfnEnum)
    {
        UINT32 pathCount = 0;
        UINT32 modeCount = 0;

        if (GetDisplayConfigBufferSizes(QDC_ONLY_ACTIVE_PATHS, &pathCount, &modeCount) == ERROR_SUCCESS)
        {
            vector<DISPLAYCONFIG_PATH_INFO> paths(pathCount);
            vector<DISPLAYCONFIG_MODE_INFO> modes(modeCount);

            if (QueryDisplayConfig(QDC_ONLY_ACTIVE_PATHS, &pathCount, paths.data(), &modeCount, modes.data(), nullptr) == ERROR_SUCCESS)
            {
                map<UINT32, DISPLAYCONFIG_SOURCE_MODE> sourceModeMap;

                for (UINT32 i = 0; i < modeCount; ++i)
                {
                    if (modes[i].infoType == DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE)
                    {
                        sourceModeMap[i] = modes[i].sourceMode;
                    }
                }

                for (const auto& path : paths)
                {
                    if (!(path.flags & DISPLAYCONFIG_PATH_ACTIVE))
                    {
                        continue;
                    }

                    LONG x = 0;
                    LONG y = 0;
                    LONG w = 0;
                    LONG h = 0;
                    UINT32 index = path.sourceInfo.modeInfoIdx;
                    auto it = sourceModeMap.find(index);

                    if (it != sourceModeMap.end())
                    {
                        x = it->second.position.x;
                        y = it->second.position.y;
                        w = static_cast<LONG>(it->second.width);
                        h = static_cast<LONG>(it->second.height);
                    }

                    DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceName = {};
                    sourceName.header.type = DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
                    sourceName.header.size = sizeof(sourceName);
                    sourceName.header.adapterId = path.sourceInfo.adapterId;
                    sourceName.header.id = path.sourceInfo.id;

                    LPCWSTR dpath = L"<Unknown Path>";
                    if (DisplayConfigGetDeviceInfo(reinterpret_cast<DISPLAYCONFIG_DEVICE_INFO_HEADER*>(&sourceName)) == ERROR_SUCCESS)
                    {
                        dpath = sourceName.viewGdiDeviceName;
                    }

                    DISPLAYCONFIG_TARGET_DEVICE_NAME targetName = {};
                    targetName.header.type = DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
                    targetName.header.size = sizeof(targetName);
                    targetName.header.adapterId = path.targetInfo.adapterId;
                    targetName.header.id = path.targetInfo.id;

                    if (DisplayConfigGetDeviceInfo(reinterpret_cast<DISPLAYCONFIG_DEVICE_INFO_HEADER*>(&targetName)) == ERROR_SUCCESS)
                    {
                        lpfnEnum({ x, y, w + x, h + y }, targetName.monitorFriendlyDeviceName[0] ? targetName.monitorFriendlyDeviceName : L"<Unknown Model>", dpath);
                    }
                }
            }
        }
    }
}