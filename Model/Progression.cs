using SQLite;

namespace Fitness_Log.Model;

[Table("Progressions")]
public class Progression
{
    [PrimaryKey]
    public string image_full_path { get; set; }

    public string date { get; set; }
    public DateTime date_sort { get; set; }
}