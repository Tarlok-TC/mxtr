using mxtrAutomation.Websites.Platform.Models.Customer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using mxtrAutomation.Corporate.Data.Enums;

namespace mxtrAutomation.Websites.Platform.ViewModelAdapters
{
    public interface ICustomerViewModelAdapter
    {
        CustomerViewModel BuildIndexViewModel();
    }
    public class CustomerViewModelAdapter : ICustomerViewModelAdapter
    {
        public CustomerViewModel BuildIndexViewModel()
        {
            CustomerViewModel model = new CustomerViewModel();

            AddPageTitle(model);

            return model;
        }
        private void AddPageTitle(CustomerViewModel model)
        {
            if (model.CustomerActionKind == Enums.CustomerActionKind.Add)
                model.PageTitle = "Add Customer";
            model.PageTitle = "Customer Portal";
            
            model.ShowWorkspaceFilter = false;
        }
    }
}