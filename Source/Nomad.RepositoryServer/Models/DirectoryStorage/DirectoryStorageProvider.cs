using System;
using System.Collections.Generic;
using System.IO;

namespace Nomad.RepositoryServer.Models.DirectoryStorage
{
    /// <summary>
    ///     Stores modules in directory <see cref="_pathToDirectory"/>. Acts like a storage.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Due to the fact that data are stored as files on server, access to them might be slow.
    /// This is the simplest possible way of implementing the <see cref="IStorageProvider"/>.
    /// </para>
    /// <para>
    ///     TODO: Describe the data tree here
    /// </para>
    /// </remarks>
    public class DirectoryStorageProvider : IStorageProvider
    {
        private readonly string _pathToDirectory;


        public DirectoryStorageProvider(string pathToDirectory)
        {
            _pathToDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,pathToDirectory);

            // get access to directory, check if exists and so on.
            if (!Directory.Exists(_pathToDirectory))
                Directory.CreateDirectory(_pathToDirectory);

            
        }


        public List<IModuleInfo> GetAvaliableModules()
        {
            throw new NotImplementedException();
        }


        public bool SaveModule(IModuleInfo moduleInfo)
        {
            throw new NotImplementedException();
        }


        public bool RemoveModule(IModuleInfo moduleInfo)
        {
            throw new NotImplementedException();
        }
    }
}