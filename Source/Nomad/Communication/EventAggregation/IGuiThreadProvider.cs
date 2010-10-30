using System;

namespace Nomad.Communication.EventAggregation
{
    ///<summary>
    /// Provides gui thread based on gui queue implementation
    ///</summary>
    public interface IGuiThreadProvider
    {
        ///<summary>
        /// Runs provided delegate in proper gui thread
        ///</summary>
        ///<param name="delegate">delegate to run</param>
        void RunInGui(Delegate @delegate);
    }
}