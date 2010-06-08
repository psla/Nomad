using Nomad.Regions.Adapters;
using TestsShared;

namespace Nomad.Tests.UnitTests.Regions.Adapters
{
    [UnitTests]
    public class TabControlAdapterContract : AdapterContractBase<TabControlAdapter>
    {
        protected override TabControlAdapter GetAdapter()
        {
            return new TabControlAdapter();
        }
    }
}