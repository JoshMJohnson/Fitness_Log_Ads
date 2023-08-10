using Microsoft.Maui.Controls.Shapes;
using Fitness_Log.Model;

namespace Fitness_Log.Popup;

public partial class CalendarCategoryAddPopup
{
    public CalendarCategoryAddPopup()
    {
        InitializeComponent();
    }

    /* closes popup for creating a category */
    private void Cancel_Create_Category(object sender, EventArgs e)
    {
        Close();
    }

    /* submits category creation */
    private async void Create_Category(object sender, EventArgs e)
    {
        string name = category_name.Text;

        if (name != null && name.Length != 0) /* if name field is not empty */
        {
            /* name appearance tweaking */
            name = name.Trim(); /* removes leading and trailing whitespace */
            name = string.Concat(char.ToUpper(name[0]), name.Substring(1));

            List<Category> categories_before = await App.RecordRepo.Get_Calendar_Category_List();
            await App.RecordRepo.Add_Calendar_Category(name);
            List<Category> categories_after = await App.RecordRepo.Get_Calendar_Category_List();

            error_prompt.IsVisible = false;

            if (categories_before.Count == categories_after.Count) /* if duplicate entry */
            {
                Close(false);
            }
            else /* else; valid entry; not a duplicate */
            {
                Close(true);
            }
        }
        else /* else; name field is empty */
        {
            error_prompt.Text = "Category name cannot be empty";
            error_prompt.IsVisible = true;
        }
    }
}