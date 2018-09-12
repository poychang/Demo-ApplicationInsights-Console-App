using System;
using System.IO;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System.Net.Http;

namespace ApplicationInsightsConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 如果你用 .NET Framework 可以直接這樣設定 TelemetryClient
            // var telemetryClient = InitializeTelemetry();

            // 這裡明確指定設定檔路徑
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ApplicationInsights.config");
            var telemetryClient = InitializeTelemetry(configPath);

            // 傳送自訂訊息的遙測資料
            telemetryClient.TrackTrace("Hello World!");

            using (var httpClient = new HttpClient())
            {
                // 因為有設定 DependencyCollector 所以會自動追蹤 HTTP 呼叫
                httpClient.GetAsync("https://microsoft.com").Wait();
            }

            // 強制排清 TelemetryClient 緩衝區內的訊息
            telemetryClient.Flush();
            System.Threading.Thread.Sleep(3000);
        }

        // 透過這個自訂的方法，方便我們初始化 TelemetryClient
        private static TelemetryClient InitializeTelemetry(string configPath = null)
        {
            // 這裡會判斷是否使用明確指定設定檔路徑，若有指定，則使用自訂的設定檔
            var configuration = string.IsNullOrEmpty(configPath)
                ? TelemetryConfiguration.Active
                : TelemetryConfiguration.CreateFromConfiguration(File.ReadAllText(configPath));

            return new TelemetryClient(configuration);
        }
    }
}
