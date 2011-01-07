using System;
using System.Windows;
using Moq;
using Nomad.Regions;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Regions
{
    /// <summary>
    ///     Specifies default implementation of <see cref="IRegionFactory"/>
    /// </summary>
    [UnitTests]
    public class RegionCreation
    {
        private readonly Mock<IRegion> _regionMock;
        private readonly IRegion _region;
        private readonly FrameworkElement _view;


        public RegionCreation()
        {
            _regionMock = new Mock<IRegion>();
            _region = _regionMock.Object;
            _view = new FrameworkElement();
        }


        /// <summary>
        ///     Specifies behavior of <see cref="RegionFactory"/> when an adapter exactly matching the 
        ///     type of view exists.
        /// </summary>
        [Test]
        public void when_matching_adapter_exists_a_region_is_created()
        {
            var regionAdapterMock = new Mock<IRegionAdapter>();
            regionAdapterMock
                .SetupGet(adapter => adapter.SupportedType)
                .Returns(typeof (FrameworkElement));
            regionAdapterMock
                .Setup(adapter => adapter.AdaptView(_view))
                .Returns(_region);

            var regionFactory = new RegionFactory(new[] {regionAdapterMock.Object});
            var createdRegion = regionFactory.CreateRegion(_view);

            Assert.AreSame(_region, createdRegion);
        }


        [Test]
        public void when_no_matching_adapter_is_found_an_exception_is_thrown()
        {
            var regionFactory = new RegionFactory(new IRegionAdapter[] {});

            Assert.Throws<InvalidOperationException>(() => regionFactory.CreateRegion(_view));
        }


        [Test]
        public void chooses_most_specific_adapter()
        {
            var specificRegionAdapterMock = new Mock<IRegionAdapter>();
            specificRegionAdapterMock
                .SetupGet(adapter => adapter.SupportedType)
                .Returns(typeof (FrameworkElement));
            specificRegionAdapterMock
                .Setup(adapter => adapter.AdaptView(_view))
                .Returns(_region);

            // no invocation expected
            var generalRegionAdapterMock = new Mock<IRegionAdapter>(MockBehavior.Strict);
            generalRegionAdapterMock
                .SetupGet(adapter => adapter.SupportedType)
                .Returns(typeof (DependencyObject));

            var regionFactory =
                new RegionFactory(new[]
                                      {
                                          generalRegionAdapterMock.Object,
                                          specificRegionAdapterMock.Object
                                      });
            var createdRegion = regionFactory.CreateRegion(_view);

            Assert.AreSame(_region, createdRegion);
        }


        [Test]
        public void chooses_an_adapter_supporting_supertype()
        {
            var generalRegionAdapterMock = new Mock<IRegionAdapter>();
            generalRegionAdapterMock
                .SetupGet(adapter => adapter.SupportedType)
                .Returns(typeof (DependencyObject));
            generalRegionAdapterMock
                .Setup(adapter => adapter.AdaptView(_view))
                .Returns(_region);

            var regionFactory = new RegionFactory(new[] {generalRegionAdapterMock.Object});
            var createdRegion = regionFactory.CreateRegion(_view);

            Assert.AreSame(_region, createdRegion);
        }
    }
}