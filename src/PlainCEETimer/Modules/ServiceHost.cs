using System;
using Microsoft.Extensions.DependencyInjection;
using PlainCEETimer.Countdown;
using PlainCEETimer.UI;
using PlainCEETimer.UI.Core;

namespace PlainCEETimer.Modules;

public static class ServiceHost
{
    public static IServiceProvider ServiceProvider => field ??= Initialize();

    private static ServiceProvider Initialize()
    {
        var svc = new ServiceCollection()
            .AddSingleton(sp => CountdownManager.Instance.CountdownService)
            .AddSingleton<IDialogService, AppMessageBox>()
            .AddSingleton<ITrayIconLoader, AppTrayIconLoader>();

        return svc.BuildServiceProvider();
    }
}
