using CommunityToolkit.Maui.Views;
using Fitness_Log.Popup;
using Fitness_Log.Model;
using Plugin.MauiMTAdmob;

namespace Fitness_Log;

public partial class GoalsPR : ContentPage
{
    private VerticalStackLayout vertical_layout_empty_pr_goal_list;

    public GoalsPR()
	{
		InitializeComponent();

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += (s, args) =>
        {
            CrossMauiMTAdmob.Current.ShowInterstitial();
        };

        Empty_PR_Goal_List_Display();
        Refresh_PR_Goal_List();
    }

    private async void Add_PR_Goal(object sender, EventArgs e)
    {
        object result = await this.ShowPopupAsync(new GoalPRPopup());

        if (result != null) /* if body weight entry was made */
        {
            string result_string = result.ToString();

            if (result_string == "True") /* if valid entry */
            {
                Refresh_PR_Goal_List();
            }
            else
            {
                await this.ShowPopupAsync(new DuplicateEntryPopup("PR Goal"));
            }
        }
    }

    /* swipe remove pr goal */
    private async void Remove_PR_Goal(object sender, EventArgs e)
    {
        SwipeItem remove_pr = (SwipeItem)sender;
        string pr_name = remove_pr.BindingContext.ToString();

        await App.RecordRepo.Remove_Goal_PR(pr_name);
        Show_Intestitial();
        Refresh_PR_Goal_List();
    }

    /* shows intestitial video ad */
    private void Show_Intestitial()
    {
        CrossMauiMTAdmob.Current.LoadInterstitial("ca-app-pub-6232744288972049/4461146967");
    }

    /* swipe edit pr goal */
    private async void Edit_PR_Goal(object sender, EventArgs e)
    {
        SwipeItem edit_pr_goal = (SwipeItem)sender;
        string goal_name = edit_pr_goal.BindingContext.ToString();

        await this.ShowPopupAsync(new GoalPREditPopup(goal_name));
        Refresh_PR_Goal_List();
    }

    /* refreshes the PR goal list being displayed on UI */
    public async void Refresh_PR_Goal_List()
    {
        List<GoalPR> pr_goal_list = await App.RecordRepo.Get_Goal_PR_List();

        pr_goals_display.ItemsSource = pr_goal_list;

        if (pr_goal_list.Count == 0)  /* if pr list is empty - no pr's set */
        {
            vertical_layout_empty_pr_goal_list.IsVisible = true;
        }
        else  /* else pr list is not empty */
        {
            vertical_layout_empty_pr_goal_list.IsVisible = false;
        }
    }

    /* creates message for empty pr goal list */
    private void Empty_PR_Goal_List_Display()
    {
        vertical_layout_empty_pr_goal_list = new VerticalStackLayout();

        vertical_layout_empty_pr_goal_list.VerticalOptions = LayoutOptions.Center;
        vertical_layout_empty_pr_goal_list.HorizontalOptions = LayoutOptions.Center;

        vertical_layout_empty_pr_goal_list.Add(new Label
        {
            Text = "No Personal Record Goals!",
            FontSize = 20,
            FontAttributes = FontAttributes.Italic,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            MinimumWidthRequest = 250
        });

        Image empty_list_image = new Image();
        empty_list_image.SetAppTheme<FileImageSource>(Image.SourceProperty, "empty_list.png", "empty_list_dark.png");
        empty_list_image.HeightRequest = 200;
        vertical_layout_empty_pr_goal_list.Add(empty_list_image);

        Grid goal_layout = goal_pr_layout;
        Grid.SetRow(vertical_layout_empty_pr_goal_list, 0);
        goal_layout.Add(vertical_layout_empty_pr_goal_list);
    }
}