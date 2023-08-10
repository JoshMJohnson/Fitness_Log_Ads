using System;
using System.Collections.Generic;
using System.Text;
using Fitness_Log.Model;

namespace Fitness_Log.Cells;

public class PRGoalDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate Is_Weight_PR_Goal { get; set; } /* weight PR goal template */

    /* time PR goal templates */
    public DataTemplate Is_Time_PR_Goal_HMS { get; set; }
    public DataTemplate Is_Time_PR_Goal_HM { get; set; }
    public DataTemplate Is_Time_PR_Goal_HS { get; set; }
    public DataTemplate Is_Time_PR_Goal_H { get; set; }
    public DataTemplate Is_Time_PR_Goal_MS { get; set; }
    public DataTemplate Is_Time_PR_Goal_M { get; set; }
    public DataTemplate Is_Time_PR_Goal_S { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var pr_goal_entry = (GoalPR)item;

        if (pr_goal_entry.is_weight_goal) /* if PR goal is a weight goal */
        {
            return Is_Weight_PR_Goal;
        }
        else /* else; PR goal is a time goal */
        {
            if (pr_goal_entry.time_hours != 0 && pr_goal_entry.time_min != 0 && pr_goal_entry.time_sec != 0) /* if hr min sec; all have non-zero entries */
            {
                return Is_Time_PR_Goal_HMS;
            }
            else if (pr_goal_entry.time_hours != 0 && pr_goal_entry.time_min != 0) /* if hr min; have non-zero entries */
            {
                return Is_Time_PR_Goal_HM;
            }
            else if (pr_goal_entry.time_hours != 0 && pr_goal_entry.time_sec != 0) /* if hr sec; have non-zero entries */
            {
                return Is_Time_PR_Goal_HS;
            }
            else if (pr_goal_entry.time_hours != 0) /* if hr; has non-zero entries */
            {
                return Is_Time_PR_Goal_H;
            }
            else if (pr_goal_entry.time_min != 0 && pr_goal_entry.time_sec != 0) /* if min sec; have non-zero entries */
            {
                return Is_Time_PR_Goal_MS;
            }
            else if (pr_goal_entry.time_min != 0) /* if min; has non-zero entries */
            {
                return Is_Time_PR_Goal_M;
            }
            else /* if sec; has non-zero entries */
            {
                return Is_Time_PR_Goal_S;
            }
        }
    }
}