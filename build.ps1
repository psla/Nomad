# Product information
properties {
	$version = "0.0.0.0"
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
	$source_dir = "$base_dir\Source"
	$release_dir = "$base_dir\Release"
	$documentation_dir = "$base_dir\Documentation"
	
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
	Remove-Item -Recurse -Force $documentation_dir -ErrorAction SilentlyContinue
}
 
task Init -depends Clean, GetProjects {
    New-Item $release_dir -ItemType directory | Out-Null
    New-Item $build_dir -ItemType directory | Out-Null
	New-Item $documentation_dir -ItemType directory | Out-Null
	New-Item $build_dir\Modules -ItemType directory | Out-Null
    
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

task Documentation -depends Compile, GetProjects -description "Provideds automated documentation" {
	
	#Prepare EVN Varibles ( We expect no Windows XP x64 or Win2003 x64 )
	if( (Get-WmiObject -computername $env:computername -class Win32_OperatingSystem ).OSArchitecture -eq "64-bit" )
	{
		echo "Setting 64 bit paths"
		
		#[Environment]::SetEnvironmentVariable("Path", $env:Path + ";C:\Program Files (x86)\Sandcastle\ProductionTools","Process")
		#[Environment]::SetEnvironmentVariable("Path", $env:Path + ";C:\Program Files (x86)\Sandcastle\ProductionTransforms","Process")
		#[Environment]::SetEnvironmentVariable("Path", $env:Path + ";C:\Program Files (x86)\HTML Help Workshop","Process")
		
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
	
	$documentation_command = "scbuild -BuildChm  -framework $framework_version -name '$documentation_dir/$product' -sources "
	
	echo "Getting list of projects with documentation: "
	
	#Get Projects builded by Nomad Team and provide them with documentation.
	if($script:projects)
	{
		foreach($project in $script:projects) 
		{
			#Skip the test projects  
			if( $($project.Name) -match ".*test.*" )
			{
				continue
			}
			
			$project_file = cat "$source_dir\$($project.Name)\$($project.Name).csproj"
			
			$regex = [regex]"<OutputType>([A-Za-z]+)</OutputType>"
			$value =  $regex.Matches($project_file).Item(0).Groups[1].Value
			
			if ( $value -eq "Library" )
			{
				$sufix = "dll"
			}
			elseif ( $value -eq "WinExe" )
			{	
				$sufix = "exe"
			}
			
			echo "$($project.Name).$sufix with $($project.Name).XML"
			
			$documentation_command += "'$build_dir/$($project.Name).$sufix', '$build_dir/$($project.Name).XML'"
			$documentation_command += ","
		}
	}
	
	#Remove the last element and save all verbose information from Sandcastle to log file.
	$documentation_command  = $documentation_command.Substring(0, $documentation_command.Length - 1 )
	#$documentation_command += " > $documentation_dir\Documentation.log"
	
	#Perform this task in big try ... catch block, beacuse of failure rate during documentation, should not stop the entire buid process
	echo "Begging generating documentation... " 
	
	try
	{
		Invoke-Expression -Command $documentation_command 
		#Exec { & $documentation_command }
		#& $documentation_command 
	}
	catch [Exception]
	{
		"Error !!! : $_" 
	}
}

task CompileSimplestModules -depends Compile {
	Push-Location $build_dir\Modules
	Exec { & csc.exe /out:SimplestModulePossible1.dll /target:library $source_dir\SimplestModulePossible.cs /reference:$build_dir/Nomad.dll /r:$build_dir/Nomad.Tests.dll}
	Exec { & csc.exe /out:SimplestModulePossible2.dll /target:library $source_dir\SimplestModulePossible.cs /reference:$build_dir/Nomad.dll /r:$build_dir/Nomad.Tests.dll}
	Pop-Location
}


task Release -depends UnitTest,CompileSimplestModules,Documentation {
	 
}