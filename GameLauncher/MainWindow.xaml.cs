using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Net;
using System.Diagnostics;
using System.Threading;
using Squirrel;


namespace GameLauncher
{



    enum LauncherStatus
    {
        ready,
        failed,
        downloadingGame,
        downloadingUpdate
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        UpdateManager manager;

        private string rootPath;
        private string versionFile;
        private string gameZip;
        private string gameExe;
        private string versionFile2;
        private string gameZip2;
        private string gameExe2;

        private LauncherStatus _status;
        internal LauncherStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (_status)
                {
                    case LauncherStatus.ready:
                        PlayButton.Content = "Graj";
                        PlayButton2.Content = "Graj";
                        break;
                    case LauncherStatus.failed:
                        PlayButton.Content = "Błąd aktualizacji - spróbuj ponownie";
                        PlayButton2.Content = "Błąd aktualizacji - spróbuj ponownie";
                        break;
                    case LauncherStatus.downloadingGame:
                        PlayButton.Content = "Pobieranie gry";
                        PlayButton2.Content = "Pobieranie gry";
                        break;
                    case LauncherStatus.downloadingUpdate:
                        PlayButton.Content = "Pobieranie aktualizacji";
                        PlayButton2.Content = "Pobieranie aktualizacji";
                        break;

                    default:
                        break;
                }
            }
        }

        public MainWindow()
        {

            InitializeComponent();


            rootPath = Directory.GetCurrentDirectory();
            versionFile = Path.Combine(rootPath, "Version_cs.txt");
            gameZip = Path.Combine(rootPath, "CS.zip");
            gameExe = Path.Combine(rootPath, "CS", "Counter Strike.exe");
            versionFile2 = Path.Combine(rootPath, "Version_efyh.txt");
            gameZip2 = Path.Combine(rootPath, "EFYH.zip");
            gameExe2 = Path.Combine(rootPath, "EFYH", "Escape From Your House.exe");
        }
        private void CheckForUpdates()
        {
            if (File.Exists(versionFile))
            {
                Version localVersion = new Version(File.ReadAllText(versionFile));
                VersionText.Text = localVersion.ToString();
                VersionText.Visibility = Visibility.Collapsed;

                try
                {

                    WebClient webClient = new WebClient();
                    Version onlineVersion = new Version(webClient.DownloadString("https://onedrive.live.com/download?cid=B83642AB61298643&resid=B83642AB61298643%21120&authkey=ABnLW5vzD2Ai-yY"));

                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        InstallGameFiles(false, onlineVersion);
                    }
                    else
                    {
                        wersjatext.Visibility = Visibility.Visible;
                        VersionText.Visibility = Visibility.Visible;
                        Status = LauncherStatus.ready;
                        PlayButton.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    Status = LauncherStatus.failed;
                    MessageBox.Show($"Error checking for game updates: {ex}");
                }
            }
            else
            {
                InstallGameFiles(false, Version.zero);
            }
        }


        private void CheckForUpdates2()
        {
            if (File.Exists(versionFile2))
            {
                Version localVersion = new Version(File.ReadAllText(versionFile2));
                VersionText2.Text = localVersion.ToString();
                VersionText2.Visibility = Visibility.Collapsed;
                try
                {

                    WebClient webClient = new WebClient();
                    Version onlineVersion = new Version(webClient.DownloadString("https://onedrive.live.com/download?cid=B83642AB61298643&resid=B83642AB61298643%21123&authkey=AN-zjbL4z27nn6M"));

                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        InstallGameFiles2(false, onlineVersion);
                    }
                    else
                    {
                        wersjatext2.Visibility = Visibility.Visible;
                        VersionText2.Visibility = Visibility.Visible;
                        Status = LauncherStatus.ready;
                        PlayButton2.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    Status = LauncherStatus.failed;
                    MessageBox.Show($"Error checking for game updates: {ex}");
                }
            }
            else
            {
                InstallGameFiles2(false, Version.zero);
            }
        }


        private void InstallGameFiles(bool _isUpdate, Version _onlineVersion)
        {
            try
            {
                WebClient webClient = new WebClient();
                if (_isUpdate)
                {
                    Status = LauncherStatus.downloadingUpdate;
                }
                else
                {
                    Status = LauncherStatus.downloadingGame;
                    _onlineVersion = new Version(webClient.DownloadString("https://onedrive.live.com/download?cid=B83642AB61298643&resid=B83642AB61298643%21120&authkey=ABnLW5vzD2Ai-yY"));
                }
                PlayButton.Visibility = Visibility.Collapsed;
                progressBar1.Visibility = Visibility.Visible;
                inst.Visibility = Visibility.Visible;
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
                webClient.DownloadFileAsync(new Uri("https://onedrive.live.com/download?cid=B83642AB61298643&resid=B83642AB61298643%21121&authkey=ABmvv9VgUevNDiU"), gameZip, _onlineVersion);
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error installing game files: {ex}");
            }
        }

        private void InstallGameFiles2(bool _isUpdate, Version _onlineVersion)
        {
            try
            {
                WebClient webClient = new WebClient();
                if (_isUpdate)
                {
                    Status = LauncherStatus.downloadingUpdate;
                }
                else
                {
                    Status = LauncherStatus.downloadingGame;
                    _onlineVersion = new Version(webClient.DownloadString("https://onedrive.live.com/download?cid=B83642AB61298643&resid=B83642AB61298643%21123&authkey=AN-zjbL4z27nn6M"));
                }
                PlayButton2.Visibility = Visibility.Collapsed;
                progressBar2.Visibility = Visibility.Visible;
                inst2.Visibility = Visibility.Visible;
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged2);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback2);
                webClient.DownloadFileAsync(new Uri("https://onedrive.live.com/download?cid=B83642AB61298643&resid=B83642AB61298643%21124&authkey=ALRxdJe6Iyl2v3k"), gameZip2, _onlineVersion);
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error installing game files: {ex}");
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Maximum = (int)e.TotalBytesToReceive / 100;
            progressBar1.Value = (int)e.BytesReceived / 100;
        }

        void client_DownloadProgressChanged2(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Maximum = (int)e.TotalBytesToReceive / 100;
            progressBar2.Value = (int)e.BytesReceived / 100;
        }


        private void DownloadGameCompletedCallback2(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                progressBar2.Visibility = Visibility.Collapsed;
                inst2.Visibility = Visibility.Collapsed;
                PlayButton2.Visibility = Visibility.Visible;
                string onlineVersion = ((Version)e.UserState).ToString();
                ZipFile.ExtractToDirectory(gameZip2, rootPath, true);
                File.Delete(gameZip2);

                File.WriteAllText(versionFile2, onlineVersion);

                VersionText2.Text = onlineVersion;
                VersionText2.Visibility = Visibility.Visible;
                wersjatext2.Visibility = Visibility.Visible;
                Status = LauncherStatus.ready;
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
            }
        }


        private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                progressBar1.Visibility = Visibility.Collapsed;
                inst.Visibility = Visibility.Collapsed;
                PlayButton.Visibility = Visibility.Visible;
                string onlineVersion = ((Version)e.UserState).ToString();
                ZipFile.ExtractToDirectory(gameZip, rootPath, true);
                File.Delete(gameZip);

                File.WriteAllText(versionFile, onlineVersion);

                VersionText.Text = onlineVersion;
                VersionText.Visibility = Visibility.Visible;
                wersjatext.Visibility = Visibility.Visible;
                Status = LauncherStatus.ready;
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

        }

        private void PlayButton_Click2(object sender, RoutedEventArgs e)
        {

            if (File.Exists(gameExe2) && Status == LauncherStatus.ready)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe2);
                startInfo.WorkingDirectory = Path.Combine(rootPath, "EFYH");
                Process.Start(startInfo);

                Close();
            }
            else if (Status == LauncherStatus.failed)
            {
                CheckForUpdates2();
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {

            if (File.Exists(gameExe) && Status == LauncherStatus.ready)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                startInfo.WorkingDirectory = Path.Combine(rootPath, "CS");
                Process.Start(startInfo);

                Close();
            }
            else if (Status == LauncherStatus.failed)
            {
                CheckForUpdates();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gra2.Visibility = Visibility.Collapsed;
            cs.Visibility = Visibility.Visible;

            ustawieniawin.Visibility = Visibility.Collapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            gra2.Visibility = Visibility.Visible;
            cs.Visibility = Visibility.Collapsed;
            ustawieniawin.Visibility = Visibility.Collapsed;
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            CheckForUpdates();
            update.Visibility = Visibility.Collapsed;
        }

        private void ustawienia(object sender, RoutedEventArgs e)
        {
            gra2.Visibility = Visibility.Collapsed;
            cs.Visibility = Visibility.Collapsed;
            ustawieniawin.Visibility = Visibility.Visible;
        }

        private void update_Click2(object sender, RoutedEventArgs e)
        {
            CheckForUpdates2();
            update2.Visibility = Visibility.Collapsed;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            manager = await UpdateManager
            .GitHubUpdateManager(@"https://github.com/krzychuL1/KrzysiulsGamesLauncher");
            launcherwersja.Text = manager.CurrentlyInstalledVersion().ToString();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var updateInfo = await manager.CheckForUpdate();

            if (updateInfo.ReleasesToApply.Count > 0)
            {
                updatebutton.IsEnabled = true;
            }    
            else
            {
                updatebutton.IsEnabled = false;
            }
        }

        private async void updatebutton_Click(object sender, RoutedEventArgs e)
        {
            await manager.UpdateApp();
            MessageBox.Show("Aplikacja pomyślnie zaktualizowana!");
        }
    }

    struct Version
    {
        internal static Version zero = new Version(0, 0, 0);

        private short major;
        private short minor;

        internal Version(short _major, short _minor, short _subMinor)
        {
            major = _major;
            minor = _minor;
        }
        internal Version(string _version)
        {
            string[] versionStrings = _version.Split('.');
            if (versionStrings.Length != 2)
            {
                major = 0;
                minor = 0;
                return;
            }

            major = short.Parse(versionStrings[0]);
            minor = short.Parse(versionStrings[1]);
        }

        internal bool IsDifferentThan(Version _otherVersion)
        {
            if (major != _otherVersion.major)
            {
                return true;
            }
            else
            {
                if (minor != _otherVersion.minor)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"{major}.{minor}";
        }
    }
}
