#include "pch.h"
#include "DisplayHelper.h"
#include <vector>
#include <Windows.h>

static bool GetSourceName(const DISPLAYCONFIG_PATH_INFO& path, DISPLAYCONFIG_SOURCE_DEVICE_NAME& name)
{
    auto ph = &name.header;

    *ph =
    {
        DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME,
        sizeof(name),
        path.sourceInfo.adapterId,
        path.sourceInfo.id
    };

    return DisplayConfigGetDeviceInfo(ph) == ERROR_SUCCESS;
}

static bool GetTargetName(const DISPLAYCONFIG_PATH_INFO& path, DISPLAYCONFIG_TARGET_DEVICE_NAME& name)
{
    auto ph = &name.header;

    *ph =
    {
        DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME,
        sizeof(name),
        path.targetInfo.adapterId,
        path.targetInfo.id
    };
    
    return DisplayConfigGetDeviceInfo(ph) == ERROR_SUCCESS;
}

static double GetRefreshRate(const DISPLAYCONFIG_PATH_INFO& path, const std::vector<DISPLAYCONFIG_MODE_INFO>& modes)
{
    if (path.targetInfo.modeInfoIdx == DISPLAYCONFIG_PATH_MODE_IDX_INVALID)
    {
        return 0.0;
    }

    DISPLAYCONFIG_RATIONAL rate = {};
    const auto& targetModeInfo = modes[path.targetInfo.modeInfoIdx];

    if (targetModeInfo.targetMode.targetVideoSignalInfo.vSyncFreq.Denominator == 0)
    {
        rate = path.targetInfo.refreshRate;
    }
    else
    {
        rate = targetModeInfo.targetMode.targetVideoSignalInfo.vSyncFreq;
    }

    if (rate.Denominator != 0)
    {
        return static_cast<double>(rate.Numerator) / static_cast<double>(rate.Denominator);
    }

    return 0.0;
}

static bool FillDeviceId(LPCWSTR dosPath, DISPLAY_DEVICEW& dd, LPCWSTR& did)
{
    if (EnumDisplayDevicesW(dosPath, 0, &dd, 0) && *dd.DeviceID)
    {
        did = dd.DeviceID;
        return true;
    }

    return false;
}

static bool CcdEnumDisplays(EnumDisplayProc lpfnEnum)
{
    UINT32 pathCount = 0;
    UINT32 modeCount = 0;

    if (GetDisplayConfigBufferSizes(QDC_ONLY_ACTIVE_PATHS, &pathCount, &modeCount) != ERROR_SUCCESS || pathCount == 0)
    {
        return false;
    }

    std::vector<DISPLAYCONFIG_PATH_INFO> paths(pathCount);
    std::vector<DISPLAYCONFIG_MODE_INFO> modes(modeCount);

    if (QueryDisplayConfig(QDC_ONLY_ACTIVE_PATHS, &pathCount, paths.data(), &modeCount, modes.data(), nullptr) != ERROR_SUCCESS || pathCount == 0)
    {
        return false;
    }

    int index = 0;

    for (const auto& path : paths)
    {
        if (!(path.flags & DISPLAYCONFIG_PATH_ACTIVE))
        {
            continue;
        }

        SystemDisplay info = { index };
        DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceName = {};
        DISPLAYCONFIG_TARGET_DEVICE_NAME targetName = {};
        DISPLAY_DEVICEW dd = { sizeof(dd) };
        UINT32 srcIdx = path.sourceInfo.modeInfoIdx;

        if (srcIdx < modeCount && modes[srcIdx].infoType == DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE)
        {
            info.position = modes[srcIdx].sourceMode.position;
            info.width = modes[srcIdx].sourceMode.width;
            info.height = modes[srcIdx].sourceMode.height;
        }

        if (GetSourceName(path, sourceName) && *sourceName.viewGdiDeviceName)
        {
            info.dosPath = sourceName.viewGdiDeviceName;
        }

        if (GetTargetName(path, targetName) && *targetName.monitorFriendlyDeviceName)
        {
            info.deviceName = targetName.monitorFriendlyDeviceName;
        }

        info.refreshRate = GetRefreshRate(path, modes);
        FillDeviceId(info.dosPath, dd, info.deviceId);

        if (!lpfnEnum(info))
        {
            return true;
        }

        index++;
    }

    return true;
}

static BOOL GdiEnumDisplays(EnumDisplayProc lpfnEnum)
{
    EnumDisplayData data = { lpfnEnum, 0 };

    return EnumDisplayMonitors(nullptr, nullptr, [](HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData) -> BOOL
    {
        auto* pdata = reinterpret_cast<EnumDisplayData*>(dwData);

        SystemDisplay info = { pdata->index };
        DISPLAY_DEVICEW dd = { sizeof(dd) };
        MONITORINFOEXW mi = { sizeof(MONITORINFOEXW) };

        if (GetMonitorInfoW(hMonitor, reinterpret_cast<MONITORINFO*>(&mi)))
        {
            info.dosPath = mi.szDevice;
            info.position = { mi.rcMonitor.left, mi.rcMonitor.top };
            info.width = mi.rcMonitor.right - mi.rcMonitor.left;
            info.height = mi.rcMonitor.bottom - mi.rcMonitor.top;

            DEVMODEW dm = { sizeof(dm) };

            if (EnumDisplaySettingsW(mi.szDevice, ENUM_CURRENT_SETTINGS, &dm))
            {
                info.refreshRate = static_cast<double>(dm.dmDisplayFrequency);
            }

            if (FillDeviceId(mi.szDevice, dd, info.deviceId))
            {
                info.deviceName = dd.DeviceString;
            }

            if (!pdata->lpfnEnum(info))
            {
                return FALSE;
            }

            pdata->index++;
        }

        return TRUE;
    }, reinterpret_cast<LPARAM>(&data));
}

BOOL EnumSystemDisplays(EnumDisplayProc lpfnEnum)
{
    if (!lpfnEnum)
    {
        return FALSE;
    }

    if (CcdEnumDisplays(lpfnEnum))
    {
        return TRUE;
    }

    return GdiEnumDisplays(lpfnEnum);
}