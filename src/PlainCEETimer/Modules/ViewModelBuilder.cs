using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace PlainCEETimer.Modules;

public class ViewModelBuilder<TViewModel>(IServiceProvider provider)
    where TViewModel : class
{
    private readonly List<Action<TViewModel>> actions = [];

    public ViewModelBuilder<TViewModel> Import<TService>(Action<TViewModel, TService> action)
        where TService : notnull
    {
        var service = provider.GetRequiredService<TService>();
        actions.Add(vm => action(vm, service));
        return this;
    }

    public ViewModelBuilder<TViewModel> Import<TService>(TService service, Action<TViewModel, TService> action)
        where TService : notnull
    {
        actions.Add(vm => action(vm, service));
        return this;
    }

    public TViewModel Build()
    {
        var vm = ActivatorUtilities.CreateInstance<TViewModel>(provider);
        var init = vm as ISupportInitialize;
        var count = actions.Count;
        init?.BeginInit();

        for (int i = 0; i < count; i++)
        {
            actions[i](vm);
        }

        init?.EndInit();
        return vm;
    }
}
