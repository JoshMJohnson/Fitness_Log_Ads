using SQLite;

namespace Fitness_Log.Model;

[Table("BodyWeight")]
public class BodyWeightEntry
{
    [PrimaryKey]
    public DateTime date { get; set; }

    [NotNull]
    public double weight { get; set; }
}