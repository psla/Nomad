# Product information
properties {
	$version = $Env:BUILDVERSION
	$framework_version = "3.5"
	$product = "Nomad"
    $company = "Miko³aj Dobski, Piotr Jessa, Maciej Kowalewski, Piotr Œlata³a"
    $copyright = "(C) 2010 Miko³aj Dobski, Piotr Jessa, Maciej Kowalewski, Piotr Œlata³a"
}

# Directories & files information
properties {
    $base_dir = Resolve-Path .
	$lib_dir = "$base_dir\Libraries"
	$build_dir = "$base_dir\Build"
	$xmldoc_dir = "$base_dir\XmlDocumentation"
	$source_dir = "$base_dir\Source"
	$release_dir = "$base_dir\Release"
	$documentation_dir = "$base_dir\Release"
	
	$sln_file = "$source_dir\$product.sln"
}

# Unit tests information
properties {
    $unit_tests_category = "UnitTests"
	$functional_tests_category = "FunctionalTests"
}    

#Directories for handling data for various types of tests
properties {
	$functional_data_dir="$source_dir\$product.Tests\$functional_tests_category\Data"

}

include ".\Libraries\PsakeExt\psake-ext.ps1"

function tests([string] $tests_category)
{
	# find all assemblies that names end with ".Tests.dll"
    $test_assemblies = Get-Item "$build_dir\*.Tests.dll"
    
    if(!$test_assemblies) {
        Write-Warning "No test assemblies found"
        return;
    }
    
    # execute tests from each of those libraries
    foreach($test_assembly in $test_assemblies) {
        $file_name = $test_assembly.Name
        $results_file_name = $file_name.Replace(".Tests.dll", ".$tests_category.results.xml")
		$results_path = Join-Path $build_dir $results_file_name
        
        Write-Host "Executing unit tests from assembly $file_name"
        Exec { & $lib_dir\NUnit\nunit-console.exe /nologo /include:$tests_category $test_assembly /xml=$results_path }
    }
}

task default -depends Logo, Release

task GetVersion -description "Sets version property according to type of build (local or CI)" {
	#TODO: fetch version information from environment variables provided by Hudson CI
}

task GetProjects -description "Identifies all projects in product" {
    [array] $script:projects = @()
    $project_files = Get-ChildItem -Filter "*.csproj" -Path $source_dir -Recurse
    
    foreach($project_file in $project_files) {
        $project_name = $project_file.Name.Substring(0, $project_file.Name.Length - ".csproj".Length)

        # validate that project's name matches it's location
        $expected_directory = "$source_dir\$project_name"
        Assert ($expected_directory -eq $project_file.DirectoryName) "Project name doesn't match directory name: $($project_file.FullName)"
        
        # try to get project's description
        $description = ""
        $readme_file = $project_file.DirectoryName + "\readme.txt"
        if(Test-Path $readme_file) {
            $description = Get-Content $readme_file -TotalCount 1
        }
        
        $project = New-Object PSObject -Property @{Name = $project_name; Description = $description}
        $script:projects = $script:projects + $project
    }
}

task ListProjects -depends GetProjects {
    $script:projects | Select-Object Name, Description | Format-Table
}

task Logo -depends GetVersion -description "Displays build header" -action {
	"----------------------------------------------------------------------"
	"Building $product version $version"
	"----------------------------------------------------------------------"
	Write-Output "Base dir is: $base_dir"
}

task Clean {
    Remove-Item -Recurse -Force $release_dir -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force $build_dir -ErrorAction SilentlyContinue
	Remove-Item -Recurse -Force $documentation_dir -ErrorAction SilentlyContinue
	Remove-Item -Recurse -Force $xmldoc_dir -ErrorAction SilentlyContinue
}
 
task Init -depends Clean, GetProjects {
    New-Item $release_dir -ItemType directory | Out-Null
    New-Item $release_dir\Nomad -ItemType directory | Out-Null
    New-Item $release_dir\Nomad\bin -ItemType directory | Out-Null
    New-Item $release_dir\Nomad\doc -ItemType directory | Out-Null
    New-Item $release_dir\Examples -ItemType directory | Out-Null
    New-Item $release_dir\Examples\doc -ItemType directory | Out-Null
    New-Item $build_dir -ItemType directory | Out-Null
	#New-Item $documentation_dir -ItemType directory | Out-Null
	New-Item $build_dir\Modules -ItemType directory | Out-Null
	New-Item $xmldoc_dir -ItemType directory | Out-Null
	New-Item $xmldoc_dir\Examples -ItemType directory | Out-Null
	New-Item $xmldoc_dir\Nomad -ItemType directory | Out-Null
    
    # generate assembly infos
	if($script:projects) {
		foreach($project in $script:projects) {
			$file = "$source_dir\$($project.Name)\Properties\AssemblyInfo.cs"
			Generate-AssemblyInfo -file $file `
				-title $project.Name `
				-description $project.Description `
				-product $product `
				-version $version `
				-company $company `
				-copyright $copyright
		}
	}
}
 
task Compile -depends Init {
    # execute msbuild - it is added to path by PSake
    Exec { msbuild "$sln_file"  /p:Configuration=Release /nologo /verbosity:quiet }
}

task UnitTest -depends Compile, FunctionalDataPrepare {
    tests $unit_tests_category
}

task FunctionalTest -depends Compile,FunctionalDataPrepare {
	tests $functional_tests_category
}

task Documentation -depends Compile, GetProjects -description "Provideds automated documentation" {
	
	#Prepare EVN Varibles ( We expect no Windows XP x64 or Win2003 x64 )
	if( (Get-WmiObject -computername $env:computername -class Win32_OperatingSystem ).OSArchitecture -eq "64-bit" )
	{
		echo "Setting 64 bit paths"
		
		$env:DxRoot = "C:\Program Files (x86)\Sandcastle"
		$env:path = $env:path + ";C:\Program Files (x86)\Sandcastle\ProductionTools"
		$env:path = $env:path + ";C:\Program Files (x86)\HTML Help Workshop"
		
	}
	else
	{
		echo "Setting 32 bit paths"
		$env:DxRoot = "C:\Program Files\Sandcastle"
		$env:path = $env:path + ";C:\Program Files\Sandcastle\ProductionTools"
		$env:path = $env:path + ";C:\Program Files\HTML Help Workshop"
	}
	
	$documentation_command1 = "scbuild -BuildChm  -framework $framework_version -name '$release_dir\$product\doc\Nomad' -sources "
	$documentation_command2 = "scbuild -BuildChm  -framework $framework_version -name '$release_dir\Examples\doc' -sources "
	
	echo "Getting list of projects with documentation: "
	
	#Get Projects builded by Nomad Team and provide them with documentation.

	$nomad_files = Get-ChildItem $xmldoc_dir\Nomad\
	$other_files = Get-ChildItem $xmldoc_dir\Examples\ 
	foreach($file in $nomad_files)
	{
		$file_bn = $file.BaseName
		$binary_file = ls "$release_dir\Nomad\bin\$file_bn.*" | where {$_ -match '.(dll|exe)$'}
		
		$binary_fn = $binary_file.FullName
		$file_fn = $file.FullName
		$documentation_command1 += "'$binary_fn', '$file_fn',"
	}
	
	foreach($file in $other_files)
	{
		$file_bn = $file.BaseName
		$binary_file = Get-ChildItem "$release_dir\Examples\" -r | where {$_ -match '^$file_bn.(dll|exe)$'}
		$binary_fn = $binary_file.FullName
		$file_fn = $file.FullName
		$documentation_command2 += "'$binary_fn', '$file_fn',"
	}
	
	#Remove the last element and save all verbose information from Sandcastle to log file.
	$documentation_command1  = $documentation_command1.Substring(0, $documentation_command1.Length - 1 )
	$documentation_command2  = $documentation_command2.Substring(0, $documentation_command2.Length - 1 )
	#$documentation_command += " > $documentation_dir\Documentation.log"
	
	#Perform this task in big try ... catch block, beacuse of failure rate during documentation, should not stop the entire buid process
	echo "Begging generating documentation... " 
	
	try
	{
		Write-Host $documentation_command1
		Invoke-Expression -Command $documentation_command1
		#Exec { & $documentation_command }
		#& $documentation_command 
	}
	catch [Exception]
	{
		"Error while documenting Nomad!!! : $_" 
	}
	
	try 
	{
		Write-Host $documentation_command2
		Invoke-Expression -Command $documentation_command2
	}
	catch [Exception]
	{
		"Error while documenting examples!!! : $_" 
	}
}

task FunctionalDataPrepare -depends Compile -description "Data preparations for functional tests" {
	New-Item $build_dir\Modules\Simple -ItemType directory | Out-Null
	New-Item $build_dir\Modules\WithDependencies -ItemType directory | Out-Null
	New-Item $build_dir\Modules\ServiceLocatorEnabled -ItemType directory | Out-Null
	
	Push-Location $build_dir\Modules\Simple
	Exec { & csc.exe /out:SimplestModulePossible1.dll /target:library $functional_data_dir\SimplestModulePossible.cs /reference:$build_dir/Nomad.dll /r:$build_dir/Nomad.Tests.dll}
	Exec { & csc.exe /out:SimplestModulePossible2.dll /target:library $functional_data_dir\SimplestModulePossible.cs /reference:$build_dir/Nomad.dll /r:$build_dir/Nomad.Tests.dll}
	Pop-Location
	
	Push-Location $build_dir\Modules\WithDependencies
	Exec { & csc.exe /out:ModuleWithConstructorDependency.dll /target:library $functional_data_dir\ModuleWithConstructorDependency.cs /reference:$build_dir/Nomad.dll /r:$build_dir/Nomad.Tests.dll}
	Exec { & csc.exe /out:ModuleWithPropertyDependency.dll /target:library $functional_data_dir\ModuleWithPropertyDependency.cs /reference:$build_dir/Nomad.dll /r:$build_dir/Nomad.Tests.dll}
	Pop-Location
	
	Push-Location $build_dir\Modules\ServiceLocatorEnabled
	Exec { & csc.exe /out:ResolvingServiceModule.dll /t:library $functional_data_dir\ResolvingServiceModule.cs /r:$build_dir/Nomad.dll /r:$build_dir/Nomad.Tests.dll}
	Exec { & csc.exe /out:RegistringServiceModule.dll /t:library $functional_data_dir\RegistringServiceModule.cs /r:$build_dir/Nomad.dll /r:$build_dir/Nomad.Tests.dll}
	Pop-Location
}

task LocalBuild -depends UnitTest,FunctionalDataPrepare,Compile -description "Local build without building documentation"{

}

task Release -depends UnitTest,FunctionalDataPrepare,Documentation -description "Fully fledgged build with everything in it" {
	 
}

task FastBuild -depends UnitTest {

}

task SlowBuild -depends FastBuild, FunctionalTest, Documentation, Deploy {

}

task Deploy -depends Compile {
	#$result_file_name = "Nomad-$version.zip"
	$result_file_name = "..\${product}.zip"
	if(Test-Path $result_file_name -eq ) {
		Remove-Item $result_file_name
	}
	Push-Location $release_dir
	Exec { & ..\Libraries\7za465\7za.exe a ${result_file_name} . }
	Pop-Location
}
