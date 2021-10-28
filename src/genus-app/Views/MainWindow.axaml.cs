using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using genus.app.Graphics;
using genus.lib;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace genus.app.Views
{
    public partial class MainWindow : Window
    {
        private VirtualMachine? vm;

        public MainWindow()
        {
            InitializeComponent();

			// Task.Factory.StartNew(() => StartGL());
            StartGL();
        }

        private void StartGL()
        {
            var windowSettings = new NativeWindowSettings
            {
                Size = new Vector2i(640, 320),
                Title = "genus"
            };

			using var glContext = new EmuWindow(GameWindowSettings.Default, windowSettings);
			glContext.Run();
		}

        private async Task StartVirtualMachine()
        {
			var rom = await File.ReadAllBytesAsync("TETRIS");

			vm = new();
			vm.Initialize(rom);
			vm.Start();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}