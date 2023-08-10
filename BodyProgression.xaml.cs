using CommunityToolkit.Maui.Views;
using Fitness_Log.Popup;
using Fitness_Log.Model;

namespace Fitness_Log;

public partial class BodyProgression : ContentPage
{
    private VerticalStackLayout vertical_layout_body_progression_empty;

    public BodyProgression()
	{
		InitializeComponent();
        Empty_Progression_Display();
        Refresh_Progression();
    }

    /* executed when PR plus button clicked */
    private async void Add_Body_Progression(object sender, EventArgs e)
    {
        object result = await this.ShowPopupAsync(new BodyProgressionPopup());

        if (result != null) /* if body weight entry was made */
        {
            string result_string = result.ToString();

            if (result_string == "True") /* if valid entry */
            {
                Refresh_Progression();
            }
            else
            {
                await this.ShowPopupAsync(new DuplicateEntryPopup("Body Progression"));
            }
        }
    }

    /* handles transition to view progression */
    private async void View_Progression(object sender, SelectionChangedEventArgs progression_clicked)
    {
        string progression_name = (progression_clicked.CurrentSelection.FirstOrDefault() as Progression)?.image_full_path;
        await this.ShowPopupAsync(new BodyProgressionDisplayPopup(progression_name));
        Refresh_Progression();
    }

    /* refreshes the PR list being displayed on UI */
    public async void Refresh_Progression()
    {
        List<Progression> progression_list = await App.RecordRepo.Get_Progression_List();

        progression_list_display.ItemsSource = progression_list;

        if (progression_list.Count == 0)  /* if body progression list is empty - no body progression's set */
        {
            vertical_layout_body_progression_empty.IsVisible = true;
        }
        else  /* else pr list is not empty */
        {
            vertical_layout_body_progression_empty.IsVisible = false;
        }
    }

    /* creates message for empty categories */
    private void Empty_Progression_Display()
    {
        vertical_layout_body_progression_empty = new VerticalStackLayout();

        vertical_layout_body_progression_empty.VerticalOptions = LayoutOptions.Center;
        vertical_layout_body_progression_empty.HorizontalOptions = LayoutOptions.Center;

        vertical_layout_body_progression_empty.Add(new Label
        {
            Text = "No Progressions Saved!",
            FontSize = 20,
            FontAttributes = FontAttributes.Italic,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            MinimumWidthRequest = 250
        });

        Image empty_list_image = new Image();
        empty_list_image.SetAppTheme<FileImageSource>(Image.SourceProperty, "empty_list.png", "empty_list_dark.png");
        empty_list_image.HeightRequest = 200;
        vertical_layout_body_progression_empty.Add(empty_list_image);

        Grid goal_layout = progression_layout;
        Grid.SetRow(vertical_layout_body_progression_empty, 0);
        goal_layout.Add(vertical_layout_body_progression_empty);
    }
}