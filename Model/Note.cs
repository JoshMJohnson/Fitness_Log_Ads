using SQLite;

namespace Fitness_Log.Model;

[Table("Notes")]
public class Note
{
    [PrimaryKey]
    public string name { get; set; }

    public string content { get; set; }

    public string last_edited_date_string { get; set; }
    public DateTime last_edited_date { get; set; }
}