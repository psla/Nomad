using System;
using System.Windows;
using Nomad.Regions;
using NUnit.Framework;

namespace Nomad.Tests.UnitTests.Regions.Adapters
{
    [TestFixture]
    public abstract class AdapterContractBase<T> where T : IRegionAdapter
    {
        protected T Adapter;


        [SetUp]
        public void SetUp()
        {
            Adapter = GetAdapter();
        }


        protected abstract T GetAdapter();


        protected virtual DependencyObject GetUnsupportedView()
        {
            return new DependencyObject();
        }


        [Test]
        public void should_throw_on_null_view()
        {
            Assert.Throws<ArgumentNullException>(() => Adapter.AdaptView(null));
        }


        [Test]
        public void should_throw_on_unsupported_view_type()
        {
            Assert.Throws<InvalidOperationException>(
                () => Adapter.AdaptView(GetUnsupportedView())
                );
        }


        [Test]
        public void get_type_should_not_or_throw_nor_return_null()
        {
            Type type = null;
            Assert.DoesNotThrow(() => type = Adapter.SupportedType);
            Assert.IsNotNull(type);
        }
    }
}