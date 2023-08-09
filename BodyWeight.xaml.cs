using WorkoutLog.Popup;
using WorkoutLog.Model;
using CommunityToolkit.Maui.Views;
using AndroidX.ConstraintLayout.Core;

namespace Fitness_Log;

public partial class BodyWeight : ContentPage
{
	public BodyWeight()
	{
		InitializeComponent();
        Refresh_Table_Data();
    }

    /* executed when body weight plus button clicked to record an update in body weight */
    private async void Record_Body_Weight(object sender, EventArgs e)
    {
        object result = await this.ShowPopupAsync(new BodyWeightAddPopup());

        if (result != null) /* if body weight entry was made */
        {
            string result_string = result.ToString();

            if (result_string == "True") /* if valid entry */
            {
                Refresh_Table_Data();
                chart_display.Fill_Chart();
            }
            else /* if entry already exists for given date */
            {
                await this.ShowPopupAsync(new DuplicateEntryPopup("Body Weight"));
            }
        }
    }

    /* refreshes the Body Weight table data when refresh button clicked */
    private void Refresh_Table(object sender, EventArgs e)
    {
        Refresh_Table_Data();
    }

    /* refreshes the Body Weight table data */
    private async void Refresh_Table_Data()
    {
        List<BodyWeightEntry> entries = await App.RecordRepo.Get_Body_Weight_List();

        /* 'Actual' cell */
        if (entries.Count == 0) /* if empty body weight entries */
        {
            actual_weight_display.Text = "---";
        }
        else  /* else at least 1 body weight entries */
        {
            actual_weight_display.Text = entries[0].weight.ToString();
        }

        /* 'Recent Change' cell */
        if (entries.Count <= 1) /* if less than 2 body weight entries */
        {
            change_weight_display.Text = "---";
        }
        else /* else; at least 2 body weight entries */
        {
            double most_recent_weight_entry = entries[0].weight;
            double second_most_recent_weight_entry = entries[1].weight;
            double weight_change = most_recent_weight_entry - second_most_recent_weight_entry;

            change_weight_display.Text = weight_change.ToString();
        }

        /* 'Closest Goal' cell */
        List<GoalBW> body_weight_goals = await App.RecordRepo.Get_Body_Weight_Goal_List();

        if (body_weight_goals.Count == 0) /* if no body weight goals */
        {
            goal_display.Text = "---";
        }
        else /* else; at least one body weight goal set */
        {
            for (int i = 0; i < body_weight_goals.Count; i++) /* loop throught the body weight goals */
            {
                if (body_weight_goals[i].date_desired) /* if current goal has a desired achieve by date */
                {
                    goal_display.Text = body_weight_goals[i].weight.ToString();
                    break;
                }

                if (i == body_weight_goals.Count - 1) /* if no body weight goals have a desired date */
                {
                    goal_display.Text = "---";
                }
            }
        }

        /* 'Last 7 Days' cell */
        DateTime current_date = DateTime.Now;
        DateTime week_earlier_date = current_date.AddDays(-7);

        bool one_entry_week = false;
        bool two_entries_week = false;

        BodyWeightEntry most_recent_entry_week = new BodyWeightEntry();
        BodyWeightEntry furthest_entry_within_week = new BodyWeightEntry();

        for (int i = 0; i < entries.Count; i++) /* loops through body weight entries */
        {
            DateTime temp_date = entries[i].date;

            if (temp_date >= week_earlier_date && one_entry_week == false) /* if entry in database is within a week from todays date */
            {
                most_recent_entry_week = entries[i];
                one_entry_week = true;
            }
            else if (temp_date >= week_earlier_date) /* if entry in database is within a week from todays date and another entry found within a week */
            {
                furthest_entry_within_week = entries[i];
                two_entries_week = true;
            }
            else /* else; entry in database is older than a week from todays date */
            {
                break;
            }
        }

        if (one_entry_week && two_entries_week) /* if at least 2 entries with 7 days */
        {
            double weight_diff = most_recent_entry_week.weight - furthest_entry_within_week.weight;
            week_change_display.Text = weight_diff.ToString();
        }
        else /* else; only 1 or 2 entries with 7 days */
        {
            week_change_display.Text = "---";
        }

        /* 'Last 30 Days' cell */
        DateTime month_earlier_date = current_date.AddDays(-30);

        bool one_entry_month = false;
        bool two_entries_month = false;

        BodyWeightEntry most_recent_entry_month = new BodyWeightEntry();
        BodyWeightEntry furthest_entry_within_month = new BodyWeightEntry();

        for (int i = 0; i < entries.Count; i++) /* loops through body weight entries */
        {
            DateTime temp_date = entries[i].date;

            if (temp_date >= month_earlier_date && one_entry_month == false) /* if entry in database is within a month from todays date */
            {
                most_recent_entry_month = entries[i];
                one_entry_month = true;
            }
            else if (temp_date >= month_earlier_date) /* if entry in database is within a month from todays date and another entry found within a month */
            {
                furthest_entry_within_month = entries[i];
                two_entries_month = true;
            }
            else /* else; entry in database is older than a month from todays date */
            {
                break;
            }
        }

        if (one_entry_month && two_entries_month) /* if at least 2 entries with 7 days */
        {
            double weight_diff = most_recent_entry_month.weight - furthest_entry_within_month.weight;
            month_change_display.Text = weight_diff.ToString();
        }
        else /* else; only 1 or 2 entries with 7 days */
        {
            month_change_display.Text = "---";
        }

        /* 'Total' cell */
        int num_entries = entries.Count;

        if (num_entries <= 1) /* if; 0 or 1 body weight entries */
        {
            total_change_display.Text = "---";
        }
        else /* else more than 1 recorded body weight entries */
        {
            double oldest_recorded_weight = entries[num_entries - 1].weight;
            double newest_recorded_weight = entries[0].weight;
            double diff_weight = newest_recorded_weight - oldest_recorded_weight;

            total_change_display.Text = diff_weight.ToString();
        }
    }

    /* executes when a body weight entry is selected in the graph */
    private async void Entry_Selected(object sender, DateTime date)
    {
        await this.ShowPopupAsync(new BodyWeightUpdatePopup(date));
        chart_display.Fill_Chart();
        Refresh_Table_Data();
    }
}