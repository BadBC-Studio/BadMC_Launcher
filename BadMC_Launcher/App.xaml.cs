using BadMC_Launcher.Helpers;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Services.ViewServices;
using BadMC_Launcher.Views.ContentDialogs.Settings;
using BadMC_Launcher.Views.Pages;
using Hardware.Info;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Serilog;
using Uno.Resizetizer;
using Windows.Foundation;
using Windows.Graphics;

namespace BadMC_Launcher;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    public static new App Current => (App)Application.Current;

    public IThemeService? AppThemeService { get; private set; }

    internal Window? MainWindow { get; private set; }
    internal IHost? Host { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseSerilog(consoleLoggingEnabled: true, fileLoggingEnabled: true, (configuration) => {
                    configuration
                        .MinimumLevel.Error()
                        .WriteTo.Console()
                        .WriteTo.File(
                            path: Path.Combine(AppDataPath.pathsList["LogsPath"], "AppLog.log"),
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 10
                        );
                })
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                )
                .ConfigureServices((context, services) =>
                {
                    //Register third-party class
                    services.AddSingleton<HttpClient>();
                    services.AddSingleton<ResourceLoader>();
                    services.AddSingleton<HardwareInfo>();

                    services.AddTransient<Random>();

                    //Register class
                    services.AddSingleton<ExceptionHandlingService>();
                    services.AddSingleton<PathService>();
                    services.AddSingleton<MinecraftConfigsService>();
                    services.AddSingleton<ThemeConfigsService>();
                    services.AddSingleton<MainSideBarService>();
                    services.AddSingleton<SettingsService>();
                    services.AddSingleton<AppAssetsService>();
                    services.AddSingleton<LaunchSettingsService>();
                    services.AddSingleton<NotificationService>();

                    //Register ContentDialogs
                    services.AddTransient<MinecraftFolderContentDialog>();
                    services.AddTransient<JavaContentDialog>();
                    services.AddTransient<JvmArgumentsContentDialog>();
                    services.AddTransient<BackgroundImageContentDialog>();
                })
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = builder.Build();

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
        }

        // Get ThemeService
        AppThemeService = MainWindow.GetThemeService();

        //Set MainWindow Configs
        MainWindow.AppWindow.Title = GetService<ThemeConfigsService>().WindowName;
        MainWindow.AppWindow.Resize(AppParameters.WindowSize);
        MainWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

#if WINAPPSDK_PACKAGED
        // TODO: 不等了拖拽三大金刚自己写(恼)
        MainWindow.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        MainWindow.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        MainWindow.AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        // Set TitleBar area
        var nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(MainWindow.AppWindow.Id);
        var buttonsNonClientArea = UIHelper.FindElementByName(MainWindow.Content, "AppTitleBarButtons") as FrameworkElement;

        if (buttonsNonClientArea != null) {
            GeneralTransform transformButtons = buttonsNonClientArea.TransformToVisual(null);
            Rect bounds = transformButtons.TransformBounds(new Rect(0, 0, buttonsNonClientArea.ActualWidth, buttonsNonClientArea.ActualHeight));

            var scale = MainWindow.Content.XamlRoot?.RasterizationScale ?? 0.0;
            var transparentRect = new RectInt32() {
                X = (int)Math.Round(bounds.X * scale),
                Y = (int)Math.Round(bounds.Y * scale),
                Width = (int)Math.Round(bounds.Width * scale),
                Height = (int)Math.Round(bounds.Height * scale)
            };
            var rects = new RectInt32[] { transparentRect };

            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rects);
        }
#endif

        // Ensure the current window is active
        MainWindow.Activate();
    }

    //Get Service
    public static T GetService<T>() {
        var services = Current.Host?.Services;
        if (services != null) {
            var service = services.GetService<T>();
            if (service != null) {
                return service;
            }
        }

        throw new InvalidOperationException("Service not found.");
    }

    private static void GetSettings() {
        GetService<MinecraftConfigsService>().SyncSettingGet();
        GetService<MinecraftConfigsService>().IsSyncEnabled = true;
        GetService<ThemeConfigsService>().SyncSettingGet();
        GetService<ThemeConfigsService>().isSyncEnabled = true;
    }
}
