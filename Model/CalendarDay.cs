namespace Fitness_Log.Model;

public class CalendarDay : CalendarDatePropertyChanged
{
    public DateTime date { get; set; }

    private bool _isCurrentDate;
    public bool is_current_date
    {
        get => _isCurrentDate;
        set => SetProperty(ref _isCurrentDate, value);
    }

    private bool has_entry;
    public bool Has_Entry
    {
        get => has_entry;
        set => SetProperty(ref has_entry, value);
    }
}