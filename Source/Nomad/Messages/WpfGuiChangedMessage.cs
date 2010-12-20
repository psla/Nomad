using System;
using Nomad.Communication.EventAggregation;

namespace Nomad.Messages
{
    /// <summary>
    /// Informs other modules that wpf thread is set
    /// </summary>
    /// <remarks>
    /// May be published only once per appdomain
    /// </remarks>
    public class WpfGuiChangedMessage
    {
        /// <summary>
        /// Provides <see cref="guiThreadProvider"/>
        /// </summary>
        /// <param name="guiThreadProvider">gui thread</param>
        public WpfGuiChangedMessage(IGuiThreadProvider guiThreadProvider)
        {
            NewGuiThreadProvider = guiThreadProvider;
        }

        /// <summary>
        /// Gui thread which should be used in specified <see cref="AppDomain"/>
        /// </summary>
        public IGuiThreadProvider NewGuiThreadProvider { get; private set; }
    }
}