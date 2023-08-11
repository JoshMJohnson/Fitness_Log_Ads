using Fitness_Log.Model;
using Plugin.MauiMTAdmob;

namespace Fitness_Log.Popup;

public partial class UpdatePR
{
    private PR updating_pr;

    public UpdatePR(string pr_name)
    {
        InitializeComponent();

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
        {
            CrossMauiMTAdmob.Current.ShowInterstitial();
        };

        Get_PR_Data(pr_name);
    }

    /* gets pr data from database */
    private async void Get_PR_Data(string pr_name)
    {
        updating_pr = await App.RecordRepo.Get_PR(pr_name);

        string[] date_string = updating_pr.date_achieved.Split('/');
        DateTime date = new DateTime(int.Parse(date_string[2]), int.Parse(date_string[0]), int.Parse(date_string[1]));

        update_pr_exercise_name.Text = pr_name;
        record_date.Date = date;


        if (updating_pr.is_weight_pr) /* if updating pr is a weight pr */
        {
            /* shows/hides content */
            time_display.IsVisible = false;
            weight_display.IsVisible = true;

            /* displays data */
            weight_pr.Placeholder = updating_pr.weight.ToString();
        }
        else /* else updating pr is a time pr */
        {
            /* shows/hides content */
            time_display.IsVisible = true;
            weight_display.IsVisible = false;

            /* displays data */
            hr_pr.Placeholder = updating_pr.time_hours.ToString();
            min_pr.Placeholder = updating_pr.time_min.ToString();
            sec_pr.Placeholder = updating_pr.time_sec.ToString();
        }
    }

    /* creates an exercise along with the PR */
    private async void Submit_PR_Update(object sender, EventArgs e)
    {
        string name = updating_pr.exercise_name;
        DateTime date = record_date.Date;

        if (updating_pr.is_weight_pr == false) /* if time pr */
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

                await App.RecordRepo.Update_PR(name, date, -1, hr_update, min_update, sec_update);
                Show_Intestitial();
                error_prompt.IsVisible = false;
                Close();
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

                await App.RecordRepo.Update_PR(name, date, weight_update, -1, -1, -1);
                Show_Intestitial();
                error_prompt.IsVisible = false;
                Close();
            }
            else /* if weight field is empty */
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

    /* closes popup for creating an exercise */
    private void Cancel_PR_Update(object sender, EventArgs e)
    {
        Close();
    }
}