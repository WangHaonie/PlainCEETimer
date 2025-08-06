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
                int index = 0;
                vector<const DISPLAYCONFIG_SOURCE_MODE*> sourceModes(modeCount, nullptr);

                for (UINT32 i = 0; i < modeCount; i++)
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

                    UINT32 sourceModeInfoIdx = path.sourceInfo.modeInfoIdx;
                    SystemDisplay info = { index };

                    if (sourceModeInfoIdx < modeCount)
                    {
                        if (auto srcMode = sourceModes[sourceModeInfoIdx])
                        {
                            info.bounds =
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

                    info.dosPath = dpath;

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

                    info.deviceName = name;

                    double refrate = 0.0;

                    if (path.targetInfo.modeInfoIdx != DISPLAYCONFIG_PATH_MODE_IDX_INVALID)
                    {
                        DISPLAYCONFIG_RATIONAL rate = {};
                        auto mode = &modes[sourceModeInfoIdx];

                        if (mode->targetMode.targetVideoSignalInfo.vSyncFreq.Denominator == 0)
                        {
                            rate = path.targetInfo.refreshRate;
                        }
                        else
                        {
                            rate = mode->targetMode.targetVideoSignalInfo.vSyncFreq;
                        }

                        refrate = static_cast<double>(rate.Numerator) / static_cast<double>(rate.Denominator);
                    }

                    info.refreshRate = refrate;

                    LPCWSTR did = L"<未知型号>";
                    DISPLAY_DEVICE dd = { sizeof(dd) };
                    
                    if (EnumDisplayDevices(dpath, 0, &dd, 0) && *dd.DeviceID)
                    {
                        did = dd.DeviceID;
                    }

                    info.deviceId = did;

                    if (!(lpfnEnum(info)))
                    {
                        return;
                    }

                    index++;
                }
            }
        }
    }
}