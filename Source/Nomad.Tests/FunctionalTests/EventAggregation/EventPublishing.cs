using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using Nomad.Communication.EventAggregation;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.FunctionalTests.EventAggregation
{
    [UnitTests]
    public class EventPublishing
    {
        private const string NameToSend = "testname";

        private IEventAggregator _eventAggregator;
        private Mock<IGuiThreadProvider> _guiThread;


        [SetUp]
        public void Setup()
        {
            _guiThread = new Mock<IGuiThreadProvider>();
            _eventAggregator = new EventAggregator(_guiThread.Object);
        }


        [Test]
        public void payload_object_is_passed_to_the_listener()
        {
            var sentPayload = new MessageType(NameToSend);
            MessageType receivedPayload = null;

            _eventAggregator.Subscribe<MessageType>(payload => receivedPayload = payload);
            _eventAggregator.Publish(sentPayload);

            Assert.AreSame(sentPayload, receivedPayload,
                           "Event handler did not receive expected payload object or was not invoked at all");
        }


        [Test]
        public void notify_ignores_event_when_no_listeners()
        {
            Assert.DoesNotThrow(() => _eventAggregator.Publish(new MessageType(NameToSend)),
                                "Publish should not throw an exception when no listeners");
        }


        [Test]
        public void all_listeners_for_type_invoked()
        {
            MessageType firstListener = null;
            MessageType secondListener = null;
            var payload = new MessageType(NameToSend);
            _eventAggregator.Subscribe<MessageType>(x => { firstListener = x; });
            _eventAggregator.Subscribe<MessageType>(x => { secondListener = x; });
            _eventAggregator.Publish(payload);
            Assert.AreSame(payload, firstListener, "First event was not successfuly invoked");
            Assert.AreSame(payload, secondListener, "Second event was not successfuly invoked");
        }


        [Test]
        public void basic_thread_safety()
        {
            var threadTestClass = new ThreadingTestHelper(_eventAggregator,NameToSend);

            for (int i = 0; i < ThreadingTestHelper.ListenerThreadsNumber; i++)
            {
                threadTestClass.CreateListenerThread(i);
            }

            threadTestClass.CreatePublisherThread();

            threadTestClass.StartAllThreadsAndWaitForTheEndOfThem();

            //wait for the end of all listeners, one second for each. Otherwise break the test
            for (int i = 0; i < ThreadingTestHelper.ListenerThreadsNumber; i++)
            {
                Assert.IsTrue(threadTestClass.EndSynchronization[i].WaitOne(1000),
                              "All listeners did not finish in specified amount of time");
                threadTestClass.Invoked[i] = false;
            }
            if (threadTestClass.Exception != null)
                throw threadTestClass.Exception;

            //verify, if all listeners subscribed successfuly
            //invoke
            _eventAggregator.Publish(new MessageType(NameToSend));
            //verify response
            for (int i = 0; i < 100; i++)
                Assert.IsTrue(threadTestClass.Invoked[i],
                              string.Format("Subscribed event {0} was not invoked", i));
        }


        [Test]
        public void after_calling_unsubscribe_handler_is_not_called_when_event_is_published()
        {
            Action<MessageType> myAction = x => { Assert.Fail("Delegate was not removed"); };
            IEventAggregatorTicket<MessageType> ticket = _eventAggregator.Subscribe(myAction);
            ticket.Dispose();
            _eventAggregator.Publish(new MessageType(NameToSend));
        }


        [Test]
        public void after_unsubscribing_subscribing_again_results_in_working_event()
        {
            Action<MessageType> myAction = x => { Assert.Fail("Delegate was not removed"); };
            IEventAggregatorTicket<MessageType> ticket = _eventAggregator.Subscribe(myAction);
            ticket.Dispose();
            _eventAggregator.Subscribe<MessageType>(x => { });
            Assert.DoesNotThrow(() => _eventAggregator.Publish(new MessageType(NameToSend)),
                                "After subscribing and unsubscribing an event we can no longer use event of this type");
        }


        [Test]
        public void when_one_handler_unsubscribes_others_are_still_called()
        {
            var sentPayload = new MessageType(NameToSend);
            MessageType receivedPayload = null;
            _eventAggregator.Subscribe<MessageType>(payload => receivedPayload = payload);

            Action<MessageType> myAction = x => { };
            IEventAggregatorTicket<MessageType> ticket = _eventAggregator.Subscribe(myAction);
            ticket.Dispose();

            _eventAggregator.Publish(sentPayload);
            Assert.AreSame(sentPayload, receivedPayload);
        }


        [Test]
        public void subscribing_for_action_in_gui_is_called_in_gui()
        {
            var sentPayload = new MessageType(NameToSend);
            Action<MessageType> @delegate = (x) => { };
            _eventAggregator.Subscribe(@delegate, DeliveryMethod.GuiThread);

            _eventAggregator.Publish(sentPayload);
            _guiThread.Verify(
                x => x.RunInGui(It.IsAny<Action>()),
                Times.Exactly(1));
        }


        [Test]
        public void subscribing_for_action_in_any_thread_is_not_invoked_in_gui()
        {
            var sentPayload = new MessageType(NameToSend);
            Action<MessageType> @delegate = (x) => { };
            _eventAggregator.Subscribe(@delegate, DeliveryMethod.AnyThread);

            _eventAggregator.Publish(sentPayload);
            _guiThread.Verify(
                x => x.RunInGui(It.Is<Action>(y => y.GetInvocationList().Contains(@delegate))),
                Times.Exactly(0));
        }


        [Test]
        public void having_action_in_gui_and_not_in_gui_results_in_both_properly_invoked()
        {
            var sentPayload = new MessageType(NameToSend);
            MessageType anyThreadPayload = null;
            _eventAggregator.Subscribe<MessageType>((payload) => anyThreadPayload = payload,
                                                    DeliveryMethod.AnyThread);
            Action<MessageType> guiDelegate = payload => { };
            _eventAggregator.Subscribe(guiDelegate, DeliveryMethod.GuiThread);

            _eventAggregator.Publish(sentPayload);

            Assert.AreSame(anyThreadPayload, sentPayload);
            _guiThread.Verify(
                x => x.RunInGui(It.IsAny<Action>()),
                Times.Exactly(1));
        }
    }
}