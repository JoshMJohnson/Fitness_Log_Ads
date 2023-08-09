using SQLite;
using WorkoutLog.Model;

namespace Fitness_Log;

public class RecordRepository
{
    private static SQLiteAsyncConnection conn;
    private string database_path;
    public string status_message { get; set; }

    public RecordRepository(string db_path)
    {
        database_path = db_path;
    }

    /* initializes database */
    private async Task Init_Database()
    {
        if (conn != null) { return; }

        conn = new SQLiteAsyncConnection(database_path); /* create database */

        /* create database tables */
        await conn.CreateTablesAsync<Category, PR, GoalPR, GoalBW, CalendarEntry>();
        await conn.CreateTablesAsync<Note, Progression, BodyWeightEntry>();
    }

    /* * body progression section */
    /* adds an entry to the body progression table within the database */
    public async Task Add_Progression(string image_path, DateTime image_date)
    {
        ArgumentNullException.ThrowIfNull(image_path, nameof(image_path));
        ArgumentNullException.ThrowIfNull(image_date, nameof(image_date));

        try
        {
            await Init_Database();

            /* translate DateTime to string; removes the time display */
            string[] date_time_temp = image_date.ToString().Split(' ');
            string date_only = date_time_temp[0];

            string[] date_broken_up = date_only.Split('/');
            DateTime date_datatype = new DateTime(int.Parse(date_broken_up[2]), int.Parse(date_broken_up[0]), int.Parse(date_broken_up[1]));

            Progression new_goal = new Progression
            {
                image_full_path = image_path,
                date = date_only,
                date_sort = date_datatype
            };

            int result = await conn.InsertAsync(new_goal);

            status_message = string.Format("{0} progression added", result);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to add progression. Error: {0}", e.Message);
        }
    }

    /* removes an entry in the body progression table within the database */
    public async Task Remove_Progression(string image_name)
    {
        ArgumentNullException.ThrowIfNull(image_name, nameof(image_name));

        try
        {
            await Init_Database();
            Progression removing_progression = await conn.FindAsync<Progression>(image_name);
            await conn.DeleteAsync(removing_progression);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to remove progression {0}. Error: {1}", image_name, e.Message);
        }
    }

    /* returns a list of progressions from the database */
    public async Task<List<Progression>> Get_Progression_List()
    {
        try
        {
            await Init_Database();
            List<Progression> body_progression_list = await conn.Table<Progression>().ToListAsync();
            body_progression_list = body_progression_list.OrderBy(progress => progress.date_sort).Reverse().ToList();
            return body_progression_list;
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to retrieve progression list. {0}", e.Message);
        }

        return new List<Progression>();
    }

    /* returns a progression from the database */
    public async Task<Progression> Get_Progression(string image_name)
    {
        await Init_Database();
        Progression progression_temp = await conn.FindAsync<Progression>(image_name);
        return progression_temp;
    }

    /* * body weight section */
    /* adds an entry to the body weight table within the database */
    public async Task Add_Body_Weight(DateTime entry_date, double entry_weight)
    {
        ArgumentNullException.ThrowIfNull(entry_date, nameof(entry_date));
        ArgumentNullException.ThrowIfNull(entry_weight, nameof(entry_weight));

        try
        {
            await Init_Database();

            BodyWeightEntry new_entry = new BodyWeightEntry
            {
                date = entry_date,
                weight = entry_weight
            };

            int result = await conn.InsertAsync(new_entry);

            status_message = string.Format("{0} body weight added", result);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to add body weight entry. Error: {0}", e.Message);
        }
    }

    /* edit an entry in the body weight table within the database */
    public async Task Edit_Body_Weight(DateTime entry_date, double updated_weight)
    {
        ArgumentNullException.ThrowIfNull(entry_date, nameof(entry_date));
        ArgumentNullException.ThrowIfNull(updated_weight, nameof(updated_weight));

        try
        {
            await Init_Database();

            BodyWeightEntry updating_body_weight_entry = await conn.FindAsync<BodyWeightEntry>(entry_date);
            updating_body_weight_entry.weight = updated_weight;

            await conn.UpdateAsync(updating_body_weight_entry);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to edit body weight entry. Error: {0}", e.Message);
        }
    }

    /* removes an entry in the body weight table within the database */
    public async Task Remove_Body_Weight(DateTime entry_date)
    {
        ArgumentNullException.ThrowIfNull(entry_date, nameof(entry_date));

        try
        {
            await Init_Database();
            BodyWeightEntry removing_bw_entry = await conn.FindAsync<BodyWeightEntry>(entry_date);
            await conn.DeleteAsync(removing_bw_entry);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to remove body weight entry on {0}. Error: {1}", entry_date, e.Message);
        }
    }

    /* returns a list of body weight entries from the database */
    public async Task<List<BodyWeightEntry>> Get_Body_Weight_List()
    {
        try
        {
            await Init_Database();
            List<BodyWeightEntry> body_weight_list = await conn.Table<BodyWeightEntry>().ToListAsync();
            body_weight_list = body_weight_list.OrderBy(entry => entry.date).Reverse().ToList();
            return body_weight_list;
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to retrieve body weight entry list. {0}", e.Message);
        }

        return new List<BodyWeightEntry>();
    }

    /* returns an entry for body weight from the database */
    public async Task<BodyWeightEntry> Get_Body_Weight(DateTime entry_date)
    {
        BodyWeightEntry temp_body_weight_entry = await conn.FindAsync<BodyWeightEntry>(entry_date);
        return temp_body_weight_entry;
    }

    /* * body weight goals section */
    /* adds a body weight entry to the goal table within the database */
    public async Task Add_Goal_Body_Weight(string goal_name, DateTime date, bool has_desired, double goal_weight)
    {
        ArgumentNullException.ThrowIfNull(goal_name, nameof(goal_name));
        ArgumentNullException.ThrowIfNull(date, nameof(date));
        ArgumentNullException.ThrowIfNull(has_desired, nameof(has_desired));
        ArgumentNullException.ThrowIfNull(goal_weight, nameof(goal_weight));

        try
        {
            await Init_Database();
            string date_only;

            if (has_desired) /* if a date set for pr goal */
            {
                /* translate DateTime to string; removes the time display */
                string[] date_time_temp = date.ToString().Split(' ');
                date_only = date_time_temp[0];
            }
            else /* else no date of goal set */
            {
                date_only = "N/A";
            }

            GoalBW new_goal = new GoalBW
            {
                name = goal_name,
                goal_achieve_by_date = date_only,
                date_desired = has_desired,
                weight = goal_weight,
            };

            int result = await conn.InsertAsync(new_goal);

            status_message = string.Format("{0} body weight goal added (Goal date: {1})", result, date);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to add body weight goal date: {0}. Error: {1}", date, e.Message);
        }
    }

    /* removes a body weight entry in the goal table within the database */
    public async Task Remove_Goal_Body_Weight(string goal_name)
    {
        ArgumentNullException.ThrowIfNull(goal_name, nameof(goal_name));

        try
        {
            await Init_Database();
            GoalBW removing_bw_goal = await conn.FindAsync<GoalBW>(goal_name);
            await conn.DeleteAsync(removing_bw_goal);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to remove body weight goal {0}. Error: {1}", goal_name, e.Message);
        }
    }

    /* edit a body weight goal */
    public async Task Edit_Body_Weight_Goal(string goal_name, DateTime goal_date, bool has_desired, double goal_weight)
    {
        ArgumentNullException.ThrowIfNull(goal_name, nameof(goal_name));
        ArgumentNullException.ThrowIfNull(goal_date, nameof(goal_date));
        ArgumentNullException.ThrowIfNull(has_desired, nameof(has_desired));
        ArgumentNullException.ThrowIfNull(goal_weight, nameof(goal_weight));

        try
        {
            await Init_Database();
            GoalBW editing_goal = await Get_Body_Weight_Goal(goal_name);

            string date_only;

            if (has_desired) /* if a date set for pr goal */
            {
                /* translate DateTime to string; removes the time display */
                string[] date_time_temp = goal_date.ToString().Split(' ');
                date_only = date_time_temp[0];
            }
            else /* else no date of goal set */
            {
                date_only = "N/A";
            }

            editing_goal.goal_achieve_by_date = date_only;
            editing_goal.date_desired = has_desired;
            editing_goal.weight = goal_weight;

            await conn.UpdateAsync(editing_goal);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to edit body weight goal name: {0}. Error: {1}", goal_name, e.Message);
        }
    }

    /* returns a list of body weight goals from the database */
    public async Task<List<GoalBW>> Get_Body_Weight_Goal_List()
    {
        try
        {
            await Init_Database();

            List<GoalBW> body_weight_goal_list = await conn.Table<GoalBW>().ToListAsync();
            body_weight_goal_list = body_weight_goal_list.OrderBy(goal => goal.date_desired).ToList();

            List<GoalBW> sorted_body_weight_goal_list = new List<GoalBW>();
            List<GoalBW> final_list = new List<GoalBW>();


            /* loops through list of body weight goal entries found in database */
            for (int i = 0; i < body_weight_goal_list.Count; i++)
            {
                string[] date_array = body_weight_goal_list[i].goal_achieve_by_date.Split('/');

                if (body_weight_goal_list[i].date_desired) /* if goal has no date desired set */
                {
                    body_weight_goal_list[i].date_sort = new DateTime(int.Parse(date_array[2]), int.Parse(date_array[0]), int.Parse(date_array[1]));
                    sorted_body_weight_goal_list.Add(body_weight_goal_list[i]);
                }
                else /* else; no goal date set */
                {
                    final_list.Add(body_weight_goal_list[i]);
                }
            }

            sorted_body_weight_goal_list = sorted_body_weight_goal_list.OrderBy(goal => goal.date_sort).ToList();

            /* adds all date desired body weight goals to final list */
            for (int i = 0; i < sorted_body_weight_goal_list.Count; i++)
            {
                final_list.Add(sorted_body_weight_goal_list[i]);
            }

            return final_list;
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to retrieve body weight goal list. {0}", e.Message);
        }

        return new List<GoalBW>();
    }

    /* returns a body weight goal with primary key matching parameter */
    public async Task<GoalBW> Get_Body_Weight_Goal(string goal_name)
    {
        GoalBW temp_goal = await conn.FindAsync<GoalBW>(goal_name);
        return temp_goal;
    }

    /* * pr goals section */
    /* adds a pr entry to the goal table within the database */
    public async Task Add_Goal_PR(string goal_name, DateTime date, bool has_desired, bool is_weight,
                                    double goal_weight, int hours, int mins, int sec)
    {
        ArgumentNullException.ThrowIfNull(goal_name, nameof(goal_name));
        ArgumentNullException.ThrowIfNull(date, nameof(date));
        ArgumentNullException.ThrowIfNull(has_desired, nameof(has_desired));
        ArgumentNullException.ThrowIfNull(is_weight, nameof(is_weight));
        ArgumentNullException.ThrowIfNull(goal_weight, nameof(goal_weight));
        ArgumentNullException.ThrowIfNull(hours, nameof(hours));
        ArgumentNullException.ThrowIfNull(mins, nameof(mins));
        ArgumentNullException.ThrowIfNull(sec, nameof(sec));

        if (is_weight) /* if weight pr type */
        {
            hours = -1;
            mins = -1;
            sec = -1;
        }
        else /* if time pr type */
        {
            goal_weight = -1;
        }

        try
        {
            await Init_Database();
            string date_only;

            if (has_desired) /* if a date set for pr goal */
            {
                /* translate DateTime to string; removes the time display */
                string[] date_time_temp = date.ToString().Split(' ');
                date_only = date_time_temp[0];
            }
            else /* else no date of goal set */
            {
                date_only = "N/A";
            }

            GoalPR new_goal = new GoalPR
            {
                name = goal_name,
                goal_achieve_by_date = date_only,
                date_desired = has_desired,
                is_weight_goal = is_weight,
                weight = goal_weight,
                time_hours = hours,
                time_min = mins,
                time_sec = sec
            };

            int result = await conn.InsertAsync(new_goal);

            status_message = string.Format("{0} PR goal added (Goal date: {1})", result, date);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to add PR goal date: {0}. Error: {1}", date, e.Message);
        }
    }

    /* removes a pr goal entry in the GoalPR table within the database */
    public async Task Remove_Goal_PR(string goal_name)
    {
        ArgumentNullException.ThrowIfNull(goal_name, nameof(goal_name));

        try
        {
            await Init_Database();
            GoalPR removing_pr_goal = await conn.FindAsync<GoalPR>(goal_name);
            await conn.DeleteAsync(removing_pr_goal);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to remove pr goal {0}. Error: {1}", goal_name, e.Message);
        }
    }

    /* edit a pr goal */
    public async Task Edit_Goal_PR(string goal_name, DateTime goal_date, bool has_desired_date, bool is_weight,
                                        double goal_weight, int goal_hours, int goal_mins, int goal_secs)
    {
        ArgumentNullException.ThrowIfNull(goal_name, nameof(goal_name));
        ArgumentNullException.ThrowIfNull(goal_date, nameof(goal_date));
        ArgumentNullException.ThrowIfNull(has_desired_date, nameof(has_desired_date));
        ArgumentNullException.ThrowIfNull(is_weight, nameof(is_weight));
        ArgumentNullException.ThrowIfNull(goal_weight, nameof(goal_weight));
        ArgumentNullException.ThrowIfNull(goal_hours, nameof(goal_hours));
        ArgumentNullException.ThrowIfNull(goal_mins, nameof(goal_mins));
        ArgumentNullException.ThrowIfNull(goal_secs, nameof(goal_secs));

        try
        {
            await Init_Database();
            GoalPR editing_goal = await Get_PR_Goal(goal_name);

            if (is_weight) /* if weight pr type */
            {
                editing_goal.weight = goal_weight;
            }
            else /* if time pr type */
            {
                editing_goal.time_hours = goal_hours;
                editing_goal.time_min = goal_mins;
                editing_goal.time_sec = goal_secs;
            }

            string date_only;

            if (has_desired_date) /* if a date set for pr goal */
            {
                /* translate DateTime to string; removes the time display */
                string[] date_time_temp = goal_date.ToString().Split(' ');
                date_only = date_time_temp[0];
            }
            else /* else no date of goal set */
            {
                date_only = "N/A";
            }

            editing_goal.goal_achieve_by_date = date_only;
            editing_goal.date_desired = has_desired_date;

            await conn.UpdateAsync(editing_goal);

            status_message = string.Format("{0} PR goal edited", goal_name);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to edit PR goal date: {0}. Error: {1}", goal_name, e.Message);
        }
    }

    /* returns a list of pr goals from the database */
    public async Task<List<GoalPR>> Get_Goal_PR_List()
    {
        try
        {
            await Init_Database();

            List<GoalPR> pr_goal_list = await conn.Table<GoalPR>().ToListAsync();
            pr_goal_list = pr_goal_list.OrderBy(goal => goal.date_desired).ToList();

            List<GoalPR> sorted_pr_goal_list = new List<GoalPR>();
            List<GoalPR> final_list = new List<GoalPR>();

            /* loops through list of pr goal entries found in database */
            for (int i = 0; i < pr_goal_list.Count; i++)
            {
                string[] date_array = pr_goal_list[i].goal_achieve_by_date.Split('/');

                if (pr_goal_list[i].date_desired) /* if goal has no date desired set */
                {
                    pr_goal_list[i].date_sort = new DateTime(int.Parse(date_array[2]), int.Parse(date_array[0]), int.Parse(date_array[1]));
                    sorted_pr_goal_list.Add(pr_goal_list[i]);
                }
                else /* else; no goal date set */
                {
                    final_list.Add(pr_goal_list[i]);
                }
            }

            sorted_pr_goal_list = sorted_pr_goal_list.OrderBy(goal => goal.date_sort).ToList();

            /* adds all date desired pr goals to final list */
            for (int i = 0; i < sorted_pr_goal_list.Count; i++)
            {
                final_list.Add(sorted_pr_goal_list[i]);
            }

            return final_list;
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to retrieve pr goal list. {0}", e.Message);
        }

        return new List<GoalPR>();
    }

    /* returns a pr goal with primary key matching parameter */
    public async Task<GoalPR> Get_PR_Goal(string goal_name)
    {
        GoalPR temp_goal = await conn.FindAsync<GoalPR>(goal_name);
        return temp_goal;
    }

    /* * personal records section */
    /* adds a pr entry to the pr table within the database */
    public async Task Add_PR(string name, DateTime date, bool is_weight_pr_type,
                                double weight_pr, int hours, int min, int sec)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        ArgumentNullException.ThrowIfNull(date, nameof(date));
        ArgumentNullException.ThrowIfNull(is_weight_pr_type, nameof(is_weight_pr_type));
        ArgumentNullException.ThrowIfNull(weight_pr, nameof(weight_pr));
        ArgumentNullException.ThrowIfNull(hours, nameof(hours));
        ArgumentNullException.ThrowIfNull(min, nameof(min));
        ArgumentNullException.ThrowIfNull(sec, nameof(sec));

        try
        {
            await Init_Database();

            /* translate DateTime to string; removes the time display */
            string[] date_time_temp = date.ToString().Split(' ');
            string date_only = date_time_temp[0];

            PR new_pr = new PR
            {
                exercise_name = name,
                date_achieved = date_only,
                is_weight_pr = is_weight_pr_type,
                weight = weight_pr,
                time_hours = hours,
                time_min = min,
                time_sec = sec
            };

            int result = await conn.InsertAsync(new_pr);

            status_message = string.Format("{0} PR added (PR Name: {1})", result, name);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to add PR {0}. Error: {1}", name, e.Message);
        }
    }

    /* updates a pr entry in the pr table within the database */
    public async Task Update_PR(string exercise_name, DateTime date, double updated_weight, int hours, int mins, int sec)
    {
        ArgumentNullException.ThrowIfNull(exercise_name, nameof(exercise_name));
        ArgumentNullException.ThrowIfNull(date, nameof(date));
        ArgumentNullException.ThrowIfNull(updated_weight, nameof(updated_weight));
        ArgumentNullException.ThrowIfNull(hours, nameof(hours));
        ArgumentNullException.ThrowIfNull(mins, nameof(mins));
        ArgumentNullException.ThrowIfNull(sec, nameof(sec));

        try
        {
            await Init_Database();
            PR updating_pr = await conn.FindAsync<PR>(exercise_name);

            /* translate DateTime to string; removes the time display */
            string[] date_time_temp = date.ToString().Split(' ');
            string date_only = date_time_temp[0];

            updating_pr.date_achieved = date_only;

            if (updating_pr.is_weight_pr) /* if updating pr is a weight pr */
            {
                updating_pr.weight = updated_weight;
            }
            else /* else updating pr is a timed pr */
            {
                updating_pr.time_hours = hours;
                updating_pr.time_min = mins;
                updating_pr.time_sec = sec;
            }

            await conn.UpdateAsync(updating_pr);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to update PR. Error: {0}", e.Message);
        }
    }

    /* removes a pr entry in the pr table within the database */
    public async Task Remove_PR(string exercise_name)
    {
        ArgumentNullException.ThrowIfNull(exercise_name, nameof(exercise_name));

        try
        {
            await Init_Database();
            PR removing_pr = await conn.FindAsync<PR>(exercise_name);
            await conn.DeleteAsync(removing_pr);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to remove PR {0}. Error: {1}", exercise_name, e.Message);
        }
    }

    /* returns a pr object from database with primary key matching parameter */
    public async Task<PR> Get_PR(string exercise_name)
    {
        PR updating_pr = await conn.FindAsync<PR>(exercise_name);
        return updating_pr;
    }

    /* returns a list of PR's from the database */
    public async Task<List<PR>> Get_PR_List()
    {
        try
        {
            await Init_Database();
            List<PR> pr_list = await conn.Table<PR>().ToListAsync();
            pr_list = pr_list.OrderBy(pr => pr.exercise_name).ToList();
            return pr_list;
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to retrieve PR list. {0}", e.Message);
        }

        return new List<PR>();
    }

    /* * workout calendar section*/
    /* adds an entry to the workout calendar table within the database */
    public async Task Add_Calendar_Entry(string category_name, DateTime date)
    {
        ArgumentNullException.ThrowIfNull(date, nameof(date));
        ArgumentNullException.ThrowIfNull(category_name, nameof(category_name));

        try
        {
            await Init_Database();

            CalendarEntry calendar_entry = new CalendarEntry
            {
                entry_date = date,
                calendar_category_name = category_name
            };

            int result = await conn.InsertAsync(calendar_entry);

            status_message = string.Format("{0} calendar entry made for {1})", result, date);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to add calendar entry on {0}. Error: {1}", date, e.Message);
        }
    }

    /* removes an entry in the workout calendar table within the database */
    public async Task Remove_Calendar_Entry(DateTime date, Category category)
    {
        ArgumentNullException.ThrowIfNull(date, nameof(date));
        ArgumentNullException.ThrowIfNull(category, nameof(category));

        try
        {
            await Init_Database();

            List<CalendarEntry> day_entries_list = await Get_Calendar_Entries_List(date); /* gets list of all entries for that day */

            /* gets list of calendar entries for selected date that match category for removal */
            List<CalendarEntry> day_entries_list_matching_removal_category = new List<CalendarEntry>();
            foreach (CalendarEntry day_entry in day_entries_list)
            {
                if (day_entry.calendar_category_name == category.name) /* if category matches removal category */
                {
                    day_entries_list_matching_removal_category.Add(day_entry);
                }
            }

            /* removes 1 instance of entry for that day */
            int result = await conn.DeleteAsync(day_entries_list_matching_removal_category[0]);

            status_message = string.Format("{0} calendar entry removed (Date: {1})", result, date);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to remove calendar entry on {0}. Error: {1}", date, e.Message);
        }
    }

    /* returns a list of all calendar entries from the database with a date equal to the parameter */
    public async Task<List<CalendarEntry>> Get_Calendar_Entries_List(DateTime date)
    {
        ArgumentNullException.ThrowIfNull(date, nameof(date));

        try
        {
            await Init_Database();
            List<CalendarEntry> calendar_entry_list = await conn.Table<CalendarEntry>().Where(d => d.entry_date == date).ToListAsync();
            return calendar_entry_list;
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to retrieve calendar entry list. {0}", e.Message);
        }

        return new List<CalendarEntry>();
    }

    /* creates a category to the categories table within the database */
    public async Task Add_Calendar_Category(string category_name)
    {
        ArgumentNullException.ThrowIfNull(category_name, nameof(category_name));

        try
        {
            await Init_Database();

            /* if category already exists but marked as not available; mark as available */
            Category existing_category = await Get_Category(category_name);
            int result;

            if (existing_category != null) /* if category already exists; mark as available */
            {
                existing_category.still_available = true;
                result = await conn.UpdateAsync(existing_category);
            }
            else /* else no record of category; create a new category */
            {
                Category new_category = new Category
                {
                    name = category_name,
                    still_available = true
                };

                result = await conn.InsertAsync(new_category);
            }

            status_message = string.Format("{0} calendar category added (Category Name: {1})", result, category_name);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to add calendar category {0}. Error: {1}", category_name, e.Message);
        }
    }

    /* removes a category in the categories table within the database */
    public async Task Remove_Calendar_Category(string category_name)
    {
        ArgumentNullException.ThrowIfNull(category_name, nameof(category_name));

        try
        {
            await Init_Database();

            Category removing_category = await conn.FindAsync<Category>(category_name);

            removing_category.still_available = false;

            await conn.UpdateAsync(removing_category);

            status_message = string.Format("calendar category removed (Category Name: {1})", category_name);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to remove calendar category {0}. Error: {1}", category_name, e.Message);
        }
    }

    /* returns a list of all calendar categories that are still available from the database */
    public async Task<List<Category>> Get_Calendar_Category_List()
    {
        try
        {
            await Init_Database();

            List<Category> category_list = await conn.Table<Category>().ToListAsync();
            List<Category> available_category_list = new List<Category>();

            /* do not include unavailable categories */
            foreach (Category category in category_list)
            {
                if (category.still_available)
                {
                    available_category_list.Add(category);
                }
            }

            available_category_list = available_category_list.OrderBy(c => c.name).ToList();

            return available_category_list;
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to retrieve calendar category list. {0}", e.Message);
        }

        return new List<Category>();
    }

    /* returns a category object from database with primary key matching parameter */
    public async Task<Category> Get_Category(string category_name)
    {
        Category temp_category = await conn.FindAsync<Category>(category_name);
        return temp_category;
    }

    /* adds a note to the database */
    public async Task Add_Note(string note_name, string note_content)
    {
        ArgumentNullException.ThrowIfNull(note_name, nameof(note_name));
        ArgumentNullException.ThrowIfNull(note_content, nameof(note_content));

        try
        {
            await Init_Database();
            DateTime current_date = DateTime.Now;

            /* translate DateTime to string; removes the time display */
            string date_only;
            string[] date_time_temp = current_date.ToString().Split(' ');
            date_only = date_time_temp[0];

            Note new_note = new Note
            {
                name = note_name,
                content = note_content,
                last_edited_date_string = date_only,
                last_edited_date = current_date
            };

            await conn.InsertAsync(new_note);

            status_message = string.Format("{0} note added", note_name);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to add note: {0}. Error: {1}", note_name, e.Message);
        }
    }

    /* removes a note to the database */
    public async Task Remove_Note(string note_name)
    {
        ArgumentNullException.ThrowIfNull(note_name, nameof(note_name));

        try
        {
            await Init_Database();
            Note removing_note = await conn.FindAsync<Note>(note_name);
            await conn.DeleteAsync(removing_note);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to remove {0} from notes. Error: {1}", note_name, e.Message);
        }
    }

    /* updates a note in the database */
    public async Task Edit_Note(string note_name, string note_content)
    {
        ArgumentNullException.ThrowIfNull(note_name, nameof(note_name));
        ArgumentNullException.ThrowIfNull(note_content, nameof(note_content));

        try
        {
            await Init_Database();

            DateTime current_date = DateTime.Now;

            /* translate DateTime to string; removes the time display */
            string date_only;
            string[] date_time_temp = current_date.ToString().Split(' ');
            date_only = date_time_temp[0];

            Note updating_note = await conn.FindAsync<Note>(note_name);
            updating_note.content = note_content;
            updating_note.last_edited_date_string = date_only;
            updating_note.last_edited_date = current_date;

            await conn.UpdateAsync(updating_note);
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to edit note. Error: {0}", e.Message);
        }
    }

    /* returns a list of notes found in database */
    public async Task<List<Note>> Get_All_Notes()
    {
        try
        {
            await Init_Database();

            List<Note> notes_list = await conn.Table<Note>().ToListAsync();
            notes_list = notes_list.OrderBy(note => note.last_edited_date).Reverse().ToList();

            return notes_list;
        }
        catch (Exception e)
        {
            status_message = string.Format("Failed to retrieve note list. {0}", e.Message);
        }

        return new List<Note>();
    }

    /* returns a single note from the database matching the parameter primary key */
    public async Task<Note> Get_Note(string note_name)
    {
        Note temp_note = await conn.FindAsync<Note>(note_name);
        return temp_note;
    }
}