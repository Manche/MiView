using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace MiView;

public static class Program
{
    public static void Main(string[] args)
    {
        // 引数でモードを判定
        if (args.Length > 0 && args[0] == "--console")
        {
            RunConsoleMode(args.Skip(1).ToArray());
        }
        else
        {
            // GUIモードで実行
            RunGUIMode(args);
        }
    }

    private static void RunConsoleMode(string[] args)
    {
        Console.WriteLine("=== MiView - Misskey Timeline Viewer ===");
        Console.WriteLine("コンソールモードで実行中");
        Console.WriteLine("");

        if (args.Length >= 2)
        {
            Console.WriteLine("WebSocket接続テストを実行中...");
            TestWebSocketConnection(args[0], args[1]).Wait();
        }
        else
        {
            Console.WriteLine("使用方法: MiView --console <instance_url> <api_key>");
            Console.WriteLine("GUIモード: MiView (引数なし)");
        }
    }

    private static void RunGUIMode(string[] args)
    {
        try
        {
            // Avaloniaアプリケーションを起動
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GUI起動エラー: {ex.Message}");
            Console.WriteLine("コンソールモードで実行してください: MiView --console");
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<AvaloniaApp>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static async Task TestWebSocketConnection(string instanceUrl, string apiKey)
    {
        try
        {
            using var ws = new ClientWebSocket();
            var wsUrl = $"wss://{instanceUrl}/streaming?i={apiKey}";
            
            Console.WriteLine($"接続中: {wsUrl}");
            await ws.ConnectAsync(new Uri(wsUrl), CancellationToken.None);
            
            if (ws.State == WebSocketState.Open)
            {
                Console.WriteLine("✓ WebSocket接続成功！");
                
                var subscribeMessage = JsonSerializer.Serialize(new
                {
                    type = "connect",
                    body = new
                    {
                        channel = "homeTimeline",
                        id = "test"
                    }
                });
                
                var buffer = Encoding.UTF8.GetBytes(subscribeMessage);
                await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                
                Console.WriteLine("✓ タイムライン購読成功");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Test complete", CancellationToken.None);
            }
            else
            {
                Console.WriteLine("✗ WebSocket接続失敗");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ 接続エラー: {ex.Message}");
        }
    }

}

public class AvaloniaApp : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}