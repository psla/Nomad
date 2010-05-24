using System.Collections.Generic;
using System.Collections.ObjectModel;
using WPFHostApp.DomainLogic;

namespace WPFHostApp
{
    public class CodeToTestList : ObservableCollection<ICodeToTest>
    {
       public CodeToTestList(List<ICodeToTest> list)
       {
           this.ClearItems();
           foreach (var codeToTest in list)
           {
               this.Items.Add(codeToTest);    
           }
       }
    }
}