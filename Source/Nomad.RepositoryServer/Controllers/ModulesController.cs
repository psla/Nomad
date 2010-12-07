using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nomad.RepositoryServer.Models;
using Nomad.Updater.ModuleRepositories.WebRepositories;
using Nomad.Utils;

namespace Nomad.RepositoryServer.Controllers
{
    public class ModulesController : Controller
    {
        private readonly RepositoryModel _repositoryModel = new RepositoryModel();

        //method bases on http://stackoverflow.com/questions/186062/can-an-asp-net-mvc-controller-return-an-image
        public FileResult ReturnAvailableModules()
        {
            //todo test it
            var repoList = _repositoryModel.ModuleInfosList;
            var packageList =
                repoList.Select(
                    repositoryModuleInfo =>
                    new WebModulePackageInfo(repositoryModuleInfo.Manifest, repositoryModuleInfo.Url))
                    .ToList();
            var webPackagesCollection =
                new WebAvailablePackagesCollection(
                    packageList);

            return File(XmlSerializerHelper.Serialize(webPackagesCollection),
                        "repo/packagesList");
        }


        public FileResult ReturnModulePackage(string urlId)
        {
            //todo test it
            return
                File(
                    _repositoryModel.ModuleInfosList.
                        Where(x => x.Url.Equals(urlId)).
                        Select(x => x.ModuleData).Single(), "repo/moduleZip"
                        );
        }

        public ActionResult ShowAvailableModules()
        {
            throw new NotImplementedException();
        }
    }
}