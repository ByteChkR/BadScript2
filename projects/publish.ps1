cd ..
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

function Build-Projects {
    . $bs pm add origin Byt3_Local ./docs/core_repo
    cd projects/PackageManager
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish App 'PackageManager' 20%" -PercentComplete 10
    . $bs build PublishApp
    cd ../VersionChange
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish App 'VersionChange' 40%" -PercentComplete 20
    . $bs build PublishApp
    cd ../WebFramework
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish Library 'WebFramework' 50%" -PercentComplete 30
    . $bs build PublishLib
    cd ../HighscoreApi
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish Library 'HighscoreApi' 70%" -PercentComplete 40
    . $bs build PublishLib
    cd ../HighscoreApi.Console
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish App 'HighscoreApi.Console' 80%" -PercentComplete 50
    . $bs build PublishApp
    cd ../CommandlineParser
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish Library 'CommandlineParser' 100%" -PercentComplete 60
    . $bs build PublishLib
    cd ../System
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish Library 'System' 100%" -PercentComplete 70
    . $bs build PublishLib
    cd ../BuildSystem
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish Library 'BuildSystem' 100%" -PercentComplete 70
    . $bs build PublishLib
    cd ../BuildSystem.Console
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish Library 'BuildSystem.Console' 100%" -PercentComplete 90
    . $bs build PublishApp
    cd ..
}

if ($writeLog -eq $true) {
    Build-Projects
}
else {
    $null = Build-Projects
}

Write-Progress -Activity "BadScript2 Project Build" -Complete