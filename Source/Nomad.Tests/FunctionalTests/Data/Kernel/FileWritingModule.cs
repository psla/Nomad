using System;
using System.IO;
using Nomad.Modules;

public class FileWritingModule : IModuleBootstraper
{
    public void OnLoad()
    {
        FileInfo fileInfo = new FileInfo(@"Modules\Kernel\FileWritingModule\WrittenFile");
        StreamWriter text = fileInfo.CreateText();
        text.WriteLine("PermissionSet allows me to write files!");
        text.Close();
    }

    public void OnUnLoad()
    {

    }
}
