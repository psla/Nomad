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

# Unit tests information
properties {
    $unit_tests_category = "UnitTests"
}    

include ".\Libraries\PsakeExt\psake-ext.ps1"


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
}
 
task Init -depends Clean, GetProjects {
    New-Item $release_dir -ItemType directory | Out-Null
    New-Item $build_dir -ItemType directory | Out-Null
    
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
		$results_path = Join-Path $build_dir $results_file_name
        
        Write-Host "Executing unit tests from assembly $file_name"
        Exec { & $lib_dir\NUnit\nunit-console.exe /nologo /include:$unit_tests_category $test_assembly /xml=$results_path }
    }
}


task Release -depends UnitTest {
	 
}