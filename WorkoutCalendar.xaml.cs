using CommunityToolkit.Maui.Views;
using System;
using System.Diagnostics;
using Fitness_Log.Model;
using Fitness_Log.CustomControls;
using Fitness_Log.Popup;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fitness_Log;

public partial class WorkoutCalendar : ContentPage
{
    private VerticalStackLayout selected_date_empty_list;
    private VerticalStackLayout vertical_layout_empty_category_list;
    private DateTime selected_date_calendar;

    public WorkoutCalendar()
	{
		InitializeComponent();
        Empty_Category_Display();
        Retrieve_Categories();

        /* initialize selected date to current date */
        selected_date_calendar = DateTime.Now.Date;
        Refresh_Selected_Date(selected_date_calendar);

        Empty_Date_Display();
    }

    /* button clicked to create a workout category */
    private async void Create_Category(object sender, EventArgs e)
    {
        object result = await this.ShowPopupAsync(new CalendarCategoryAddPopup());

        if (result != null) /* if body weight entry was made */
        {
            string result_string = result.ToString();

            if (result_string == "True") /* if valid entry */
            {
                Retrieve_Categories();
            }
            else
            {
                await this.ShowPopupAsync(new DuplicateEntryPopup("Calendar Category"));
            }
        }
    }

    /* button clicked to remove a workout category */
    private async void Remove_Category(object sender, EventArgs e)
    {
        await this.ShowPopupAsync(new CalendarCategoryRemovePopup());
        Retrieve_Categories();
    }

    /* updates category display */
    public async void Retrieve_Categories()
    {
        List<Category> category_list = await App.RecordRepo.Get_Calendar_Category_List();
        workout_category_display.ItemsSource = category_list;

        if (category_list.Count == 0)  /* if pr list is empty - no pr's set */
        {
            vertical_layout_empty_category_list.IsVisible = true;
        }
        else  /* else pr list is not empty */
        {
            vertical_layout_empty_category_list.IsVisible = false;
        }
    }

    /* executes when the plus button is clicked to record an exercise */
    private async void Record_Exercise(object sender, EventArgs e)
    {
        await this.ShowPopupAsync(new CalendarAddPopup());
        Refresh_Selected_Date(selected_date_calendar);
        Display_Entries();
    }

    /* executes when the minus button is clicked to record an exercise */
    private async void Unrecord_Exercise(object sender, EventArgs e)
    {
        await this.ShowPopupAsync(new CalendarRemovePopup(selected_date_calendar));
        Refresh_Selected_Date(selected_date_calendar);
        Display_Entries();
    }

    /* updates indicator on calendar when exercise rocorded/unrecorded */
    private void Display_Entries()
    {
        horizontal_calendar_display.Identify_Date_Needs_Entry_Symbol();
    }


    /* refreshes selected date exercise display after exercise entry recorded */
    public async void Refresh_Selected_Date(DateTime selected_date_parameter)
    {
        List<CalendarEntry> day_exercise_list = await App.RecordRepo.Get_Calendar_Entries_List(selected_date_parameter);

        /* if no exercises recorded for selected date */
        if (day_exercise_list.Count == 0)
        {
            selected_date_empty_list.IsVisible = true;
            calendar_day_exercise_display.IsVisible = false;
        }
        else /* else; at least one entry for selected date */
        {
            selected_date_empty_list.IsVisible = false;
            workout_selected_date_exercise_display.ItemsSource = day_exercise_list;
            calendar_day_exercise_display.IsVisible = true;
        }
    }

    /* executes when a date is selected in the horizontal calendar */
    private async void Horizontal_Calendar_On_Date_Selected(object sender, DateTime date)
    {
        selected_date_calendar = date;
        List<CalendarEntry> day_exercise_list = await App.RecordRepo.Get_Calendar_Entries_List(date);

        /* if no exercises recorded for selected date */
        if (day_exercise_list.Count == 0)
        {
            selected_date_empty_list.IsVisible = true;
            calendar_day_exercise_display.IsVisible = false;
        }
        else /* else; at least one entry for selected date */
        {
            selected_date_empty_list.IsVisible = false;
            calendar_day_exercise_display.IsVisible = true;
            workout_selected_date_exercise_display.ItemsSource = day_exercise_list;
        }
    }

    /* creates message stating that no exercises have been recorded for selected day */
    private void Empty_Date_Display()
    {
        selected_date_empty_list = new VerticalStackLayout();

        selected_date_empty_list.VerticalOptions = LayoutOptions.Center;
        selected_date_empty_list.HorizontalOptions = LayoutOptions.Center;

        selected_date_empty_list.Add(new Label
        {
            Text = "No Exercises Recorded!",
            FontSize = 14,
            FontAttributes = FontAttributes.Italic,
            HorizontalOptions = LayoutOptions.Center,
        });

        Grid goal_layout = calendar_day_exercise_display;
        Grid.SetRow(selected_date_empty_list, 0);
        goal_layout.Add(selected_date_empty_list);
    }

    /* creates message for empty categories */
    private void Empty_Category_Display()
    {
        vertical_layout_empty_category_list = new VerticalStackLayout();

        vertical_layout_empty_category_list.VerticalOptions = LayoutOptions.Center;
        vertical_layout_empty_category_list.HorizontalOptions = LayoutOptions.Center;

        vertical_layout_empty_category_list.Add(new Label
        {
            Text = "No Routines Yet!",
            FontSize = 12,
            FontAttributes = FontAttributes.Italic,
            HorizontalOptions = LayoutOptions.Center
        });

        Grid goal_layout = category_layout;
        Grid.SetRow(vertical_layout_empty_category_list, 0);
        goal_layout.Add(vertical_layout_empty_category_list);
    }
}