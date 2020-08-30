<#
.SYNOPSIS
    Build, verify and install NLedger from source code on any OS.
.DESCRIPTION
    This script helps people to get workable NLedger from source code
    on their computer. It performs four basic steps: a) build binaries;
    b) run unit tests; c) run integration tests; d) install created binaries
    (add to PATH and create 'ledger' link). In case of any errors on any steps,
    the script provides troubleshooting information to help people solve environmental
    issues. Switches can regulate this process (omit some steps as well as uninstall 
    previously installed binaries).
.PARAMETER coreOnly
    Orders to create .Net Core binaries only. By default, the script creates both Framework
    and Core binaries. This switch is selected automatically on non-Windows platforms.
.PARAMETER debugMode
    Orders to create binaries in Debug mode (Release by default)
.PARAMETER noUnitTests
    Omits running xUnit tests
.PARAMETER noNLTests
    Omits running NLTest integration tests
.PARAMETER noInstall
    Do not install created binaries. By default, the script adds the folder with binaries
    to PATH and creates a hard link 'ledger'. For .Net Framework binaries, it also
    triggers NGen run.
.PARAMETER uninstall
    If this switch is selected, the script removes binaries with PATH and deletes a hard link.
    Any other switches are ignored.
.EXAMPLE
    PS> ./get-nledger-up.ps1
    Build, verify and install NLedger from source code.
.EXAMPLE
    PS> ./get-nledger-up.ps1 -Verbose
    Show detail diagnostic information to troubleshoot build issues.
.EXAMPLE
    PS> ./get-nledger-up.ps1 -coreOnly
    Create .Net Core binaries only.
.EXAMPLE
    PS> ./get-nledger-up.ps1 -uninstall
    Uninstall NLedger if it was previously installed
.NOTES
    Author: Dmitry Merzlyakov
    Date:   August 27, 2020
    ===
    Run the script on Windows: >powershell -ExecutionPolicy RemoteSigned -File ./get-nledger-up.ps1
    Run the script on other OS: >powershell -File ./get-nledger-up.ps1
#>
[CmdletBinding()]
Param(
    [Switch][bool]$coreOnly = $False,
    [Switch][bool]$debugMode = $False,
    [Switch][bool]$noUnitTests = $False,
    [Switch][bool]$noNLTests = $False,
    [Switch][bool]$noInstall = $False,
    [Switch][bool]$uninstall = $False
)

trap 
{ 
  write-error $_ 
  exit 1 
} 

[string]$Script:ScriptPath = Split-Path $MyInvocation.MyCommand.Path
[Version]$minDotnetVersion = "3.1"

Write-Progress -Activity "Building, testing and installing NLedger" -Status "Initialization"

# Check environmental information

[bool]$isWindowsPlatform = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)
Write-Verbose "Detected: is windows platform=$isWindowsPlatform"

[bool]$isDotNetInstalled = -not(-not($(get-command dotnet -ErrorAction SilentlyContinue)))
if (!$isDotNetInstalled) { throw "DotNet Core is not installed (command 'dotnet' is not available)" }

[Version]$dotnetVersion = $(dotnet --version)
Write-Verbose "Detected: dotnet version=$dotnetVersion"
if ($dotnetVersion -lt $minDotnetVersion) { throw "Detected dotnet version is $dotnetVersion but minimal required is $minDotnetVersion" }

# Validate switchers

if (!$isWindowsPlatform -and !$coreOnly) {
    $coreOnly = $true
    Write-Verbose "Since it is not windows platform, switch 'coreOnly' is changed to 'True'."
}

# Check codebase structure

[string]$solutionPath = [System.IO.Path]::GetFullPath("$Script:ScriptPath/Source/NLedger.sln")
Write-Verbose "Expected solution path: $solutionPath"
if (!(Test-Path -LiteralPath $solutionPath -PathType Leaf)) { "File '$solutionPath' does not exist. Check that source code base is in valid state." }

[string]$nlTestPath = [System.IO.Path]::GetFullPath("$Script:ScriptPath/Contrib/NLTestToolkit/NLTest.ps1")
Write-Verbose "Expected NLTest path: $nlTestPath"
if (!(Test-Path -LiteralPath $nlTestPath -PathType Leaf)) { "File '$nlTestPath' does not exist. Check that source code base is in valid state." }

# First step: build sources

Write-Progress -Activity "Building, testing and installing NLedger" -Status "Building source code [$(if($coreOnly){".Net Core"}else{".Net Framework,.Net Core"})] [$(if($debugMode){"Debug"}else{"Release"})]"

[string]$buildCommandLine = "dotnet build '$solutionPath'"
if ($coreOnly) { $buildCommandLine += " /p:CoreOnly=True"}
if ($debugMode) { $buildCommandLine += " --configuration Debug"} else { $buildCommandLine += " --configuration Release" }
Write-Verbose "Build sources command line: $buildCommandLine"

$null = (Invoke-Expression $buildCommandLine | Write-Verbose)
if ($LASTEXITCODE -ne 0) { throw "Build failed for some reason. Run this script again with '-Verbose' to get more information about the cause." }

# Second step: run unit tests

if(!$noUnitTests) {
    Write-Progress -Activity "Building, testing and installing NLedger" -Status "Running unit tests [$(if($coreOnly){".Net Core"}else{".Net Framework,.Net Core"})] [$(if($debugMode){"Debug"}else{"Release"})]"

    [string]$unittestCommandLine = "dotnet test '$solutionPath'"
    if ($coreOnly) { $unittestCommandLine += " /p:CoreOnly=True"}
    if ($debugMode) { $unittestCommandLine += " --configuration Debug"} else { $buildCommandLine += " --configuration Release" }
    Write-Verbose "Run unit tests command line: $unittestCommandLine"

    $null = (Invoke-Expression $unittestCommandLine | Write-Verbose)
    if ($LASTEXITCODE -ne 0) { throw "Unit tests failed for some reason. Run this script again with '-Verbose' to get more information about the cause." }
}

# Third step: run integration tests

function composeNLedgerExePath {
    Param([Parameter(Mandatory=$True)][bool]$core)
    [string]$private:config = $(if($debugMode){"Debug"}else{"Release"})
    [string]$private:framework = $(if($core){"netcoreapp3.1"}else{"net45"})
    [string]$private:extension = $(if($isWindowsPlatform){".exe"}else{""})
    return [System.IO.Path]::GetFullPath("$Script:ScriptPath/Source/NLedger.CLI/bin/$private:config/$private:framework/NLedger-cli$private:extension")
}    

if (!$noNLTests)
{
    if (!$coreOnly){
        Write-Progress -Activity "Building, testing and installing NLedger" -Status "Running integration tests [.Net Framework] [$(if($debugMode){"Debug"}else{"Release"})]"

        [string]$nledgerFrameworkExeFile = composeNLedgerExePath -core $false
        if (!(Test-Path -LiteralPath $nledgerFrameworkExeFile -PathType Leaf)) { throw "Cannot find $nledgerFrameworkExeFile" }

        $null = (& $nlTestPath -nledgerExePath $nledgerFrameworkExeFile -noconsole -showProgress | Write-Verbose)
        if ($LASTEXITCODE -ne 0) { throw "Integration tests failed for some reason. Run this script again with '-Verbose' to get more information about the cause." }
    }

    Write-Progress -Activity "Building, testing and installing NLedger" -Status "Running integration tests [.Net Core] [$(if($debugMode){"Debug"}else{"Release"})]"

    [string]$nledgerCoreExeFile = composeNLedgerExePath -core $true
    if (!(Test-Path -LiteralPath $nledgerCoreExeFile -PathType Leaf)) { throw "Cannot find $nledgerCoreExeFile" }

    $null = (& $nlTestPath -nledgerExePath $nledgerCoreExeFile -noconsole -showProgress | Write-Verbose)
    if ($LASTEXITCODE -ne 0) { throw "Integration tests failed for some reason. Run this script again with '-Verbose' to get more information about the cause." }
}


Write-Progress -Activity "Building, testing and installing NLedger" -Status "Completed" -Completed

# Print summary

Write-Host "*** NLedger Build succeeded ***"
Write-Host

Write-Host "Build source code: OK [$(if($coreOnly){".Net Core"}else{".Net Framework,.Net Core"})] [$(if($debugMode){"Debug"}else{"Release"})]"
if (!($coreOnly)) {Write-Host "   .Net Framework binary: $(composeNLedgerExePath -core $false)"}
Write-Host "   .Net Core binary: $(composeNLedgerExePath -core $true)"
Write-Host

if (!($noUnitTests)) {
    Write-Host "Unit tests: OK [$(if($coreOnly){".Net Core"}else{".Net Framework,.Net Core"})] [$(if($debugMode){"Debug"}else{"Release"})]"
} else {
    Write-Host "Unit tests: IGNORED"
}
Write-Host

if (!($noNLTests)) {
    Write-Host "NLedger tests: OK [$(if($coreOnly){".Net Core"}else{".Net Framework,.Net Core"})] [$(if($debugMode){"Debug"}else{"Release"})]"
} else {
    Write-Host "NLedger tests: IGNORED"
}
Write-Host
