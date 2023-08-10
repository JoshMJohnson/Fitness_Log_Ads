using System.Collections.ObjectModel;
using System.Windows.Input;
using Fitness_Log.Model;

namespace Fitness_Log.CustomControls;

public partial class CalendarView : StackLayout
{
    #region BinableProperty

    /* updating data after changing month displayed */
    public static readonly BindableProperty displayed_month_property = BindableProperty.Create(
        nameof(_tempDate),
        typeof(DateTime),
        declaringType: typeof(CalendarView),
        defaultBindingMode: BindingMode.TwoWay,
        defaultValue: DateTime.Now,
        propertyChanged: Selected_Date_Property_Changed);

    /* updating data after selecting a new date */
    public static readonly BindableProperty selected_date_property = BindableProperty.Create(
        nameof(selected_date),
        typeof(DateTime),
        declaringType: typeof(CalendarView),
        defaultBindingMode: BindingMode.TwoWay,
        defaultValue: DateTime.Now,
        propertyChanged: Selected_Date_Property_Changed);

    /* executes when a date is selected */
    private static void Selected_Date_Property_Changed(BindableObject bindable, object oldValue, object newValue)
    {
        var controls = (CalendarView)bindable;

        if (newValue != null)
        {
            var new_date = (DateTime)newValue;

            if (controls._tempDate.Month == new_date.Month && controls._tempDate.Year == new_date.Year)
            {
                var current_date = controls.dates.Where(d => d.date == new_date.Date).FirstOrDefault();

                if (current_date != null)
                {
                    controls.dates.ToList().ForEach(d => d.is_current_date = false);
                    current_date.is_current_date = true;
                }
            }
            else
            {
                controls.Bind_Dates(new_date);
            }
        }
    }

    /* displays current month displayed for workout calendar */
    public DateTime displaying_month
    {
        get => (DateTime)GetValue(displayed_month_property);
        set => SetValue(displayed_month_property, value);
    }

    public static readonly BindableProperty displaying_month_command_property = BindableProperty.Create(
        nameof(displaying_month_command),
        typeof(ICommand),
        declaringType: typeof(CalendarView));

    public ICommand displaying_month_command
    {
        get => (ICommand)GetValue(displaying_month_command_property);
        set => SetValue(displaying_month_command_property, value);
    }

    /* contains data for the current date selected */
    public DateTime selected_date
    {
        get => (DateTime)GetValue(selected_date_property);
        set => SetValue(selected_date_property, value);
    }

    public static readonly BindableProperty selected_date_command_property = BindableProperty.Create(
        nameof(selected_date_command),
        typeof(ICommand),
        declaringType: typeof(CalendarView));

    public ICommand selected_date_command
    {
        get => (ICommand)GetValue(selected_date_command_property);
        set => SetValue(selected_date_command_property, value);
    }

    public event EventHandler<DateTime> on_date_selected;
    public DateTime _tempDate
    {
        get => (DateTime)GetValue(displayed_month_property);
        set => SetValue(displayed_month_property, value);
    }
    #endregion

    public ObservableCollection<CalendarDay> dates { get; set; } = new ObservableCollection<CalendarDay>();

    public CalendarView()
    {
        InitializeComponent();
        Bind_Dates(DateTime.Now);
    }

    /* prepares the calendar to be displayed; links calendar nodes with appropriate data */
    private void Bind_Dates(DateTime date)
    {
        dates.Clear();

        int days_count = DateTime.DaysInMonth(date.Year, date.Month); /* get num of days in selected month */

        /* loop through the days in the selected month */
        for (int day = 1; day <= days_count; day++)
        {
            dates.Add(new CalendarDay
            {
                date = new DateTime(date.Year, date.Month, day),
            });
        }

        var selected_date_data = dates.Where(d => d.date.Date == selected_date.Date).FirstOrDefault();

        if (selected_date_data != null)
        {
            selected_date_data.is_current_date = true;
            _tempDate = selected_date_data.date;
        }

        Identify_Date_Needs_Entry_Symbol();
    }

    #region Commands
    /* update calendar currently selected date info */
    public ICommand current_date_command => new Command<CalendarDay>((current_date) =>
    {
        _tempDate = current_date.date;
        selected_date = current_date.date;
        on_date_selected?.Invoke(null, current_date.date);
        selected_date_command?.Execute(current_date.date);
    });

    /* changes temp variables to next months data */
    public ICommand next_month_command => new Command(() =>
    {
        _tempDate = _tempDate.AddMonths(1);
        displaying_month = _tempDate;
        Bind_Dates(_tempDate);
    });

    /* changes temp variables to previous months data */
    public ICommand previous_month_command => new Command(() =>
    {
        _tempDate = _tempDate.AddMonths(-1);
        displaying_month = _tempDate;
        Bind_Dates(_tempDate);
    });
    #endregion

    /* jumps horizontal calendar to display current date on display */
    private void Jump_To_Current_Date(object sender, EventArgs e)
    {
        DateTime present_date = DateTime.Now.Date;

        _tempDate = present_date;
        selected_date = present_date;

        on_date_selected?.Invoke(null, present_date);
        selected_date_command?.Execute(present_date);
        Bind_Dates(present_date);
    }

    /* marks/unmarks each date if it has/hasn't any workout entries */
    public async void Identify_Date_Needs_Entry_Symbol()
    {
        /* loops through all the dates in the month being displayed */
        for (int i = 0; i < dates.Count; i++)
        {
            DateTime temp_date = dates[i].date;

            List<CalendarEntry> entries = await App.RecordRepo.Get_Calendar_Entries_List(temp_date);

            if (entries.Count != 0) /* if at least one entry for current calendar date */
            {
                dates[i].Has_Entry = true;
            }
            else /* else; no entries for current calendar date */
            {
                dates[i].Has_Entry = false;
            }
        }
    }
}