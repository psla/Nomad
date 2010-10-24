using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Nomad.Modules.Manifest;

namespace Nomad.Updater
{
    // NOTE: If you change the interface name "IModulesRepository" here, you must also update the reference to "IModulesRepository" in Web.config.
    /// <summary>
    /// <see cref="ServiceContractAttribute"/> for repository.
    /// </summary>
    /// <remarks>
    ///     
    /// </remarks>
    [ServiceContract]
    public interface IModulesRepository
    {
        /// <summary>
        ///     Gets all available modules.
        /// </summary>
        /// <returns>Complex class that represents the <see cref="List{T}"/> of <see cref="ModuleManifest"/> </returns>
        [OperationContract]
        AvailableModules GetAvailableModules();

        /// <summary>
        ///     Gets the single <see cref="ModulePackage"/> from server identified by module unique name.
        /// </summary>
        /// <param name="moduleUniqeName">Unique name of the module which will be got.</param>
        /// <returns>Module Package that contains the specified module</returns>
        [OperationContract]
        ModulePackage GetModule(string moduleUniqeName);
        // TODO: Add your service operations here

        /// <summary>
        /// Add modules to repository. 
        /// </summary>
        /// <param name="module">Unique module name of the new module.</param>
        /// <returns>True if addition was succesful.</returns>
        [OperationContract]
        bool AddModule(ModulePackage module);
    }

    /// <summary>
    /// Complex class that represents available modules list. 
    /// </summary>
    /// <remarks>
    ///     Class is defined this way because of the <see cref="DataContract"/> constraints
    /// </remarks>
    [DataContract]
    public class AvailableModules
    {
        /// <summary>
        /// 
        /// </summary>
        public AvailableModules()
        {
            
        }

        /// <summary>
        ///     Initializes the instance of <see cref="AvailableModules"/> class.
        /// </summary>
        /// <param name="manifests">List of <see cref="ModuleManifest"/> </param>
        public AvailableModules(List<ModuleManifest> manifests)
        {
            Manifests = manifests;
        }


        /// <summary>
        /// List of available modules within repository.
        /// </summary>
        [DataMember]
        public List<ModuleManifest> Manifests { get; set; }
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
