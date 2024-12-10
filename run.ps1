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

.PARAMETER DeleteDocker
Add --rm to docker build

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
                    Where-Object { $_ -match "^\s+'([\w+-]+)'\s*{" } |
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
    [string] $DockerTag = [DateTime]::Now.ToString("MMdd-HHmmss"),
    [switch] $Plain,
    [switch] $NoCache,
    [bool] $DeleteDocker = $True,
    [switch] $RunDockerTests,
    [string] $NugetUserName = $env:nuget_username,
    [string] $NugetPw = $env:nuget_password,
    [int] $TestPort = 44300,
    [string] $PersesTag = "latest"
)

$appName = 'BoxServer' # image, folder, Perses Journal names

function commonPersesArgs([string] $folderPath = "/Scripts/")
{ @(
    "--env-file", ".env",
    "-e", "PERSES__JOURNALTABLE=$appName",
    "-e", "PERSES__ENVIRONMENT=dev",
    "-e", "PERSES__FOLDERPATH=$folderPath",
    "--rm"
)
}

$currentTask = ""

function Get-DockerEnvFile([string[]] $extra) {
    $folder = $PSScriptRoot
    while ($folder -and !(Test-Path (Join-Path $folder shared.appsettings.Development.json))) {
        $folder = Split-Path $folder -Parent
    }
    if ($folder -and (Test-Path (Join-Path $folder shared.appsettings.Development.json))) {
        $env = Get-Content (Join-Path $folder shared.appsettings.Development.json) -raw | ConvertFrom-json
        Get-Member -input $env -MemberType NoteProperty | ForEach-Object {
            $name = $_.name
            "$($name -replace ':','__')=$((($env.$name -replace "`n","\n") -replace "(localhost|127.0.0.1|::1)","host.docker.internal")-replace ':9092',':29092')"
        } | Out-File (Join-Path $PSScriptRoot ".env") -Encoding ascii
    } else {
        Write-Warning "No shared appsettings found."
        Set-Content (Join-Path $PSScriptRoot ".env") -Encoding ascii -Value ''
        return
    }
    foreach ($e in $extra) {
        $e | Out-File (Join-Path $PSScriptRoot ".env") -Encoding ascii -Append
    }
    "Wrote $(Join-Path $PSScriptRoot ".env")"
}

function buildDocker( [string] $file, [string] $imageName, [string] $tag, [switch] $runTest ) {

    executeSB -RelativeDir 'src' {
        Write-Verbose $PWD
        $extra = @()
        if ($Plain) {
            $extra += "--progress","plain"
        }
        if ($DeleteDocker) {
            $extra += "--rm"
        }
        $deleteIgnore = $false
        $fullPath = Split-Path (Convert-Path $file) -Parent
        if ((Test-Path (Join-Path $fullPath '.dockerignore')) -and ($fullPath -ne $PWD) ) {
            $deleteIgnore = $true
            Copy-Item (Join-Path $fullPath '.dockerignore') '.'
        }

        if ($IsMacOS -and (uname -m | Select-String ARM64)) {
            # this assumes you have built the loyal parent images locally for the M1
            $extra += "--build-arg", "registry=docker.io/library",
                    "--build-arg", "aspNetVersion=9.0-bookworm-slim-arm64v8",
                    "--build-arg", "sdkVersion=9.0-bookworm-slim-arm64v8"
        }
        $buildExtra = $extra
        if ($NoCache){
            $buildExtra += "--no-cache"
        }

        $target = 'build'
        if ($runTest) {
            $target = 'build-test-output'
            $buildExtra += "--output", "../docker-build-output"
        }

        Write-Verbose "Target is $target File is $file"
        Write-Verbose "BuildExtra is $($buildExtra -join ' ')"
        Write-Verbose "Extra is $($extra -join ' ')"
        "-----------------------------------"
        "  docker build for publish$($runTest ? ' and test' : '')"
        "-----------------------------------"
        docker build  `
                --target $target `
                --file $file `
                @buildExtra `
                .
        "Build and test output written to ../docker-build-output"

        if ($LASTEXITCODE -eq 0) {
            $lowerImageName = $imageName.ToLowerInvariant()
            "-----------------------------------"
            "  docker build for final"
            "-----------------------------------"
            docker build  `
                    --tag ${lowerImageName}:$tag `
                    --file $file `
                    @extra `
                    .
            if ($LASTEXITCODE -eq 0) {
                docker tag ${lowerImageName}:$tag ${lowerImageName}:latest
            }
        }
        if ($deleteIgnore) {
            Remove-Item "./.dockerignore" -Force -ErrorAction SilentlyContinue
        }
    }
}

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
            'testUnit' {
                executeSB -RelativeDir "src/test/unit" {
                    dotnet test
                }
            }
            'testIntegration' {
                $prev = $env:ApiTests__URIPREFIX
                try {
                    executeSB -RelativeDir "src/test/integration" {
                        $env:ApiTests__URIPREFIX="https://localhost:$TestPort"
                        dotnet test
                }
                } finally {
                    $env:ApiTests__URIPREFIX = $prev
                }
            }
            'run' {
                executeSB -RelativeDir "src/${appName}Api" {
                    dotnet run
                }
            }
            'watch' {
                executeSB -RelativeDir "src/${appName}Api" {
                    dotnet watch
                }
            }
            'runDocker' {
                Get-DockerEnvFile
                executeSB {
                    $DockerTag = "latest"
                    docker run --rm `
                               --env-file .env `
                               --interactive `
                               -e aspnetcore_hostBuilder__reloadConfigOnChange=false `
                               --publish "${TestPort}:44300" `
                               --tty `
                               --name $appName `
                               "$($appName.ToLowerInvariant()):$DockerTag"
                }
            }
            'buildDocker' {
                $dockerfile = "Dockerfile"
                executeSB -RelativeDir 'src' {
                    # If make your Dockerfile, you can skip this MakeDockerfile step
                    if (!(Get-Command MakeDockerfile -ErrorAction SilentlyContinue)) {
                        throw "MakeDockerFile is not installed. See https://github.com/loyalhealth/MakeDockerfile"
                    }
                    # outputs Dockerfile and .dockerignore to output folder
                    MakeDockerfile --mainFile ../DevOps/boxserver-api/variables/variables-common.yml --dotnetSdk 9.0 --output .
                    if ($LASTEXITCODE -ne 0) {
                        throw "Error making Dockerfile"
                    }
                }

                buildDocker -file $dockerfile -imageName $appName -tag $DockerTag -runTest $RunDockerTests
            }
            'buildPerses' {
                executeSB -RelativeDir '.' {
                    $file = "./DevOps/Docker/perses/Dockerfile"
                    $fullPath = Split-Path (Convert-Path $file) -Parent
                    if (Test-Path (Join-Path $fullPath '.dockerignore')) {
                        Copy-Item (Join-Path $fullPath '.dockerignore') '.'
                    }
                    $extra = @()
                    if ($Plain) {
                        $extra += "--progress","plain"
                    }
                    if ($DeleteDocker) {
                        $extra += "--rm"
                    }
                    if ($NoCache) {
                        $extra += "--no-cache"
                    }
                    $lowerAppName = $appName.ToLowerInvariant()
                    docker build `
                        --file $file `
                        --tag "${lowerAppName}-perses:$PersesTag" `
                        @extra `
                        .
                    Remove-Item "./.dockerignore" -Force -ErrorAction SilentlyContinue
                }
            }
            'generate' {
                if (!(Test-Path ~/swagger-codegen/Invoke-SwaggerGen.ps1)) {
                    Write-Warning "swagger-codegen not in ~/. You can get with with git clone git@github.com:Seekatar/swagger-codegen.git"
                }
                ~/swagger-codegen/Invoke-SwaggerGen.ps1 -OASFile (Join-Path $PSScriptRoot "doc/OAS/openapi.yaml") `
                                    -Namespace "$AppName.Models" `
                                    -OutputFolder (Join-Path ([System.IO.Path]::GetTempPath()) 'serverapi') `
                                    -ControllerNamespace "$appName.Controllers" `
                                    -RenameController `
                                    -RemoveEnumSuffix `
                                    -Force `
                                    -NoNullGuid `
                                    -NoToString `
                                    -NoValidateModel
                                    # -Verbose
                "`n`nOutput written to $(Join-Path ([System.IO.Path]::GetTempPath()) 'serverapi')"
            }
            'updateDb' {
                Get-DockerEnvFile
                executeSB  {
                    $baseDir = Convert-Path $PSScriptRoot
                    docker run (commonPersesArgs) -v "$baseDir/Scripts/${AppName}:/Scripts" loyal.azurecr.io/perses:master
                }
            }
            'updateDbDocker' {
                Get-DockerEnvFile
                executeSB {
                    docker run (commonPersesArgs("./Scripts")) "backendtemplate-perses:$PersesTag"
                }
            }
            'bootstrapDb' {
                Get-DockerEnvFile
                executeSB {
                    $baseDir = Convert-Path $PSScriptRoot
                    docker run (commonPersesArgs) -v "$baseDir/Scripts/Bootstrap:/Scripts" loyal.azurecr.io/perses:master
                }
            }
            'bootstrapDbDocker' {
                Get-DockerEnvFile
                executeSB {
                    docker run (commonPersesArgs "./Bootstrap") boxserver-perses:latest
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
