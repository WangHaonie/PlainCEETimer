using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PlainCEETimer.Modules.Http
{
    public class Downloader
    {
        public event Action<DownloadReport> Downloading;
        public event Action<Exception> Error;
        public event Action Completed;

        public async Task DownloadAsync(string url, string savePath, CancellationToken token, long contentLength, int defaultBuffer = 1024 * 16)
        {
            try
            {
                var report = new DownloadReport();
                using var response = await HttpService.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);
                response.EnsureSuccessStatusCode();
                var total = 0L;

                if ((total = response.Content.Headers.ContentLength ?? contentLength) == 0L)
                {
                    total = 378880L;
                }

                report.Total = total / 1024L;
                using var file = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None, defaultBuffer, true);
                using var http = await response.Content.ReadAsStreamAsync();
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
                    report.Downloaded = downloaded / 1024L;
                    report.Progress = (int)(downloaded * 100 / total);
                    report.Speed = (downloaded - last) / (elapsed - lastReport).TotalSeconds / 1024D;
                    last = downloaded;
                    lastReport = elapsed;
                    Downloading?.Invoke(report);

                    if (token.IsCancellationRequested)
                    {
                        file.Close();
                        http.Close();
                        File.Delete(savePath);
                        return;
                    }
                }

                report.Progress = 100;
                report.Speed = 0;
                Downloading?.Invoke(report);
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
    }
}
