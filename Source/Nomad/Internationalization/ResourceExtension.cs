using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Nomad.Internationalization
{
    ///<summary>
    /// Provides resource for wpf controls
    ///</summary>
    public class ResourceExtension : MarkupExtension
    {
        private readonly string _key;


        /// <summary>
        /// Initializes resource source for extension
        /// </summary>
        /// <param name="key"></param>
        public ResourceExtension(string key)
        {
            _key = key;
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding("Value")
                              {
                                  Source = new TranslationSource(_key)
                              };
            return binding.ProvideValue(serviceProvider);
        }
    }
}