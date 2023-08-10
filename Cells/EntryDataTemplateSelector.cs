using Fitness_Log.Model;

namespace Fitness_Log.Cells;

public class EntryDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate Min_Group_6 { get; set; }
    public DataTemplate Min_Group_5 { get; set; }
    public DataTemplate Min_Group_4 { get; set; }
    public DataTemplate Min_Group_3 { get; set; }
    public DataTemplate Min_Group_2 { get; set; }
    public DataTemplate Group_1 { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var body_weight_entry = (BodyWeightEntryDot) item;

        double total_weight_change = body_weight_entry.highest_value - body_weight_entry.lowest_value;
        double total_weight_change_gap = total_weight_change / 5.0;

        double half_gap = total_weight_change_gap / 2.0;

        double min_group6 = body_weight_entry.highest_value - half_gap;
        double min_group5 = body_weight_entry.highest_value - half_gap - total_weight_change_gap;
        double min_group4 = body_weight_entry.highest_value - half_gap - (total_weight_change_gap * 2.0);
        double min_group3 = body_weight_entry.highest_value - half_gap - (total_weight_change_gap * 3.0);
        double min_group2 = body_weight_entry.highest_value - half_gap - (total_weight_change_gap * 4.0);

        if (body_weight_entry.weight >= min_group6)
        {
            return Min_Group_6;
        }
        else if (body_weight_entry.weight >= min_group5)
        {
            return Min_Group_5;
        }
        else if (body_weight_entry.weight >= min_group4)
        {
            return Min_Group_4;
        }
        else if (body_weight_entry.weight >= min_group3)
        {
            return Min_Group_3;
        }
        else if (body_weight_entry.weight >= min_group2)
        {
            return Min_Group_2;
        }
        else
        {
            return Group_1;
        }
    }
}