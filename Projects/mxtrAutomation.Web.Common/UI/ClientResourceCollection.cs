using System.Collections.Generic;
using System.Linq;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Web.Common.UI
{
    public interface IClientResourceCollection<KIND, CATEGORY> : IDictionary<KIND, ClientResourceBase<KIND, CATEGORY>>
    {
        IEnumerable<KIND> GetDependencyTree(KIND resourceKind);
        IEnumerable<KIND> SortResources(IEnumerable<KIND> resources);
    }

    public class ClientResourceCollection<KIND, CATEGORY> : Dictionary<KIND, ClientResourceBase<KIND, CATEGORY>>, IClientResourceCollection<KIND, CATEGORY>
    {
        // ReSharper disable PossibleMultipleEnumeration
        public IEnumerable<KIND> GetDependencyTree(KIND resourceKind)
        {
            IEnumerable<KIND> dependencies =
                this[resourceKind].Dependencies;

            if (dependencies == null)
                return new KIND[] {};

            return dependencies.Union(dependencies.SelectMany(GetDependencyTree)).Distinct();
        }

        public IEnumerable<KIND> SortResources(IEnumerable<KIND> resources)
        {
            List<KIND> allResources =
                resources.Union(resources.SelectMany(GetDependencyTree)).Distinct().ToList();

            List<KIND> sortedResources = new List<KIND>();

            while (allResources.Count > 0)
            {
                List<KIND> resourcesWithNoDepencies =
                    allResources.Where(r => GetDependencyTree(r).Where(x => !sortedResources.Contains(x)).IsNullOrEmpty()).ToList();

                foreach (KIND clientResource in resourcesWithNoDepencies)
                {
                    sortedResources.Add(clientResource);
                    allResources.Remove(clientResource);
                }
            }

            return sortedResources.OrderBy(r => this[r].Category);
        }
        // ReSharper restore PossibleMultipleEnumeration

#if false
        protected virtual IEnumerable<ClientResource> SortResources(IEnumerable<ClientResource> resources)
        {
            List<ClientResource> allResources =
                resources.Union(resources.SelectMany(r => r.GetDependencyTree())).Distinct().ToList();

            List<ClientResource> sortedResources = new List<ClientResource>();

            while (allResources.Count > 0)
            {
                List<ClientResource> resourcesWithNoDepencies =
                    allResources.Where(r => r.GetDependencyTree().Where(x => !sortedResources.Any(s => x.Key == s.Key)).IsNullOrEmpty()).ToList();

                foreach (var clientResource in resourcesWithNoDepencies)
                {
                    sortedResources.Add(clientResource);
                    allResources.Remove(clientResource);
                }                
            }

            return sortedResources;
        }
#endif
    }
}
