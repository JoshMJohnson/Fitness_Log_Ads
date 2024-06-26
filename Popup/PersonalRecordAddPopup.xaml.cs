using Fitness_Log.Model;
using Plugin.MauiMTAdmob;

namespace Fitness_Log.Popup;

public partial class PersonalRecordAddPopup
{
    public PersonalRecordAddPopup()
    {
        InitializeComponent();

        time_display.IsVisible = false;
        record_date.MaximumDate = DateTime.Now;

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
        {
            CrossMauiMTAdmob.Current.ShowInterstitial();
        };
    }

    /* creates an exercise along with the PR */
    private async void Submit_PR(object sender, EventArgs e)
    {
        string name = exercise_name_entry.Text;

        if (name != null && name.Length != 0) /* if name field is not empty */
        {
            /* name appearance tweaking */
            name = name.Trim(); /* removes leading and trailing whitespace */
            name = string.Concat(char.ToUpper(name[0]), name.Substring(1));

            if (name != null && name.Length != 0) /* if name field is not empty */
            {

                DateTime date = record_date.Date;

                if (exercise_type_toggle.IsToggled) /* if time pr */
                {
                    string hr_update_string = hr_pr.Text;
                    string min_update_string = min_pr.Text;
                    string sec_update_string = sec_pr.Text;

                    if ((hr_update_string != null && hr_update_string.Length != 0) || (min_update_string != null && min_update_string.Length != 0)
                            || (sec_update_string != null && sec_update_string.Length != 0)) /* if at least one time field is not empty */
                    {
                        int hr_update = 0;
                        int min_update = 0;
                        int sec_update = 0;

                        if (sec_update_string != null && sec_update_string.Length != 0) /* if sec field is not empty */
                        {
                            sec_update_string = sec_update_string.ToString();
                            sec_update = int.Parse(sec_update_string);

                            if (sec_update >= 60) /* if sec field is more than 60 mins */
                            {
                                int additonal_mins = sec_update / 60;
                                min_update = additonal_mins;

                                sec_update = sec_update % 60;
                            }
                        }

                        if (min_update_string != null && min_update_string.Length != 0) /* if min field is not empty */
                        {
                            min_update_string = min_update_string.ToString();
                            min_update += int.Parse(min_update_string);

                            if (min_update >= 60) /* if min field is more than 60 mins */
                            {
                                int additional_hrs = min_update / 60;
                                hr_update = additional_hrs;

                                min_update = min_update % 60;
                            }
                        }

                        if (hr_update_string != null && hr_update_string.Length != 0) /* if hour field is not empty */
                        {
                            hr_update_string = hr_update_string.ToString();
                            hr_update += int.Parse(hr_update_string);
                        }

                        List<PR> pr_list_before = await App.RecordRepo.Get_PR_List();
                        await App.RecordRepo.Add_PR(name, date, false, -1, hr_update, min_update, sec_update);
                        List<PR> pr_list_after = await App.RecordRepo.Get_PR_List();

                        error_prompt.IsVisible = false;

                        if (pr_list_before.Count == pr_list_after.Count) /* if duplicate entry */
                        {
                            Close(false);
                        }
                        else /* else; valid entry; not a duplicate */
                        {
                            /* only displays ads 1/3 of the time */
                            int adCounter = Preferences.Get("AdCounter", 0);
                            adCounter++;
                            Preferences.Set("AdCounter", adCounter);

                            if (adCounter % 6 == 0)
                            {
                                Show_Intestitial();
                            }

                            Close(true);
                        }
                    }
                    else /* else; time field is empty */
                    {
                        error_prompt.Text = "At least one time field must have a value";
                        error_prompt.IsVisible = true;
                    }
                }
                else /* else weight pr */
                {
                    string weight_update_string = weight_pr.Text;

                    if (weight_update_string != null && weight_update_string.Length != 0) /* if weight field is not empty */
                    {
                        weight_update_string = weight_update_string.ToString();

                        double weight_update = double.Parse(weight_update_string);

                        List<PR> pr_list_before = await App.RecordRepo.Get_PR_List();
                        await App.RecordRepo.Add_PR(name, date, true, weight_update, -1, -1, -1);
                        List<PR> pr_list_after = await App.RecordRepo.Get_PR_List();

                        if (pr_list_before.Count == pr_list_after.Count) /* if duplicate entry */
                        {
                            Close(false);
                        }
                        else /* else; valid entry; not a duplicate */
                        {
                            /* only displays ads 1/3 of the time */
                            int adCounter = Preferences.Get("AdCounter", 0);
                            adCounter++;
                            Preferences.Set("AdCounter", adCounter);

                            if (adCounter % 6 == 0)
                            {
                                Show_Intestitial();
                            }

                            Close(true);
                        }
                    }
                    else /* if weight field is empty */
                    {
                        error_prompt.Text = "Weight value cannot be empty";
                        error_prompt.IsVisible = true;
                    }
                }
            }
            else /* else; name field is empty */
            {
                error_prompt.Text = "PR exercise name cannot be empty";
                error_prompt.IsVisible = true;
            }
        }
        else /* else; name field is empty */
        {
            error_prompt.Text = "PR exercise name cannot be empty";
            error_prompt.IsVisible = true;
        }
    }

    /* shows intestitial video ad */
    private void Show_Intestitial()
    {
        CrossMauiMTAdmob.Current.LoadInterstitial("ca-app-pub-6232744288972049/4461146967");
    }

    /* closes popup for creating an exercise */
    private void Cancel_PR(object sender, EventArgs e)
    {
        Close();
    }

    /* handles event when toggled between weight and time PR type */
    private void exercise_type_change(object sender, EventArgs e)
    {
        /* if changed to toggle on weight PR */
        if (time_display.IsVisible)
        {
            weight_display.IsVisible = true;
            time_display.IsVisible = false;
        }
        else /* else changed to toggle on time PR */
        {
            weight_display.IsVisible = false;
            time_display.IsVisible = true;
        }
    }
}
