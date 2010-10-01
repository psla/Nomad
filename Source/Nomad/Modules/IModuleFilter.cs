namespace Nomad.Modules
{
    public interface IModuleFilter
    {
        bool Matches(ModuleInfo moduleInfo);
    }
}