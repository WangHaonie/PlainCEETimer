using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace PlainCEETimer.Modules.Http;

public class NetworkTaskScheduler
{
    private int m_flag;
    private readonly Action m_operation;

    public NetworkTaskScheduler(Action action)
    {
        if (action != null)
        {
            m_operation = action;
            CheckConnectivity();
        }
    }

    private async void CheckConnectivity()
    {
        if (Interlocked.Exchange(ref m_flag, 1) == 0)
        {
            if (await CheckConnectivityCore().ConfigureAwait(false))
            {
                m_operation();
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

    private async Task<bool> CheckConnectivityCore()
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Head, "http://www.gstatic.com/generate_204");
            using var res = await HttpService.SendAsync(req).ConfigureAwait(false);

            return res.IsSuccessStatusCode || res.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch
        {
            return false;
        }
    }

    private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
    {
        CheckConnectivity();
    }

    private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
    {
        CheckConnectivity();
    }
}