using System.Windows.Automation;
using White.Core.UIItems.Actions;
using White.Core.UIItems.Custom;

namespace Nomad.Tests.FunctionalTests.Regions
{
    [ControlTypeMapping(CustomUIItemType.StatusBar)]
    public class StatusBarUIItem : CustomUIItem
    {
        // Implement these two constructors. The order of parameters should be same.
        public StatusBarUIItem(AutomationElement automationElement, ActionListener actionListener)
            : base(automationElement, actionListener)
        {
        }


        //Empty constructor is mandatory with protected or public access modifier.
        protected StatusBarUIItem()
        {
        }
    }
}