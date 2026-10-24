using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PlainCEETimer.Modules.Http;

public class Downloader
{
    public event DownloadingEventHandler Downloading;
    public event Action<Exception> Error;
    public event Action Completed;

    public async Task DownloadAsync(string url, string savePath, CancellationToken token, long expected = 0, int defaultBuffer = 1024 * 16)
    {
        try
        {
            var report = new DownloadReport();
            using var response = await HttpService.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();
            var length = response.Content.Headers.ContentLength ?? 0;
            var total = length > 0 ? length : -1L;

            report.Total = total;
            using var file = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, defaultBuffer, true);
            using var http = await response.Content.ReadAsStreamAsync();
            OnDownloading(report);
            var buffer = new byte[defaultBuffer];
            var downloaded = 0L;
            var last = 0L;
            var read = 0;
            var sw = Stopwatch.StartNew();
            var lastReport = sw.Elapsed;
            var elapsed = new TimeSpan();

            while ((read = await http.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
            {
                await file.WriteAsync(buffer, 0, read, token);
                downloaded += read;
                elapsed = sw.Elapsed;
                report.Downloaded = downloaded;
                report.Progress = total > 0 ? (int)(downloaded * 100 / total) : -1;
                report.Speed = (downloaded - last) / (elapsed - lastReport).TotalSeconds;
                last = downloaded;
                lastReport = elapsed;
                OnDownloading(report);

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