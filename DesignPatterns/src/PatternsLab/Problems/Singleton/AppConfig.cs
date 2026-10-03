using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PatternsLab.Problems.Singleton;

// public class AppConfig
// {
//     public static int LoadCount;

//     public string DbConnection { get; set; }
//     public string Theme { get; set; }

//     public AppConfig()
//     {
//         LoadCount++;
//         Console.WriteLine($"[AppConfig] Loading settings from disk... (load #{LoadCount})");
//         Thread.Sleep(300);
//         DbConnection = "Server=localhost;Db=School";
//         Theme = "Light";
//     }
// }

public sealed class AppConfig
{
    public static int LoadCount;
    public string DbConnection {get; set;}
    public string Theme {get; set;}
    private readonly static Lazy<AppConfig> _lazy = new Lazy<AppConfig>(() => new AppConfig());
    public static AppConfig Instance => _lazy.Value;

    private AppConfig()
    {
        LoadCount++;
        Console.WriteLine($"[AppConfig] Loading settings from disk... (load #{LoadCount})");
        Thread.Sleep(300);
        DbConnection = "Server=localhost;Db=School";
        Theme = "Light";
    }
}

public class DatabaseService
{
    public AppConfig Config {get;} = AppConfig.Instance;

    public void Connect() => Console.WriteLine($"Connecting to {Config.DbConnection}");
}

public class UiService
{
    public AppConfig Config {get;} = AppConfig.Instance;

    public void Render() => Console.WriteLine($"UI is using theme: {Config.Theme}");
}
