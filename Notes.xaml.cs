using CommunityToolkit.Maui.Views;
using Fitness_Log.Popup;
using Fitness_Log.Model;
using System.Windows.Input;

namespace Fitness_Log;

public partial class Notes : ContentPage
{
    private VerticalStackLayout vertical_layout_empty_notes_list;

    public Notes()
	{
		InitializeComponent();
        Empty_Notes_Display();
        Refresh_Notes();
    }

    /* creates message for empty categories */
    private void Empty_Notes_Display()
    {
        /* display when PR list is empty */
        vertical_layout_empty_notes_list = new VerticalStackLayout();

        vertical_layout_empty_notes_list.VerticalOptions = LayoutOptions.Center;
        vertical_layout_empty_notes_list.HorizontalOptions = LayoutOptions.Center;

        vertical_layout_empty_notes_list.Add(new Label
        {
            Text = "No Notes!",
            FontSize = 20,
            FontAttributes = FontAttributes.Italic,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            MinimumWidthRequest = 175
        });

        Image empty_list_image = new Image();
        empty_list_image.SetAppTheme<FileImageSource>(Image.SourceProperty, "empty_list.png", "empty_list_dark.png");
        empty_list_image.HeightRequest = 200;
        vertical_layout_empty_notes_list.Add(empty_list_image);

        Grid empty_display = notes_layout;
        Grid.SetRow(vertical_layout_empty_notes_list, 0);
        empty_display.Add(vertical_layout_empty_notes_list);
    }

    /* executed when the Notes plus button clicked */
    private async void Add_Note(object sender, EventArgs e)
    {
        object result = await this.ShowPopupAsync(new NotesAddPopup());

        if (result != null) /* if body weight entry was made */
        {
            string result_string = result.ToString();

            if (result_string == "True") /* if valid entry */
            {
                Refresh_Notes();
            }
            else
            {
                await this.ShowPopupAsync(new DuplicateEntryPopup("Note"));
            }
        }
    }

    /* handles transition to view note */
    private async void View_Note(object sender, SelectionChangedEventArgs note_clicked)
    {
        string note_name = (note_clicked.CurrentSelection.FirstOrDefault() as Note)?.name;
        var edit_clicked = await this.ShowPopupAsync(new NoteDisplayPopup(note_name));

        if (edit_clicked != null) /* if chosen to edit note */
        {
            await this.ShowPopupAsync(new NoteEditPopup(note_name));
        }

        Refresh_Notes();
    }

    /* refreshes the display of notes */
    private async void Refresh_Notes()
    {
        List<Note> notes_list = await App.RecordRepo.Get_All_Notes();

        notes_display.ItemsSource = notes_list;

        if (notes_list.Count == 0)  /* if pr list is empty - no pr's set */
        {
            vertical_layout_empty_notes_list.IsVisible = true;
        }
        else  /* else pr list is not empty */
        {
            vertical_layout_empty_notes_list.IsVisible = false;
        }
    }
}