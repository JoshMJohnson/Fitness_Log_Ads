using Fitness_Log.Model;

namespace Fitness_Log.Popup;

public partial class DuplicateEntryPopup
{
    public DuplicateEntryPopup(string table_name)
    {
        InitializeComponent();

        /* if duplicate entry is a body progression entry */
        if (table_name == "Body Progression")
        {
            Display_Body_Progression(table_name);
        }
        else if (table_name == "Body Weight") /* else if duplicate entry is a body weight entry */
        {
            Display_Body_Weight(table_name);
        }
        else /* else if duplicate entry must have a unique name */
        {
            Display_Unique_Name(table_name);
        }
    }

    /* displays body progression duplicate info */
    private void Display_Body_Progression(string table_name)
    {
        string prompt = $"The {table_name} entry is a duplicate.";
        string prompt2 = $"The {table_name} entry was not added since the image is already saved as a previous progression.";

        duplicate_table_prompt_display.Text = prompt;
        duplicate_table_prompt2_display.Text = prompt2;
    }

    /* displays body weight duplicate info */
    private void Display_Body_Weight(string table_name)
    {
        string prompt = $"The {table_name} entry was not added since an entry already exists for that date.";
        string prompt2 = $"Only 1 {table_name} entry can be recorded per day.";

        duplicate_table_prompt_display.Text = prompt;
        duplicate_table_prompt2_display.Text = prompt2;
    }

    /* displays body weight goal duplicate info */
    private void Display_Unique_Name(string table_name)
    {
        string prompt = $"The {table_name} is a duplicate.";
        string prompt2 = $"A {table_name} must have a unique name.";

        duplicate_table_prompt_display.Text = prompt;
        duplicate_table_prompt2_display.Text = prompt2;
    }

    /* closes the popup alert for duplicate */
    private void Close_Popup(object sender, EventArgs e)
    {
        Close();
    }
}