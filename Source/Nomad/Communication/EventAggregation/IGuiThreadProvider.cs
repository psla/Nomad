using System;

namespace Nomad.Communication.EventAggregation
{
    ///<summary>
    /// Provides gui thread based on gui queue implementation
    ///</summary>
    /// <remarks>
    /// <see cref="IGuiThreadProvider"/> ensures that execution is performed without parallelization (tasks are executed one after another)
    /// </remarks>
    public interface IGuiThreadProvider
    {
        ///<summary>
        /// Runs provided delegate in proper gui thread
        ///</summary>
        ///<param name="action">action to run in gui thread</param>
        void RunInGui(Action action);
    }
}