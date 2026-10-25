using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules.Http;

[NoConstants]
public class Downloader
{
    private const ulong ReportIntervalMs = 300;

    public event DownloadingEventHandler Downloading;
    public event Action<Exception> Error;
    public event Action Completed;

    public async Task DownloadAsync(string url, string savePath, CancellationToken token, long expected = 0, int bufferSize = 1024 * 16)
    {
        try
        {
            var report = new DownloadReport();
            using var response = await HttpService.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();
            var length = response.Content.Headers.ContentLength ?? 0;
            var total = length > 0 ? length : -1L;

            report.Total = total;
            using var file = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true);
            using var http = await response.Content.ReadAsStreamAsync();
            OnDownloading(report);
            var buffer = new byte[bufferSize];
            var downloaded = 0L;
            var read = 0;
            var lastBytes = 0L;
            var lastTick = DateTime.TickCount;
            var speed = 0.0;
            var elapsed = 0UL;
            var now = 0UL;

            while ((read = await http.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
            {
                await file.WriteAsync(buffer, 0, read, token);
                downloaded += read;
                now = DateTime.TickCount;
                elapsed = now - lastTick;

                if (elapsed >= ReportIntervalMs)
                {
                    var v = (downloaded - lastBytes) * 1000.0 / elapsed;
                    speed = speed <= 0 ? v : speed * 0.7 + v * 0.3;
                    report.Downloaded = downloaded;
                    report.Progress = total > 0 ? (int)(downloaded * 100 / total) : -1;
                    report.Speed = speed;
                    lastBytes = downloaded;
                    lastTick = now;
                    OnDownloading(report);
                }

                if (token.IsCancellationRequested)
                {
                    file.Close();
                    http.Close();
                    File.Delete(savePath);
                    return;
                }
            }

            if (downloaded == 0 || (expected > 0 && downloaded != expected))
            {
                throw new InvalidDataException("当前文件可能已被损坏或不正确。");
            }

            report.Downloaded = downloaded;
            report.Progress = 100;
            report.Speed = 0;
            OnDownloading(report);
            Completed?.Invoke();
        }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested && ex is not TaskCanceledException)
            {
                Error?.Invoke(ex);
            }
        }
    }

    private void OnDownloading(DownloadReport report)
    {
        Downloading?.Invoke(this, report);
    }
}