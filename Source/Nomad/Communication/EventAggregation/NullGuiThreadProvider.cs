using System;

namespace Nomad.Communication.EventAggregation
{
    /// <summary>
    /// Provides empty gui thread (executes in the same thread as source thread)
    /// </summary>
    public class NullGuiThreadProvider : IGuiThreadProvider
    {
        private readonly object _serialEnsurance = new object();
        #region IGuiThreadProvider Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public void RunInGui(Action action)
        {
            lock (_serialEnsurance)
                action();
        }

        #endregion
    }
}