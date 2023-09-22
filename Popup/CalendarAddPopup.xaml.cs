using Fitness_Log.Model;
using Plugin.MauiMTAdmob;

namespace Fitness_Log.Popup;

public partial class CalendarAddPopup
{
    public CalendarAddPopup()
    {
        InitializeComponent();

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
        {
            CrossMauiMTAdmob.Current.ShowInterstitial();
        };

        Get_Exercise_Categories();        
    }

    /* processes a submission to record an exercise performed */
    private async void Submit_Record(object sender, EventArgs e)
    {
        string category_name = exercise_category.SelectedItem.ToString();
        DateTime date = record_date.Date;

        await App.RecordRepo.Add_Calendar_Entry(category_name, date);

        /* only displays ads 1/3 of the time */
        int adCounter = Preferences.Get("AdCounter", 0);
        adCounter++;
        Preferences.Set("AdCounter", adCounter);

        if (adCounter % 3 == 0)
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

    /* gets exercise categories and lists them as options in the picker */
    public async void Get_Exercise_Categories()
    {
        List<Category> categories = await App.RecordRepo.Get_Calendar_Category_List();
        List<string> category_string_list = new List<string>();

        if (categories.Count == 0) /* if no categories found in database */
        {
            exercise_category_empty_display.IsVisible = true;
            exercise_category.IsVisible = false;
            submit_exercise_button.IsVisible = false;
        }
        else /* else; at least 1 category found in database */
        {
            exercise_category_empty_display.IsVisible = false;
            exercise_category.IsVisible = true;
            submit_exercise_button.IsVisible = true;

            /* loops through the list of categories found on database */
            for (int i = 0; i < categories.Count; i++)
            {
                category_string_list.Add(categories[i].name);
            }

            exercise_category.ItemsSource = category_string_list;
            exercise_category.SelectedIndex = 0;
        }

        record_date.Date = DateTime.Now;
    }
}
