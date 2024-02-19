param ($config="Debug", [Switch] $writeLog=$false, [Switch] $noTests=$false, [Switch] $noProjects=$false, [Switch] $noBuild=$false)

if ($IsWindows) {
    $os = "win"
    $bs = "$pwd/build/bs.exe"
} 
elseif ($IsLinux) {
    $os = "linux"
    $bs = "$pwd/build/bs"
}
elseif ($IsMacOS) {
    $os = "osx"
    $bs = "$pwd/build/bs"
}
else {
    write-output "Could not determine OS Version. Exiting.."
    exit
}

function Build-Language {
    if ($noBuild -eq $false)
    {
        echo "Building Language for OS: '$os' with Config: '$config'.."
        Write-Progress -Activity "BadScript2 Build" -Status "Build BadScript2 Project 10%" -PercentComplete 10
        if (Test-Path "build") {
            Remove-Item "build" -Force -Recurse
        }
        cd src/BadScript2.Console/BadScript2.Console
        dotnet publish -o ../../../build -c $config --os $os
        cd ../../..
        cp _copy_to_build/* build/
    }
}

function Build-Projects {
    if ($noProjects -eq $false)
    {
        echo "Building BadScript2 Projects"
        cd projects
        cd BuildSystem
        Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'BuildSystem' 10%" -PercentComplete 10
        . $bs build ReleaseLib
        cd ../BuildSystem.Console
        Write-Progress -Activity "BadScript2 Build" -Status "Build App 'BuildSystem.Console' 20%" -PercentComplete 20
        . $bs build ReleaseApp
        cd ../Debugger
        Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'Debugger' 30%" -PercentComplete 30
        . $bs build ReleaseRun
        cd ../PackageHandler
        Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'PackageHandler' 40%" -PercentComplete 40
        . $bs build ReleaseStartup
        cd ../System
        Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'System' 50%" -PercentComplete 50
        . $bs build ReleaseLib
        cd ../CommandlineParser
        Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'CommandlineParser' 70%" -PercentComplete 70
        . $bs build ReleaseLib
        cd ../VersionChange
        Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'VersionChange' 90%" -PercentComplete 90
        . $bs build ReleaseApp
        cd ../..
    }
}

if ($writeLog -eq $true) {
    Build-Language
    Build-Projects
}
else {
    $null = Build-Language
    $null = Build-Projects
}

Write-Progress -Activity "BadScript2 Build" -Complete

if ($noTests -eq $false)
{
    . $bs test
    Remove-Item "TestResult.xml"
}
