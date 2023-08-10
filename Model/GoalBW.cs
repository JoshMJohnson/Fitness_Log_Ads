using SQLite;
using System.ComponentModel;

namespace Fitness_Log.Model;

[Table("GoalsBW")]
public class GoalBW
{
    [PrimaryKey]
    public string name { get; set; }

    public string goal_achieve_by_date { get; set; }
    public bool date_desired { get; set; }

    public double weight { get; set; }

    public DateTime date_sort { get; set; }
}