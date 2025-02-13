using mxtrAutomation.Websites.Platform.Models.NextDayRoute.ViewModels;
using mxtrAutomation.Websites.Platform.Queries;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface INextDayRouteViewModelAdapter
    {
        NextDayRouteViewModel BuildNextDayRouteViewModel();
    }
    public class NextDayRouteViewModelAdapter : INextDayRouteViewModelAdapter
    {
        public NextDayRouteViewModel BuildNextDayRouteViewModel()
        {
            NextDayRouteViewModel model = new NextDayRouteViewModel();

            AddPageTitle(model);
            return model;
        }
        public void AddPageTitle(NextDayRouteViewModel model)
        {
            model.PageTitle = "NextDayRoute";
        }
    }
}