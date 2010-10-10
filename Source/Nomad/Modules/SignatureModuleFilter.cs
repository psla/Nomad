using System;
using Nomad.Signing;

namespace Nomad.Modules
{
    public class SignatureModuleFilter : IModuleFilter
    {
        private readonly ISignatureProvider _signatureProvider;


        public SignatureModuleFilter(ISignatureProvider signatureProvider)
        {
            _signatureProvider = signatureProvider;
        }


        public bool Matches(ModuleInfo moduleInfo)
        {
            return true;
        }
    }
}