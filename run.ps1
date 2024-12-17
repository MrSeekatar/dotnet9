#! pwsh
<#
.SYNOPSIS
Helper for running various tasks

.PARAMETER Tasks
One or more tasks to run. Use tab completion to see available tasks.

.PARAMETER DockerTag
For building docker images, the tag to use. Defaults to a timestamp

.PARAMETER Plain
For building docker images, use plain output since default collapses output after each step.

.PARAMETER NoCache
For building docker images, add --no-cache

.PARAMETER RunDockerTests
When doing BuildDocker, run the tests in docker

.PARAMETER NugetUserName
Username for nuget to pull in Docker. Defaults to $env:nuget_username

.PARAMETER NugetPw
Username for nuget to pull in Docker. Defaults to $env:nuget_password

.EXAMPLE
./run buildDocker -Plain -NoCache

Build the docker image with plain output

#>
[CmdletBinding()]
param (
    [ArgumentCompleter({
        param($commandName, $parameterName, $wordToComplete, $commandAst, $fakeBoundParameters)
        $runFile = (Join-Path (Split-Path $commandAst -Parent) run.ps1)
        if (Test-Path $runFile) {
            Get-Content $runFile |
                    Where-Object { $_ -match "^\s+['`"]([\w+-]+)['`"]\s*{" } |
                    ForEach-Object {
                        if ( !($fakeBoundParameters[$parameterName]) -or
                            (($matches[1] -notin $fakeBoundParameters.$parameterName) -and
                             ($matches[1] -like "$wordToComplete*"))
                            )
                        {
                            $matches[1]
                        }
                    }
        }
     })]
    [string[]] $Tasks,
    [switch] $Plain,
    [switch] $NoCache,
    [int] $TestPort = 44300,
    [string] $CertPassword = $env:cert_password
)

$appName = 'BoxServer' # image, folder names

$currentTask = ""

function buildDocker( [string] $file, [string] $imageName) {

    executeSB -RelativeDir 'src' {
        Write-Verbose $PWD
        $extra = @()
        if ($Plain) {
            $extra += "--progress","plain"
        }

        if ($NoCache){
            $extra += "--no-cache"
        }
        $lowerImageName = $imageName.ToLowerInvariant()

        docker build  `
                --tag ${lowerImageName}:latest `
                --file $file `
                @extra `
                .
    }
}

$imageName = "box-api"

# execute a script, checking lastexit code
function executeSB
{
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [scriptblock] $ScriptBlock,
    [string] $RelativeDir,
    [string] $Name = $currentTask
)
    if ($RelativeDir) {
        Push-Location (Join-Path $PSScriptRoot $RelativeDir)
    } else {
        Push-Location $PSScriptRoot
    }
    try {
        $global:LASTEXITCODE = 0

        Invoke-Command -ScriptBlock $ScriptBlock

        if ($LASTEXITCODE -ne 0) {
            throw "Error executing command '$Name', last exit $LASTEXITCODE"
        }
    } catch {
        Write-Error "$_`n$($_.ScriptStackTrace)"
        throw $_
    } finally {
        Pop-Location
    }
}

foreach ($currentTask in $Tasks) {

    try {
        $prevPref = $ErrorActionPreference
        $ErrorActionPreference = "Stop"

        "-------------------------------"
        "Starting $currentTask"
        "-------------------------------"

        switch ($currentTask) {
            'build' {
                executeSB -RelativeDir 'src' {
                    dotnet build
                }
            }
            'watch' {
                executeSB -RelativeDir "src/${appName}Api" {
                    dotnet watch
                }
            }
            'runApi' {
                executeSB -RelativeDir "src/${appName}Api" {
                    dotnet run
                }
            }
            'runApiDocker' {
                if (!$CertPassword) {
                    Write-Warning "No certificate password provided. Use -CertPassword"
                    exit
                }
                docker run --rm -p ${TestPort}:44300 `
                                -e ASPNETCORE_Kestrel__Certificates__Default__Password="$certPassword" `
                                -e ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx `
                                $imageName
            }
            "buildApiDocker" {
                if (!(Test-Path (Join-Path $PSScriptRoot src/aspnetapp.pfx))) {
                    Write-Warning "No certificate found at src/aspnetapp.pfx. Create with: "
                    Write-Warning "  dotnet dev-certs https -ep aspnetapp.pfx -p `$env:cert_password"
                    Write-Warning "  dotnet dev-certs https --trust"
                    Write-Warning "The password must be the same one you pass in as -CertPassword"
                    Write-Warning "On Mac and Linux chmod 0644 aspnetapp.pfx"
                    exit
                }
                buildDocker -file "BoxServerApi/Dockerfile" -imageName $imageName
                docker tag box-api:latest loyal.azurecr.io/box-api:latest # for testing with helm since Loyal's chart uses this
            }
            "runAppHost" {
                executeSB -RelativeDir "src/Box.AppHost" {
                    dotnet run
                }
            }
            default {
                Write-Warning "Unknown task $currentTask"
            }
        }

    } finally {
        $ErrorActionPreference = $prevPref
    }
}
