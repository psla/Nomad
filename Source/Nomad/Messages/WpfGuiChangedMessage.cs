using Nomad.Communication.EventAggregation;

namespace Nomad.Messages
{
    public class WpfGuiChangedMessage
    {
        public WpfGuiChangedMessage(IGuiThreadProvider guiThreadProvider)
        {
            NewGuiThreadProvider = guiThreadProvider;
        }


        public IGuiThreadProvider NewGuiThreadProvider { get; private set; }
    }
}