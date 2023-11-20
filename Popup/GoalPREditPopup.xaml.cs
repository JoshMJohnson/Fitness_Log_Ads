using Fitness_Log.Model;
using Plugin.MauiMTAdmob;

namespace Fitness_Log.Popup;

public partial class GoalPREditPopup
{
    private GoalPR editing_pr_goal { get; set; }

    public GoalPREditPopup(string goal_name)
    {
        InitializeComponent();

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
        {
            CrossMauiMTAdmob.Current.ShowInterstitial();
        };

        Gather_Data(goal_name);

        achieve_by_date.MinimumDate = DateTime.Now;
    }

    /* gathers current info about the body weight goal from the database */
    private async void Gather_Data(string goal_name)
    {
        editing_pr_goal = await App.RecordRepo.Get_PR_Goal(goal_name);

        exercise_name_entry.Text = editing_pr_goal.name;

        if (editing_pr_goal.is_weight_goal) /* if pr goal is a weight pr */
        {
            time_display.IsVisible = false;
            weight_display.IsVisible = true;

            weight_pr.Placeholder = editing_pr_goal.weight.ToString();
        }
        else /* else pr goal is a time goal */
        {
            time_display.IsVisible = true;
            weight_display.IsVisible = false;

            hr_pr.Placeholder = editing_pr_goal.time_hours.ToString();
            min_pr.Placeholder = editing_pr_goal.time_min.ToString();
            sec_pr.Placeholder = editing_pr_goal.time_sec.ToString();
        }

        if (editing_pr_goal.date_desired) /* if pr goal has a date achieve date */
        {
            date_included_toggle.IsToggled = true;

            string[] date_time_temp = editing_pr_goal.goal_achieve_by_date.ToString().Split('/');
            achieve_by_date.Date = new DateTime(int.Parse(date_time_temp[2]), int.Parse(date_time_temp[0]), int.Parse(date_time_temp[1]));
        }
        else /* else; goal has no desired date previously */
        {
            date_included_toggle.IsToggled = false;
        }
    }

    /* save the updated body weight goal */
    private async void Save_PR_Goal(object sender, EventArgs e)
    {
        string name = editing_pr_goal.name;

        DateTime date;
        bool has_desired;

        if (date_included_toggle.IsToggled) /* if pr goal date included */
        {
            date = achieve_by_date.Date;
            has_desired = true;
        }
        else /* else pr goal date not included */
        {
            date = DateTime.Now;
            has_desired = false;
        }

        if (editing_pr_goal.is_weight_goal == false) /* if time pr */
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

                await App.RecordRepo.Edit_Goal_PR(name, date, has_desired, false, -1, hr_update, min_update, sec_update);

                /* only displays ads 1/3 of the time */
                int adCounter = Preferences.Get("AdCounter", 0);
                adCounter++;
                Preferences.Set("AdCounter", adCounter);

                if (adCounter % 6 == 0)
                {
                    Show_Intestitial();
                }

                error_prompt.IsVisible = false;
                Close();
            }
            else /* else; time fields are empty */
            {
                error_prompt.Text = "At least one time field must have a value";
                error_prompt.IsVisible = true;
            }
        }
        else /* else weight pr */
        {
            string weight_update_string = weight_pr.Text;

            if (weight_update_string != null && weight_update_string.Length != 0)
            {
                weight_update_string = weight_update_string.ToString();

                double weight_update = double.Parse(weight_update_string);
                await App.RecordRepo.Edit_Goal_PR(name, date, has_desired, true, weight_update, -1, -1, -1);

                /* only displays ads 1/3 of the time */
                int adCounter = Preferences.Get("AdCounter", 0);
                adCounter++;
                Preferences.Set("AdCounter", adCounter);

                if (adCounter % 6 == 0)
                {
                    Show_Intestitial();
                }

                error_prompt.IsVisible = false;
                Close();
            }
            else
            {
                error_prompt.Text = "Weight value cannot be empty";
                error_prompt.IsVisible = true;
            }
        }
    }

    /* shows intestitial video ad */
    private void Show_Intestitial()
    {
        CrossMauiMTAdmob.Current.LoadInterstitial("ca-app-pub-6232744288972049/4461146967");
    }

    /* close the popup window */
    private void Cancel_PR_Goal(object sender, EventArgs e)
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

    /* handles event when toggled between goal date and no date */
    private void date_included_change(object sender, EventArgs e)
    {
        if (date_included_toggle.IsToggled) /* if date desired */
        {
            record_date_display.IsVisible = true;
            no_date_display.IsVisible = false;
        }
        else /* else no date desired */
        {
            record_date_display.IsVisible = false;
            no_date_display.IsVisible = true;
        }
    }
}