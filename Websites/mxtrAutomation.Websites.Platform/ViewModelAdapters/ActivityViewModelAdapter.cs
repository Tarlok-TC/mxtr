using mxtrAutomation.Websites.Platform.Models.Activity.ViewModels;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface IActivityViewModelAdapter
    {
        ActivityViewModel BuildActivityViewModel();
    }

    public class ActivityViewModelAdapter : IActivityViewModelAdapter
    {
        public ActivityViewModel BuildActivityViewModel()
        {
            ActivityViewModel model = new ActivityViewModel();        

            AddPageTitle(model);

            return model;
        }

        public void AddPageTitle(ActivityViewModel model)
        {
            model.PageTitle = "Activity";
        }
    }
}
