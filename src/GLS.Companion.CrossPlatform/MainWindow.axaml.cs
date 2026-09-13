using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SharpHook;
using SharpHook.Data;

namespace GLS.Companion.CrossPlatform;

public partial class MainWindow : Window
{
    private readonly EventLoopGlobalHook _globalHook;

    private bool _windowHiddenByHotkey;
    private bool _isParked;
    private bool _isRestoringSettings;

    private string _activeTool = "Cargo";
    private AppWindowMode _lastNonParkMode = AppWindowMode.Full;

    private readonly string _settingsPath;

    private enum AppWindowMode
    {
        Full,
        Compact,
        Run
    }

    private sealed class AppSettings
    {
        public string ActiveTool { get; set; } = "Cargo";
        public string WindowMode { get; set; } = "Full";
        public bool IsParked { get; set; }
        public bool AlwaysOnTop { get; set; }
    }

    public MainWindow()
    {
        InitializeComponent();

        _settingsPath = GetSettingsPath();

        _isRestoringSettings = true;
        LoadSettings();
        _isRestoringSettings = false;

        _globalHook = new EventLoopGlobalHook();
        _globalHook.KeyPressed += GlobalHook_KeyPressed;

        _ = StartGlobalHookAsync();

        Closed += MainWindow_Closed;
    }

    // ------------------------------------------------------------
    // SETTINGS / PERSISTENCE
    // ------------------------------------------------------------

    private static string GetSettingsPath()
    {
        string basePath = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData);

        if (string.IsNullOrWhiteSpace(basePath))
        {
            basePath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.UserProfile),
                ".config");
        }

        string settingsDirectory =
            Path.Combine(basePath, "GLSCompanion");

        Directory.CreateDirectory(settingsDirectory);

        return Path.Combine(settingsDirectory, "settings.json");
    }

    private void LoadSettings()
    {
        AppSettings settings = new();

        try
        {
            if (File.Exists(_settingsPath))
            {
                string json = File.ReadAllText(_settingsPath);

                settings =
                    JsonSerializer.Deserialize<AppSettings>(json)
                    ?? new AppSettings();
            }
        }
        catch
        {
            settings = new AppSettings();
        }

        _activeTool =
            string.Equals(
                settings.ActiveTool,
                "Salvage",
                StringComparison.OrdinalIgnoreCase)
                ? "Salvage"
                : "Cargo";

        if (Enum.TryParse(
                settings.WindowMode,
                true,
                out AppWindowMode savedMode))
        {
            _lastNonParkMode = savedMode;
        }
        else
        {
            _lastNonParkMode = AppWindowMode.Full;
        }

        Topmost = settings.AlwaysOnTop;
        TopmostCheckBox.IsChecked = settings.AlwaysOnTop;

        ApplySelectedTool();
        ApplyWindowMode(_lastNonParkMode);

        if (settings.IsParked)
        {
            EnterParkMode(saveSettings: false);
        }
    }

    private void SaveSettings()
    {
        if (_isRestoringSettings)
        {
            return;
        }

        try
        {
            AppSettings settings = new()
            {
                ActiveTool = _activeTool,
                WindowMode = _lastNonParkMode.ToString(),
                IsParked = _isParked,
                AlwaysOnTop = Topmost
            };

            string json = JsonSerializer.Serialize(
                settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
            // Settings persistence must never prevent the app from running.
        }
    }

    // ------------------------------------------------------------
    // SHARPHOOK STARTUP
    // ------------------------------------------------------------

    private async Task StartGlobalHookAsync()
    {
        try
        {
            await _globalHook.RunAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(
                $"GLS Companion global hotkey failed: {ex.Message}");
        }
    }

    // ------------------------------------------------------------
    // GLOBAL F9 HOTKEY
    // ------------------------------------------------------------

    private void GlobalHook_KeyPressed(
        object? sender,
        KeyboardHookEventArgs e)
    {
        if (e.Data.KeyCode != KeyCode.VcF9)
        {
            return;
        }

        Dispatcher.UIThread.Post(ToggleWindowVisibility);
    }

    private void ToggleWindowVisibility()
    {
        if (IsVisible && !_windowHiddenByHotkey)
        {
            _windowHiddenByHotkey = true;
            Hide();
            return;
        }

        _windowHiddenByHotkey = false;

        Show();

        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }

        Activate();
    }

    private void MainWindow_Closed(
        object? sender,
        EventArgs e)
    {
        SaveSettings();

        _globalHook.KeyPressed -= GlobalHook_KeyPressed;
        _globalHook.Dispose();
    }

    // ------------------------------------------------------------
    // TOOL SELECTION
    // ------------------------------------------------------------

    private void CargoButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _activeTool = "Cargo";

        ApplySelectedTool();
        SaveSettings();
    }

    private void SalvageButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _activeTool = "Salvage";

        ApplySelectedTool();
        SaveSettings();
    }

    private void ApplySelectedTool()
    {
        bool cargoActive =
            !string.Equals(
                _activeTool,
                "Salvage",
                StringComparison.OrdinalIgnoreCase);

        CargoWebView.IsVisible = cargoActive;
        SalvageWebView.IsVisible = !cargoActive;

        if (cargoActive)
        {
            SetActiveTool(CargoButton, SalvageButton);
        }
        else
        {
            SetActiveTool(SalvageButton, CargoButton);
        }
    }

    private static void SetActiveTool(
        Button activeButton,
        Button inactiveButton)
    {
        if (!activeButton.Classes.Contains("activeTool"))
        {
            activeButton.Classes.Add("activeTool");
        }

        inactiveButton.Classes.Remove("activeTool");
    }

    // ------------------------------------------------------------
    // ALWAYS ON TOP
    // ------------------------------------------------------------

    private void TopmostCheckBox_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Topmost = TopmostCheckBox.IsChecked == true;
        SaveSettings();
    }

    // ------------------------------------------------------------
    // FULL MODE
    // ------------------------------------------------------------

    private void FullButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _lastNonParkMode = AppWindowMode.Full;

        ExitParkMode(saveSettings: false);
        ApplyWindowMode(AppWindowMode.Full);

        SaveSettings();
    }

    // ------------------------------------------------------------
    // COMPACT MODE
    // ------------------------------------------------------------

    private void CompactButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _lastNonParkMode = AppWindowMode.Compact;

        ExitParkMode(saveSettings: false);
        ApplyWindowMode(AppWindowMode.Compact);

        SaveSettings();
    }

    // ------------------------------------------------------------
    // RUN MODE
    // ------------------------------------------------------------

    private void RunButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _lastNonParkMode = AppWindowMode.Run;

        ExitParkMode(saveSettings: false);
        ApplyWindowMode(AppWindowMode.Run);

        SaveSettings();
    }

    private void ApplyWindowMode(AppWindowMode mode)
    {
        WindowState = WindowState.Normal;

        WebViewContainer.IsVisible = true;

        switch (mode)
        {
            case AppWindowMode.Compact:
                FooterContainer.IsVisible = true;
                Width = 800;
                Height = 600;
                break;

            case AppWindowMode.Run:
                FooterContainer.IsVisible = false;
                Width = 550;
                Height = 750;
                break;

            default:
                FooterContainer.IsVisible = true;
                Width = 1100;
                Height = 750;
                break;
        }
    }

    // ------------------------------------------------------------
    // PARK MODE
    // ------------------------------------------------------------

    private void ParkButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        EnterParkMode();
    }

    private void EnterParkMode(bool saveSettings = true)
    {
        if (_isParked)
        {
            return;
        }

        _isParked = true;

        WindowState = WindowState.Normal;

        NormalInterface.IsVisible = false;
        ParkedInterface.IsVisible = true;

        SystemDecorations = WindowDecorations.None;
        CanResize = false;

        Width = 330;
        Height = 38;

        if (saveSettings)
        {
            SaveSettings();
        }
    }

    // ------------------------------------------------------------
    // PARKED BAR DRAGGING
    // ------------------------------------------------------------

    private void ParkedDragArea_PointerPressed(
        object? sender,
        PointerPressedEventArgs e)
    {
        var properties =
            e.GetCurrentPoint(this).Properties;

        if (properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }

    // ------------------------------------------------------------
    // RESTORE FROM PARK
    // ------------------------------------------------------------

    private void RestoreButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        RestorePreviousMode();
    }

    private void RestorePreviousMode()
    {
        ExitParkMode(saveSettings: false);
        ApplyWindowMode(_lastNonParkMode);

        Activate();
        SaveSettings();
    }

    private void ExitParkMode(bool saveSettings = true)
    {
        if (!_isParked)
        {
            return;
        }

        _isParked = false;

        SystemDecorations = WindowDecorations.Full;
        CanResize = true;

        ParkedInterface.IsVisible = false;
        NormalInterface.IsVisible = true;

        if (saveSettings)
        {
            SaveSettings();
        }
    }
}
