using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Nomad.Modules.Manifest;

namespace Nomad.ModulesRepository.Data
{
    // NOTE: If you change the interface name "IModulesRepository" here, you must also update the reference to "IModulesRepository" in Web.config.
    [ServiceContract]
    public interface IModulesRepository
    {
        [OperationContract]
        AvailableModules GetAvailableModules();

        [OperationContract]
        ModulePackage GetModule(string module);
        // TODO: Add your service operations here

        [OperationContract]
        bool AddModule(ModulePackage module);
    }

    [DataContract]
    public class AvailableModules
    {
        [DataMember]
        public List<ModuleManifest> Manifests { get; set; }
    }

    public class ModuleManifestDto
    {
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
