using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;
using mxtrAutomation.Common.Extensions;
using mxtrAutomation.Common.Ioc;

namespace mxtrAutomation.Web.Common.UI
{
    public interface IClientResourceBundleManager
    {
        IEnumerable<string> CreateCssBundles<T, U>(List<IncludedResource<T>> resources);
        IEnumerable<string> CreateJSBundles<T, U>(List<IncludedResource<T>> resources);
    }

    public class ClientResourceBundleManager : IClientResourceBundleManager
    {
        public IEnumerable<KeyValuePair<string, string[]>> CreateBundles<T, U>(List<IncludedResource<T>> resources)
        {
            if (resources == null || resources.Count == 0)
                yield break;

            IClientResourceCollection<T, U> cssCollection =
                ServiceLocator.Current.GetInstance<IClientResourceCollection<T, U>>();

            IEnumerable<IGrouping<BundleKind, IncludedResource<T>>> bundles =
                resources
                    .GroupBy(r => r.BundleKind)
                    .OrderBy(b => b.Key);

            List<string> bundleNames = new List<string>();
            List<T> includedResources = new List<T>();

            foreach (IGrouping<BundleKind, IncludedResource<T>> bundle in bundles)
            {
                List<T> sortedResources =
                    cssCollection
                        .SortResources(bundle.Select(b => b.ResourceKind))
                        .Where(r => !includedResources.Contains(r))
                        .ToList();

                includedResources.AddRange(sortedResources);

                string bundleName =
                    "~/bundles/combine-{0}-{1}"
                        .With(bundle.Key, sortedResources.Select(r => (r as ResourceKindBase<string>).Value).ToString("-"))
                        .ToLower();

                string[] resourcePaths =
                    sortedResources
                        .Where(cssCollection.ContainsKey)
                        .Select(r => cssCollection[r].Value)
                        .ToArray();

                yield return new KeyValuePair<string, string[]>(bundleName, resourcePaths);
            }
   
        }

        public IEnumerable<string> CreateCssBundles<T,U>(List<IncludedResource<T>> resources)
        {
            List<KeyValuePair<string, string[]>> bundles = CreateBundles<T, U>(resources).ToList();

            foreach (var bundle in bundles.Where(bundle => !BundleTable.Bundles.Exists(b => b.Path == bundle.Key)))
            {
                BundleTable.Bundles.Add(new StyleBundle(bundle.Key).Include(bundle.Value));
            }

            return bundles.Select(b => b.Key);
        }

        public IEnumerable<string> CreateJSBundles<T, U>(List<IncludedResource<T>> resources)
        {
            List<KeyValuePair<string, string[]>> bundles = CreateBundles<T, U>(resources).ToList();

            foreach (var bundle in bundles.Where(bundle => !BundleTable.Bundles.Exists(b => b.Path == bundle.Key)))
            {
                BundleTable.Bundles.Add(new ScriptBundle(bundle.Key).Include(bundle.Value));
            }

            return bundles.Select(b => b.Key);
        }
    }
}
