param ($config="Debug", [Switch] $writeLog=$false)


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
    Write-Progress -Activity "BadScript2 Build" -Status "Build BadScript2 Project 20%" -PercentComplete 20
    cd src/BadScript2.Console/BadScript2.Console
    dotnet publish -o ../../../build -c $config --os $os
    cd ../../..
    cp _copy_to_build/* build/
}

function Build-Projects {
    cd projects
    cd BuildSystem
    Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'BuildSystem' 30%" -PercentComplete 30
    . $bs build ReleaseLib
    cd ../BuildSystem.Console
    Write-Progress -Activity "BadScript2 Build" -Status "Build App 'BuildSystem.Console' 40%" -PercentComplete 40
    . $bs build ReleaseApp
    cd ../Debugger
    Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'Debugger' 50%" -PercentComplete 50
    . $bs build ReleaseRun
    cd ../PackageHandler
    Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'PackageHandler' 60%" -PercentComplete 60
    . $bs build ReleaseStartup
    cd ../System
    Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'System' 70%" -PercentComplete 70
    . $bs build ReleaseLib
    cd ../WebFramework
    Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'WebFramework' 80%" -PercentComplete 80
    . $bs build ReleaseLib
    cd ../HighscoreApi
    Write-Progress -Activity "BadScript2 Build" -Status "Build Library 'HighscoreApi' 90%" -PercentComplete 90
    . $bs build ReleaseLib
    cd ../HighscoreApi.Console
    Write-Progress -Activity "BadScript2 Build" -Status "Build App 'HighscoreApi.Console' 100%" -PercentComplete 100
    . $bs build ReleaseApp
    cd ../..
}

if ($writeLog -eq $true) {
    echo "Building Language for OS: '$os' with Config: '$config'.."
    Build-Language
    echo "Building BadScript2 Projects"
    Build-Projects
}
else {
    echo "Building Language for OS: '$os' with Config: '$config'.."
    $null = Build-Language
    echo "Building BadScript2 Projects"
    $null = Build-Projects
}

Write-Progress -Activity "BadScript2 Build" -Complete
