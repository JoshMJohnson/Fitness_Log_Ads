using SQLite;

namespace Fitness_Log.Model;

[Table("CalendarEntries")]
public class CalendarEntry
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    [NotNull]
    public DateTime entry_date { get; set; }

    [NotNull]
    public string calendar_category_name { get; set; }
}