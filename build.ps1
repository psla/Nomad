# Product information
properties {
	$version = "0.0.0.0"
	$product = "Nomad"
    $company = "Miko³aj Dobski, Piotr Jessa, Maciej Kowalewski, Piotr Œlata³a"
    $copyright = "(C) 2010 Miko³aj Dobski, Piotr Jessa, Maciej Kowalewski, Piotr Œlata³a"
}

# Directories & files information
properties {
    $base_dir = Resolve-Path .
	$lib_dir = "$base_dir\Libraries"
	$build_dir = "$base_dir\Build"
	$source_dir = "$base_dir\Source"
	$release_dir = "$base_dir\Release"
	
	$sln_file = "$source_dir\$product.sln"
}

# Projects information
properties {
    $projects = @{ Name = "Nomad"; Description = "Main library of the Nomad framework" },
                @{ Name = "Nomad.Tests"; Description = "Tests for Nomad library" }
}

# Unit tests information
properties {
    $unit_tests_category = "UnitTests"
}    

include ".\Libraries\PsakeExt\psake-ext.ps1"


task default -depends Logo, Release

task GetVersion -description "Sets version property according to type of build (local or CI)" {
	#TODO: fetch version information from environment variables provided by Hudson CI
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
}
 
task Init -depends Clean {
    New-Item $release_dir -ItemType directory | Out-Null
    New-Item $build_dir -ItemType directory | Out-Null
    
    # generate assembly infos
    foreach($project in $projects) {
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
 
task Compile -depends Init {
    # execute msbuild - it is added to path by PSake
    Exec { msbuild $sln_file /nologo /verbosity:minimal }
}

task UnitTest -depends Compile {
    # find all assemblies that names end with ".Tests.dll"
    $test_assemblies = Get-Item "$build_dir\*.Tests.dll"
    
    if(!$test_assemblies) {
        Write-Warning "No test assemblies found"
        return;
    }
    
    # execute tests from each of those libraries
    foreach($test_assembly in $test_assemblies) {
        $file_name = $test_assembly.Name
        $results_file_name = $file_name.Replace(".Tests.dll", ".unittest.results.xml")
        
        Write-Host "Executing unit tests from assembly $file_name"
        Exec { & $lib_dir\NUnit\nunit-console.exe /nologo /include:$unit_tests_category $test_assembly /xml=$results_file_name }
    }
}


task Release -depends UnitTest {
	 
}