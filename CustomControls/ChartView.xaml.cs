using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Fitness_Log.Model;

namespace Fitness_Log.CustomControls;

public partial class ChartView : Grid
{
    #region BinableProperty
    public ObservableCollection<BodyWeightEntryDot> entries { get; set; } = new ObservableCollection<BodyWeightEntryDot>();

    public ChartView()
	{
		InitializeComponent();
        Fill_Chart();
    }

    /* prepares the body weight graph to be displayed; links chart entries with appropriate data */
    public async void Fill_Chart()
    {
        entries.Clear();

        List<BodyWeightEntry> entry_list = await App.RecordRepo.Get_Body_Weight_List();
        entry_list = entry_list.OrderBy(progress => progress.date).ToList();

        double highest_val_entry = -1;
        double lowest_val_entry = -1;

        /* loop through the body weight entries in database */
        for (int i = 0; i < entry_list.Count; i++)
        {
            if (i == 0) /* if first entry */
            {
                highest_val_entry = entry_list[i].weight;
                lowest_val_entry = entry_list[i].weight;
            }
            else if (entry_list[i].weight > highest_val_entry) /* else if current value is new high value */
            {
                highest_val_entry = entry_list[i].weight;
            }
            else if (entry_list[i].weight < lowest_val_entry) /* else if current value is new low value */
            {
                lowest_val_entry = entry_list[i].weight;
            }
        }

        /* loop through the body weight entries in database */
        for (int i = 0; i < entry_list.Count; i++)
        {
            entries.Add(new BodyWeightEntryDot
            {
                date = entry_list[i].date,
                weight = entry_list[i].weight,
                highest_value = highest_val_entry,
                lowest_value = lowest_val_entry
            });
        }

        Update_Y_Axis(entry_list);
    }

    /* picks accurate y axis label values and displays them */
    public void Update_Y_Axis(List<BodyWeightEntry> entry_list)
    {
        double highest_body_weight_value = -1;
        double lowest_body_weight_value = -1;
        bool has_entry = false;

        for (int i = 0; i < entry_list.Count; i++) /* loops through list of body weight entries */
        {
            if (has_entry == false) /* if first entry from list */
            {
                has_entry = true;
                highest_body_weight_value = entry_list[i].weight;
                lowest_body_weight_value = entry_list[i].weight;
                continue;
            }

            if (entry_list[i].weight > highest_body_weight_value) /* if current entry is higher weight value than the current high value */
            {
                highest_body_weight_value = entry_list[i].weight;
            }
            else if (entry_list[i].weight < lowest_body_weight_value) /* if current entry is lower weight value than the current low value */
            {
                lowest_body_weight_value = entry_list[i].weight;
            }
        }

        if (has_entry) /* if at least 1 body weight entry */
        {
            if (highest_body_weight_value == lowest_body_weight_value) /* if only one entry */
            {
                y_axis_6.Text = entries[0].weight.ToString();
                y_axis_5.Text = "----";
                y_axis_4.Text = "----";
                y_axis_3.Text = "----";
                y_axis_2.Text = "----";
                y_axis_1.Text = "----";
            }
            else /* more than one entry */
            {
                double total_weight_change = highest_body_weight_value - lowest_body_weight_value;
                double total_weight_change_gap = total_weight_change / 5.0;

                double y_value5 = (total_weight_change_gap * 4.0) + lowest_body_weight_value;
                double y_value4 = (total_weight_change_gap * 3.0) + lowest_body_weight_value;
                double y_value3 = (total_weight_change_gap * 2.0) + lowest_body_weight_value;
                double y_value2 = total_weight_change_gap + lowest_body_weight_value;

                /* rounds y-axis values to 1 decimal point */
                y_value5 = Math.Round(y_value5, 2);
                y_value4 = Math.Round(y_value4, 2);
                y_value3 = Math.Round(y_value3, 2);
                y_value2 = Math.Round(y_value2, 2);

                y_axis_6.Text = highest_body_weight_value.ToString();
                y_axis_5.Text = y_value5.ToString();
                y_axis_4.Text = y_value4.ToString();
                y_axis_3.Text = y_value3.ToString();
                y_axis_2.Text = y_value2.ToString();
                y_axis_1.Text = lowest_body_weight_value.ToString();

                double half_gap = total_weight_change_gap / 2.0;

                for (int i = 0; i < entries.Count; i++) /* adjusts y value of entry marker */
                {
                    double min_group6 = highest_body_weight_value - half_gap;
                    double min_group5 = highest_body_weight_value - half_gap - total_weight_change_gap;
                    double min_group4 = highest_body_weight_value - half_gap - (total_weight_change_gap * 2.0);
                    double min_group3 = highest_body_weight_value - half_gap - (total_weight_change_gap * 3.0);
                    double min_group2 = highest_body_weight_value - half_gap - (total_weight_change_gap * 4.0);

                    int initial_adjustments = 0;

                    /* set initial adjustments */
                    if (entries[i].weight >= min_group6) /* if entry in top 1/6 */
                    {
                        initial_adjustments = -3;
                    }
                    else if (entries[i].weight >= min_group3 && entries[i].weight < min_group4)
                    {
                        initial_adjustments = -8;
                    }
                    else if (entries[i].weight < min_group3) /* else if entry is not in bottom 2/6th section */
                    {
                        initial_adjustments = -11;
                    }
                    else /* else; entry is not last or first sections */
                    {
                        initial_adjustments = -5;
                    }

                    int final_adjustments = initial_adjustments;

                    var main_display_info = DeviceDisplay.MainDisplayInfo;
                    double screen_pixels_height = main_display_info.Height;

                    int heading_height = 220;
                    double heading_height_pixels = heading_height * 5.4;

                    double chart_height_pixels = screen_pixels_height - heading_height_pixels;
                    chart_height_pixels = Math.Abs(chart_height_pixels);

                    double chart_adjustment_height_pixels = chart_height_pixels / 10.0;

                    /* slight y value adjustments for entry dot */
                    if (entries[i].weight >= min_group6) /* only positive adjustment */
                    {
                        double diff_value_from_line = highest_body_weight_value - entries[i].weight;
                        double ratio_from_line = diff_value_from_line / half_gap;
                        double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                        int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                        final_adjustments += adjustment_value;
                    }
                    else if (entries[i].weight >= min_group5)
                    {
                        if (entries[i].weight > y_value5) /* negative adjustment; above line */
                        {
                            double diff_value_from_line = entries[i].weight - y_value5;
                            double ratio_from_line = diff_value_from_line / half_gap;
                            double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                            int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                            final_adjustments -= adjustment_value;
                        }
                        else /* positive adjustment; below line */
                        {
                            double diff_value_from_line = y_value5 - entries[i].weight;
                            double ratio_from_line = diff_value_from_line / half_gap;
                            double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                            int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                            final_adjustments += adjustment_value;
                        }
                    }
                    else if (entries[i].weight >= min_group4)
                    {
                        if (entries[i].weight > y_value4) /* negative adjustment; above line */
                        {
                            double diff_value_from_line = entries[i].weight - y_value4;
                            double ratio_from_line = diff_value_from_line / half_gap;
                            double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                            int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                            final_adjustments -= adjustment_value;
                        }
                        else /* positive adjustment; below line */
                        {
                            double diff_value_from_line = y_value4 - entries[i].weight;
                            double ratio_from_line = diff_value_from_line / half_gap;
                            double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                            int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                            final_adjustments += adjustment_value;
                        }
                    }
                    else if (entries[i].weight >= min_group3)
                    {
                        if (entries[i].weight > y_value3) /* negative adjustment; above line */
                        {
                            double diff_value_from_line = entries[i].weight - y_value3;
                            double ratio_from_line = diff_value_from_line / half_gap;
                            double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                            int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                            final_adjustments -= adjustment_value;
                        }
                        else /* positive adjustment; below line */
                        {
                            double diff_value_from_line = y_value3 - entries[i].weight;
                            double ratio_from_line = diff_value_from_line / half_gap;
                            double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                            int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                            final_adjustments += adjustment_value;
                        }
                    }
                    else if (entries[i].weight >= min_group2)
                    {
                        if (entries[i].weight > y_value2) /* negative adjustment; above line */
                        {
                            double diff_value_from_line = entries[i].weight - y_value2;
                            double ratio_from_line = diff_value_from_line / half_gap;
                            double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                            int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                            final_adjustments -= adjustment_value;
                        }
                        else /* positive adjustment; below line */
                        {
                            double diff_value_from_line = y_value2 - entries[i].weight;
                            double ratio_from_line = diff_value_from_line / half_gap;
                            double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                            int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                            final_adjustments += adjustment_value;
                        }
                    }
                    else /* only negative adjustment */
                    {
                        double diff_value_from_line = entries[i].weight - lowest_body_weight_value;
                        double ratio_from_line = diff_value_from_line / half_gap;
                        double chart_adjustment_height = chart_adjustment_height_pixels / 4.0;
                        int adjustment_value = (int)(chart_adjustment_height * ratio_from_line);

                        final_adjustments -= adjustment_value;
                    }

                    entries[i].y_adjustment = final_adjustments;
                }
            }
        }
        else /* else no body weight entries found */
        {
            y_axis_6.Text = "----";
            y_axis_5.Text = "----";
            y_axis_4.Text = "----";
            y_axis_3.Text = "----";
            y_axis_2.Text = "----";
            y_axis_1.Text = "----";
        }
    }

    #region Commands
    /* update calendar currently selected date info */
    public ICommand current_date_command => new Command<BodyWeightEntryDot>((current_date) =>
    {
        selected_entry = current_date.date;
        on_entry_selected?.Invoke(null, current_date.date);
        selected_date_command?.Execute(current_date.date);
    });
    #endregion
}