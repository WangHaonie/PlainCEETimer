using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace PlainCEETimer.Modules.Http;

public class NetworkedAction(Action action)
{
    private int m_flag;

    public void Invoke()
    {
        if (action != null)
        {
            CheckAndInvokeAsync();
        }
    }

    private async void CheckAndInvokeAsync()
    {
        if (Interlocked.Exchange(ref m_flag, 1) == 0)
        {
            if (await CheckConnectivityAsync())
            {
                action();
                NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;
                NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
            }
            else
            {
                if (Interlocked.Exchange(ref m_flag, 0) == 1)
                {
                    NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
                    NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
                }
            }
        }
    }

    private async Task<bool> CheckConnectivityAsync()
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Head, "http://g.cn/generate_204");
            using var res = await HttpService.SendAsync(req);

            return res.IsSuccessStatusCode || res.StatusCode == HttpStatusCode.NoContent;
        }
        catch
        {
            return false;
        }
    }

    private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
    {
        CheckAndInvokeAsync();
    }

    private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
    {
        CheckAndInvokeAsync();
    }
}