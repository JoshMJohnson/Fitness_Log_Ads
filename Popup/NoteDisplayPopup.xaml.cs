using Fitness_Log.Popup;
using Fitness_Log.Model;

namespace Fitness_Log.Popup;

public partial class NoteDisplayPopup
{
    private Note current_note { get; set; }

    public NoteDisplayPopup(string note_name)
    {
        InitializeComponent();
        View_Note(note_name);
    }

    /* displays the notes content */
    private async void View_Note(string note_name)
    {
        current_note = await App.RecordRepo.Get_Note(note_name);

        note_name_display.Text = current_note.name;
        last_modified_display.Text = current_note.last_edited_date_string;
        note_content_display.Text = current_note.content;
    }

    /* executes when update note button clicked */
    private void Edit_Note(object sender, EventArgs e) => Close(true);

    /* executes when delete note button is clicked */
    private async void Delete_Note(object sender, EventArgs e)
    {
        string note_name = current_note.name;
        await App.RecordRepo.Remove_Note(note_name);
        Close();
    }

    /* closes the note view */
    private void Close_Note(object sender, EventArgs e)
    {
        Close();
    }
}