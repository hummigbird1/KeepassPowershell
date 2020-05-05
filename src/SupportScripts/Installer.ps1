function Deploy-KeepassCmdletModule
{
    param(
        [Switch]$System,
        [Switch]$WhatIf
    )

    $moduleName = "KeepassPSCmdlets"
    $installPath = "$home\Documents\WindowsPowerShell\Modules"
    if($System)
    {
        $installPath = "$($Env:ProgramFiles)\WindowsPowerShell\Modules"
    }

    <# Validate that the determined target is actually configured as PSModulePath otherwise it wouldn't be automagically usable after installation #>
    $validModulePaths = $env:PSMODULEPATH.Split(';')
    if(($validModulePaths -contains $installPath) -eq $false)
    {
        throw "The determined installation path of $installPath seems not to be a valid Powershell module path. Unfortunately you will have to install manually." 
    }
    
    $sourceFolder = $PSScriptRoot
    $targetFolder = "$installPath\$moduleName"
    Write-Output "Installing Module to: $targetFolder"
    if((Test-Path -Path $targetFolder -PathType Container) -eq $false)
    {
        $null = New-Item -Path $targetFolder -ItemType Directory -WhatIf:$WhatIf
    }
    try
    {
        Expand-Archive -DestinationPath $targetFolder -Path "$sourceFolder\$moduleName.zip" -Force -WhatIf:$WhatIf
        if($WhatIf -eq $false)
        {
            Write-Host -ForegroundColor Green "The module was successfully installed to '$targetFolder'. The cmdlets should be available now."
        }
        else
        {
            Write-Warning "The module was NOT installed yet as this was just a What-If."
        }
    }
    catch
    {
        throw;
    }
    
}

function Install-KeepassCmdletModule
{
    param(
        [Switch]$WhatIf
    )

    $selection = Read-Host -Prompt "Install for all users? [Y]es / [N]o / [A]bort installation: (Enter = N)"
    $systemInstall = $false;
    switch($selection)
    {
        {($_ -eq "y") -or ($_ -eq "Y")} { $systemInstall = $true; }
        {($_ -eq "a") -or ($_ -eq "A")} { return; }

    }

    Deploy-KeepassCmdletModule -System:$systemInstall -WhatIf:$WhatIf
}

Install-KeepassCmdletModule