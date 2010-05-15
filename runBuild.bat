@powershell -command "import-module .\Libraries\PSake\psake.psm1; $psake.use_exit_on_error = $true; invoke-psake .\build.ps1"
@pause