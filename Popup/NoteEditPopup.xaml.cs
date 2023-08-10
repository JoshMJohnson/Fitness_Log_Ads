using Fitness_Log.Model;

namespace Fitness_Log.Popup;

public partial class NoteEditPopup
{
    private Note current_note { get; set; }
    private int max_char_value { get; set; }
    private int current_char_value { get; set; }

    public NoteEditPopup(string note_name)
    {
        InitializeComponent();

        current_char_value = 0;
        max_char_value = 300;

        Prepare_Existing_Note(note_name);
    }

    /* fills in all fields with current note data */
    private async void Prepare_Existing_Note(string note_name)
    {
        current_note = await App.RecordRepo.Get_Note(note_name);

        /* note length variables */
        current_char_value_display.Text = current_char_value.ToString();
        max_char_value_display.Text = max_char_value.ToString();

        /* note specific data */
        note_title_content.Text = note_name;
        text_box_display.Text = current_note.content;
    }

    /* refreshes the current char value of the note text box */
    private void Refresh_Word_Count(object sender, EventArgs e)
    {
        current_char_value = text_box_display.Text.Length;
        current_char_value_display.Text = current_char_value.ToString();
    }

    /* executes when save button is clicked */
    private async void Save_Note(object sender, EventArgs e)
    {
        string name = note_title_content.Text;

        if (name != null && name.Length != 0) /* if name field is not empty */
        {
            name = name.Trim();

            if (name != null && name.Length != 0) /* if name field is not empty after trim */
            {
                string note_content = text_box_display.Text;

                if (note_content == null || note_content.Length == 0) /* if no note content */
                {
                    note_content = "";
                }

                await App.RecordRepo.Edit_Note(name, note_content);

                error_prompt.IsVisible = false;
                Close();
            }
            else /* else; name field is empty after trim */
            {
                error_prompt.Text = "Note title cannot be empty";
                error_prompt.IsVisible = true;
            }
        }
        else /* else; name field is empty */
        {
            error_prompt.Text = "Note title cannot be empty";
            error_prompt.IsVisible = true;
        }
    }

    /* executes when delete note button is clicked */
    private async void Delete_Note(object sender, EventArgs e)
    {
        string note_name = current_note.name;
        await App.RecordRepo.Remove_Note(note_name);
        Close();
    }

    /* closes the note popup */
    private void Cancel_Record(object sender, EventArgs e)
    {
        Close();
    }
}