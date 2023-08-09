namespace Fitness_Log;

public partial class AppShell : Shell
{
	public AppShell()
	{
        InitializeComponent();
        Prepare_Header();
        Prepare_Footer();
    }

    /* prepare header */
    private void Prepare_Header()
    {
        header_image.BackgroundColor = Color.FromRgb(153, 217, 234);
    }

    /* prepare footer */
    private void Prepare_Footer()
    {
        /* time stamp */
        DateTime current_date = DateTime.Now;
        string[] date_string = current_date.ToString().Split(' ');
        string date_display = date_string[0];
        footer_date.Text = date_display;
    }
}
