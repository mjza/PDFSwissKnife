using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using CommunityToolkit.Maui;
using PDFSwissKnife.Views;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Maui.Storage;

namespace PDFSwissKnife;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register the FolderPicker as a singleton
		builder.Services.AddSingleton<IFolderPicker>(FolderPicker.Default);

		// Register the MainPage as transient to make sure it can resolve the IFolderPicker dependency.
		builder.Services.AddTransient<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
