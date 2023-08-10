using CommunityToolkit.Maui.Views;
using Fitness_Log.Popup;
using Fitness_Log.Model;

namespace Fitness_Log;

public partial class GoalsBodyWeight : ContentPage
{
    private VerticalStackLayout vertical_layout_empty_bw_goal_list;

    public GoalsBodyWeight()
	{
		InitializeComponent();
        Empty_Body_Weight_Goals_Display();
        Refresh_BW_Goal_List();
    }

    /* submits a body weight goal to the database */
    private async void Add_Body_Weight_Goal(object sender, EventArgs e)
    {
        object result = await this.ShowPopupAsync(new GoalBodyWeightPopup());

        if (result != null) /* if body weight entry was made */
        {
            string result_string = result.ToString();

            if (result_string == "True") /* if valid entry */
            {
                Refresh_BW_Goal_List();
            }
            else
            {
                await this.ShowPopupAsync(new DuplicateEntryPopup("Body Weight Goal"));
            }
        }
    }

    /* swipe remove BW goal */
    private async void Remove_Body_Weight_Goal(object sender, EventArgs e)
    {
        SwipeItem remove_bw_goal = (SwipeItem)sender;
        string goal_name = remove_bw_goal.BindingContext.ToString();

        await App.RecordRepo.Remove_Goal_Body_Weight(goal_name);
        Refresh_BW_Goal_List();
    }

    /* swipe edit BW goal */
    private async void Edit_Body_Weight_Goal(object sender, EventArgs e)
    {
        SwipeItem edit_bw_goal = (SwipeItem)sender;
        string goal_name = edit_bw_goal.BindingContext.ToString();

        await this.ShowPopupAsync(new GoalBWEditPopup(goal_name));
        Refresh_BW_Goal_List();
    }

    /* refreshes the BW goal list being displayed on UI */
    public async void Refresh_BW_Goal_List()
    {
        List<GoalBW> body_weight_goal_list = await App.RecordRepo.Get_Body_Weight_Goal_List();

        body_weight_goals_display.ItemsSource = body_weight_goal_list;

        if (body_weight_goal_list.Count == 0)  /* if pr list is empty - no pr's set */
        {
            vertical_layout_empty_bw_goal_list.IsVisible = true;
        }
        else  /* else pr list is not empty */
        {
            vertical_layout_empty_bw_goal_list.IsVisible = false;
        }
    }

    /* creates message for empty body weight goal list */
    private void Empty_Body_Weight_Goals_Display()
    {
        vertical_layout_empty_bw_goal_list = new VerticalStackLayout();

        vertical_layout_empty_bw_goal_list.VerticalOptions = LayoutOptions.Center;
        vertical_layout_empty_bw_goal_list.HorizontalOptions = LayoutOptions.Center;

        vertical_layout_empty_bw_goal_list.Add(new Label
        {
            Text = "No Body Weight Goals!",
            FontSize = 20,
            FontAttributes = FontAttributes.Italic,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            MinimumWidthRequest = 250
        });

        Image empty_list_image = new Image();
        empty_list_image.SetAppTheme<FileImageSource>(Image.SourceProperty, "empty_list.png", "empty_list_dark.png");
        empty_list_image.HeightRequest = 200;
        vertical_layout_empty_bw_goal_list.Add(empty_list_image);

        Grid goal_layout = goal_bw_layout;
        Grid.SetRow(vertical_layout_empty_bw_goal_list, 0);
        goal_layout.Add(vertical_layout_empty_bw_goal_list);
    }
}