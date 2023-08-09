using System;
using System.Collections.Generic;
using System.Text;
using WorkoutLog.Model;

namespace Fitness_Log.Cells;

/* Determines which data template to return based on cell data */
public class PRDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate Is_Weight_PR { get; set; } /* weight PR goal template */

    /* time PR goal templates */
    public DataTemplate Is_Time_PR_HMS { get; set; }
    public DataTemplate Is_Time_PR_HM { get; set; }
    public DataTemplate Is_Time_PR_HS { get; set; }
    public DataTemplate Is_Time_PR_H { get; set; }
    public DataTemplate Is_Time_PR_MS { get; set; }
    public DataTemplate Is_Time_PR_M { get; set; }
    public DataTemplate Is_Time_PR_S { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var pr_entry = (PR)item;

        if (pr_entry.is_weight_pr) /* if PR goal is a weight goal */
        {
            return Is_Weight_PR;
        }
        else /* else; PR goal is a time goal */
        {
            if (pr_entry.time_hours != 0 && pr_entry.time_min != 0 && pr_entry.time_sec != 0) /* if hr min sec; all have non-zero entries */
            {
                return Is_Time_PR_HMS;
            }
            else if (pr_entry.time_hours != 0 && pr_entry.time_min != 0) /* if hr min; have non-zero entries */
            {
                return Is_Time_PR_HM;
            }
            else if (pr_entry.time_hours != 0 && pr_entry.time_sec != 0) /* if hr sec; have non-zero entries */
            {
                return Is_Time_PR_HS;
            }
            else if (pr_entry.time_hours != 0) /* if hr; has non-zero entries */
            {
                return Is_Time_PR_H;
            }
            else if (pr_entry.time_min != 0 && pr_entry.time_sec != 0) /* if min sec; have non-zero entries */
            {
                return Is_Time_PR_MS;
            }
            else if (pr_entry.time_min != 0) /* if min; has non-zero entries */
            {
                return Is_Time_PR_M;
            }
            else /* if sec; has non-zero entries */
            {
                return Is_Time_PR_S;
            }
        }
    }
}