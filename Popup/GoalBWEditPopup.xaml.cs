using System;
using Fitness_Log.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fitness_Log.Popup;

public partial class GoalBWEditPopup
{
    private GoalBW editing_body_weight_goal { get; set; }

    public GoalBWEditPopup(string goal_name)
    {
        InitializeComponent();
        Gather_Data(goal_name);

        weight_date.MinimumDate = DateTime.Now;
    }

    /* gathers current info about the body weight goal from the database */
    private async void Gather_Data(string goal_name)
    {
        editing_body_weight_goal = await App.RecordRepo.Get_Body_Weight_Goal(goal_name);

        goal_name_entry.Text = editing_body_weight_goal.name;
        weight_entry.Placeholder = editing_body_weight_goal.weight.ToString();

        if (editing_body_weight_goal.date_desired) /* if goal has a desired date previously */
        {
            date_included_toggle.IsToggled = true;

            string[] date_time_temp = editing_body_weight_goal.goal_achieve_by_date.ToString().Split('/');
            weight_date.Date = new DateTime(int.Parse(date_time_temp[2]), int.Parse(date_time_temp[0]), int.Parse(date_time_temp[1]));
        }
        else /* else; goal has no desired date previously */
        {
            date_included_toggle.IsToggled = false;
        }
    }

    /* save the updated body weight goal */
    private async void Save_Body_Weight_Goal(object sender, EventArgs e)
    {
        string goal_name = editing_body_weight_goal.name;

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

            await App.RecordRepo.Edit_Body_Weight_Goal(goal_name, date, has_desired, weight_update);

            error_prompt.IsVisible = false;
            Close();
        }
        else /* else; body weight value is empty */
        {
            error_prompt.Text = "Goal weight value cannot be empty";
            error_prompt.IsVisible = true;
        }
    }

    /* close the popup window */
    private void Cancel_Body_Weight_Goal(object sender, EventArgs e)
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