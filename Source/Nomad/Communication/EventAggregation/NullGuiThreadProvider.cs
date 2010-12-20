using System;

namespace Nomad.Communication.EventAggregation
{
    /// <summary>
    /// Provides empty gui thread (executes in the same thread as source thread)
    /// </summary>
    public class NullGuiThreadProvider : IGuiThreadProvider
    {
        #region IGuiThreadProvider Members

        public void RunInGui(Delegate @delegate)
        {
            @delegate.DynamicInvoke(null);
        }

        #endregion
    }
}