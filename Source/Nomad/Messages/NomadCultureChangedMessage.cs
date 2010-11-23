using System.Globalization;
using System.Text;

namespace Nomad.Messages
{
    ///<summary>
    /// Message fo UICultureChanged Nomad service.
    ///</summary>
    public class NomadCultureChangedMessage : NomadMessage
    {
        ///<summary>
        /// Initiales an instance of <see cref="NomadCultureChangedMessage"/>.
        ///</summary>
        ///<param name="cultureInfo">Carried <see cref="CultureInfo"/> important for all subscribers.</param>
        ///<param name="message">Optional message.</param>
        public NomadCultureChangedMessage(CultureInfo cultureInfo, string message) : base(message)
        {
            CurrentCulture = cultureInfo;
        }


        ///<summary>
        /// <see cref="CultureInfo"/> that all subscribed members should change to.
        ///</summary>
        public CultureInfo CurrentCulture { get; private set; }


        /// <summary>
        ///     Inherited <see cref="object.ToString"/> method which every message has to implement.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(new[]
                          {
                              "Culture changed to: ",
                              CurrentCulture.Name,
                              " OptionalMessage: ",
                              Message
                          });
            return sb.ToString();
        }
    }
}