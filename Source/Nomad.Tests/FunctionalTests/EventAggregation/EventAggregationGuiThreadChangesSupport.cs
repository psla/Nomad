using System;
using Moq;
using Nomad.Communication.EventAggregation;
using Nomad.Messages;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.EventAggregation
{
    [IntegrationTests]
    public class EventAggregationGuiThreadChangesSupport
    {
        private EventAggregator _eventAggregator;


        [SetUp]
        public void Setup()
        {
            _eventAggregator = new EventAggregator(new NullGuiThreadProvider());
        }


        [Test]
        public void wpf_gui_changed_supported_by_eventaggregator()
        {
            var guiThreadProvider = new Mock<IGuiThreadProvider>();
            var guiChanged = new WpfGuiChangedMessage(guiThreadProvider.Object);

            _eventAggregator.Publish(guiChanged);

            _eventAggregator.Subscribe<NullMessage>((x) => { }, DeliveryMethod.GuiThread);
            _eventAggregator.Publish(new NullMessage());

            // verify that publish executed in gui thread
            guiThreadProvider.Verify(x => x.RunInGui(It.IsAny<Action>()), Times.Exactly(1),
                                     "After changing type of IGuiThreadProvider, gui should be executed in new gui thread");
        }


        [Test]
        public void cannot_set_wpf_gui_thread_twice()
        {
            var guiThreadProvider = new Mock<IGuiThreadProvider>();
            var guiChanged = new WpfGuiChangedMessage(guiThreadProvider.Object);

            _eventAggregator.Publish(guiChanged);
            Assert.Throws<InvalidOperationException>(() => _eventAggregator.Publish(guiChanged),
                                                     "Cannot set wpf gui thread twice!");
        }
    }

    public class NullMessage
    {
    }
}