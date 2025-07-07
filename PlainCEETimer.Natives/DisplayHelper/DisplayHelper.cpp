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
                vector<const DISPLAYCONFIG_SOURCE_MODE*> sourceModes(modeCount, nullptr);

                for (UINT32 i = 0; i < modeCount; ++i)
                {
                    if (modes[i].infoType == DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE)
                    {
                        sourceModes[i] = &modes[i].sourceMode;
                    }
                }

                for (const auto& path : paths)
                {
                    if (!(path.flags & DISPLAYCONFIG_PATH_ACTIVE))
                    {
                        continue;
                    }

                    RECT rect = {};

                    if (path.sourceInfo.modeInfoIdx < modeCount)
                    {
                        if (auto srcMode = sourceModes[path.sourceInfo.modeInfoIdx])
                        {
                            rect =
                            {
                                srcMode->position.x,
                                srcMode->position.y,
                                srcMode->position.x + static_cast<LONG>(srcMode->width),
                                srcMode->position.y + static_cast<LONG>(srcMode->height)
                            };
                        }
                    }

                    DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceName = 
                    {
                        {
                            DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME,
                            sizeof(sourceName),
                            path.sourceInfo.adapterId,
                            path.sourceInfo.id
                        }
                    };

                    LPCWSTR dpath = L"<未知路径>";

                    if (DisplayConfigGetDeviceInfo(reinterpret_cast<DISPLAYCONFIG_DEVICE_INFO_HEADER*>(&sourceName)) == ERROR_SUCCESS && *sourceName.viewGdiDeviceName)
                    {
                        dpath = sourceName.viewGdiDeviceName;
                    }

                    DISPLAYCONFIG_TARGET_DEVICE_NAME targetName =
                    {
                        {
                            DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME,
                            sizeof(targetName),
                            path.targetInfo.adapterId,
                            path.targetInfo.id
                        }
                    };

                    LPCWSTR name = L"<未知名称>";

                    if (DisplayConfigGetDeviceInfo(reinterpret_cast<DISPLAYCONFIG_DEVICE_INFO_HEADER*>(&targetName)) == ERROR_SUCCESS && *targetName.monitorFriendlyDeviceName)
                    {
                        name = targetName.monitorFriendlyDeviceName;
                    }

                    DISPLAY_DEVICE dd = {};
                    dd.cb = sizeof(dd);
                    LPCWSTR did = L"<未知型号>";
                    
                    if (EnumDisplayDevices(dpath, 0, &dd, 0) && *dd.DeviceID)
                    {
                        did = dd.DeviceID;
                    }

                    lpfnEnum(rect, name, dpath, did);
                }
            }
        }
    }
}