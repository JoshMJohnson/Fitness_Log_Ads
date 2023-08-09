namespace Fitness_Log;

public partial class App : Application
{
    public static RecordRepository RecordRepo { get; private set; }

    public App()
	{
		InitializeComponent();

		MainPage = new AppShell();

        RecordRepo = repo;
    }
}
