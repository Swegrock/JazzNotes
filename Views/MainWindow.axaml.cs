using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JazzNotes.Helpers;
using JazzNotes.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace JazzNotes.Views
{
    public class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnMainWindowOpened(object sender, EventArgs e)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                this.WindowState = WindowState.FullScreen;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
            WindowHelper.MaxWidth = this.Width;
            this.viewModel = this.DataContext as MainWindowViewModel;
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            FileHelper.SaveLinker(this.viewModel.Linker);
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            this.viewModel.PdfHelper.Clean();
        }
    }
}