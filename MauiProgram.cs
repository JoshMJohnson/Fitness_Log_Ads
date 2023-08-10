using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Plugin.MauiMTAdmob;

namespace Fitness_Log;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiMTAdmob()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        /* database access */
        string database_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Workout_Log_Database.db");
        builder.Services.AddSingleton<RecordRepository>(s => ActivatorUtilities.CreateInstance<RecordRepository>(s, database_path));

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
