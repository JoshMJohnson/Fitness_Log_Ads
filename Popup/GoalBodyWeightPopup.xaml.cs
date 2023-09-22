using Fitness_Log.Model;
using Plugin.MauiMTAdmob;

namespace Fitness_Log.Popup;

public partial class GoalBodyWeightPopup
{
    public GoalBodyWeightPopup()
    {
        InitializeComponent();
        weight_date.MinimumDate = DateTime.Now;

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
        {
            CrossMauiMTAdmob.Current.ShowInterstitial();
        };
    }

    /* creates an exercise along with the PR */
    private async void Submit_Goal(object sender, EventArgs e)
    {
        string goal_name = goal_name_entry.Text;

        if (goal_name != null && goal_name.Length != 0) /* if goal name field is not empty */
        {
            /* name appearance tweaking */
            goal_name = goal_name.Trim(); /* removes leading and trailing whitespace */
            goal_name = string.Concat(char.ToUpper(goal_name[0]), goal_name.Substring(1));

            if (goal_name != null && goal_name.Length != 0) /* if name field is not empty after trim */
            {

                string weight_update_string = weight_entry.Text;

                if (weight_update_string != null && weight_update_string.Length != 0) /* if body weight value is not empty */
                {
                    weight_update_string = weight_update_string.ToString();

                    double weight_update = double.Parse(weight_update_string);

                    DateTime date;
                    bool has_desired;

                    if (date_included_toggle.IsToggled) /* if pr goal date included */
                    {
                        date = weight_date.Date;
                        has_desired = true;
                    }
                    else /* else pr goal date not included */
                    {
                        date = DateTime.Now;
                        has_desired = false;
                    }

                    List<GoalBW> body_weight_goal_list_before = await App.RecordRepo.Get_Body_Weight_Goal_List();
                    await App.RecordRepo.Add_Goal_Body_Weight(goal_name, date, has_desired, weight_update);
                    List<GoalBW> body_weight_goal_list_after = await App.RecordRepo.Get_Body_Weight_Goal_List();

                    error_prompt.IsVisible = false;

                    if (body_weight_goal_list_before.Count == body_weight_goal_list_after.Count) /* if duplicate entry */
                    {
                        Close(false);
                    }
                    else /* else; valid entry; not a duplicate */
                    {
                        /* only displays ads 1/3 of the time */
                        int adCounter = Preferences.Get("AdCounter", 0);
                        adCounter++;
                        Preferences.Set("AdCounter", adCounter);

                        if (adCounter % 3 == 0)
                        {
                            Show_Intestitial();
                        }

                        Close(true);
                    }
                }
                else /* else; body weight value is empty */
                {
                    error_prompt.Text = "Goal weight value cannot be empty";
                    error_prompt.IsVisible = true;
                }
            }
            else /* else; goal name field is empty */
            {
                error_prompt.Text = "Goal name cannot be empty";
                error_prompt.IsVisible = true;
            }
        }
        else /* else; goal name field is empty */
        {
            error_prompt.Text = "Goal name cannot be empty";
            error_prompt.IsVisible = true;
        }
    }

    /* shows intestitial video ad */
    private void Show_Intestitial()
    {
        CrossMauiMTAdmob.Current.LoadInterstitial("ca-app-pub-6232744288972049/4461146967");
    }

    /* closes popup for creating an exercise */
    private void Cancel_Goal(object sender, EventArgs e)
    {
        Close();
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
