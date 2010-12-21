using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Nomad.Modules.Manifest;
using Nomad.Utils;

namespace Nomad.RepositoryServer.Models.StorageProviders
{
    public class LinqToSqlStorageProvider : IStorageProvider
    {
        private readonly ModuleInfoDataContext _db = new ModuleInfoDataContext();

        public IEnumerable<IModuleInfo> GetAvaliableModules()
        {
            var list = _db.Modules.Select(x => new ModuleInfo()
                                                   {
                                                       Id = x.Id.ToString(),
                                                       Manifest = XmlSerializerHelper.Deserialize<ModuleManifest>(x.Manifest.ToArray()),
                                                       ModuleData = x.ModuleData.ToArray(),

                                                   }).ToList();

            return list.Cast<IModuleInfo>().ToList();
        }



        public void SaveModule(IModuleInfo moduleInfo)
        {   
            // get the max
            var maxes = _db.Modules.Select(x => x.Id).ToList();
            decimal max;
            if (maxes.Count == 0)
                max = 0;
            else
            {
                max = maxes.Max(x => x);
                max++;
            }
            

            var module = new Module()
                                {
                                    Id = max,
                                    Manifest = XmlSerializerHelper.Serialize(moduleInfo.Manifest),
                                    ModuleData = moduleInfo.ModuleData,
                                };

            _db.Modules.InsertOnSubmit(module);
            _db.SubmitChanges();
        }
        

        public void RemoveModule(IModuleInfo moduleInfo)
        {
            var module = _db.Modules
                .Where( x => x.Id.ToString().Equals(moduleInfo.Id))
                .Select(x => x)
                .Single();

            _db.Modules.DeleteOnSubmit(module);
            _db.SubmitChanges();
        }
    }

    
}