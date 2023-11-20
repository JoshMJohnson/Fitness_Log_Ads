using Fitness_Log.Model;
using Plugin.MauiMTAdmob;

namespace Fitness_Log.Popup;

public partial class CalendarRemovePopup
{
    private DateTime selected_date { get; set; }

    public CalendarRemovePopup(DateTime date)
    {
        InitializeComponent();

        selected_date = date;

        /* set and display default values */
        string selected_date_string = $"{selected_date.Month}/{selected_date.Day}/{selected_date.Year}";
        record_date.Text = selected_date_string;

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
        {
            CrossMauiMTAdmob.Current.ShowInterstitial();
        };

        Retrieve_Day_Entries();
    }

    /* processes a submission to record an exercise performed */
    private async void Remove_Record(object sender, EventArgs e)
    {
        /* gather info of entry to be removed */
        DateTime removal_date = selected_date;

        string string_removal_category = day_entries_display.SelectedItem.ToString();

        Category removal_category = await App.RecordRepo.Get_Category(string_removal_category);

        /* remove entry */
        await App.RecordRepo.Remove_Calendar_Entry(removal_date, removal_category);

        /* only displays ads 1/3 of the time */
        int adCounter = Preferences.Get("AdCounter", 0);
        adCounter++;
        Preferences.Set("AdCounter", adCounter);

        if (adCounter % 6 == 0)
        {
            Show_Intestitial();
        }

        Close();
    }

    /* shows intestitial video ad */
    private void Show_Intestitial()
    {
        CrossMauiMTAdmob.Current.LoadInterstitial("ca-app-pub-6232744288972049/4461146967");
    }

    /* closes popup for adding an exercise */
    private void Cancel_Record(object sender, EventArgs e)
    {
        Close();
    }

    /* retrieves day entries for selected date */
    private async void Retrieve_Day_Entries()
    {
        List<CalendarEntry> day_entries = await App.RecordRepo.Get_Calendar_Entries_List(selected_date);

        /* if no entries for that day */
        if (day_entries.Count == 0)
        {
            remove_button.IsVisible = false;
            day_entries_display.IsVisible = false;
            day_entries_display_empty.IsVisible = true;
        }
        else /* else; at least one entry for that day */
        {
            remove_button.IsVisible = true;
            day_entries_display.IsVisible = true;
            day_entries_display_empty.IsVisible = false;

            /* updates data */
            List<string> string_day_entries = new List<string>();

            foreach (CalendarEntry day in day_entries)
            {
                string_day_entries.Add(day.calendar_category_name);
            }

            day_entries_display.ItemsSource = string_day_entries;
            day_entries_display.SelectedIndex = 0;
        }
    }
}
