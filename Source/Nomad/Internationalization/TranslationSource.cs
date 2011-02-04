using System.ComponentModel;
using System.Windows.Markup;

namespace Nomad.Internationalization
{
    ///<summary>
    /// Contains data necessary for binding WPF <see cref="MarkupExtension"/> with <see cref="ResourceProvider"/>
    ///</summary>
    internal class TranslationSource : INotifyPropertyChanged
    {
        private readonly string _key;
        private readonly ResourceProvider _resourceProvider;


        public TranslationSource(string key)
        {
            _key = key;
            _resourceProvider = ResourceProvider.CurrentResourceProvider;
            _resourceProvider.CultureChanged += (x, y) => InvokeValueChanged();
        }


        public object Value
        {
            get { return _resourceProvider.Retrieve(_key); }
        }

        #region INotifyPropertyChanged Members

        ///<summary>
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void InvokeValueChanged()
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs("Value"));
        }
    }
}