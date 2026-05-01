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

    private static readonly string[] m_tests =
    [
        "http://g.cn/generate_204",
        "http://connectivitycheck.gstatic.com/generate_204",
        "http://connect.rom.miui.com/generate_204"
    ];

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

    private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
    {
        CheckAndInvokeAsync();
    }

    private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
    {
        CheckAndInvokeAsync();
    }

    private static async Task<bool> CheckConnectivityAsync()
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
        {
            return false;
        }

        foreach (var url in m_tests)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Head, url);
                using var res = await HttpService.SendAsync(req);

                if (res.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
            }
            catch
            {
                continue;
            }
        }

        return false;
    }
}