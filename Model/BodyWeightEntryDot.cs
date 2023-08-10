namespace Fitness_Log.Model;

public class BodyWeightEntryDot : BodyWeightEntryPropertyChanged
{
    public DateTime date { get; set; }
    public double weight { get; set; }
    public int y_adjustment { get; set; }
    public double highest_value { get; set; }
    public double lowest_value { get; set; }

    private bool _isCurrentDateSelected;
    public bool is_current_date_selected
    {
        get => _isCurrentDateSelected;
        set => SetProperty(ref _isCurrentDateSelected, value);
    }
}