using System.Collections.Generic;

namespace Nomad.Signing
{
    public class AssemblyVerificator
    {
        private IDictionary<string, IssuerInformation> _issuers = new Dictionary<string, IssuerInformation>();
        public void AddTrustedIssuer(IssuerInformation issuerInformation)
        {
            _issuers.Add(issuerInformation.IssuerName, issuerInformation);
        }


        public bool VerifyFile(File file, FileSignature signature)
        {
            IssuerInformation information = null;
            if(_issuers.TryGetValue(signature.IssuerName, out information))
            {
                return information.IssuerAlgorithm.Verify(file.Data, signature.Signature)
                       && file.FileName == signature.FileName;
            }
            else
            {
                return false;
            }
        }
    }
}