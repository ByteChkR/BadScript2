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
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish App 'PackageManager' 20%" -PercentComplete 20
    . $bs build PublishApp
    cd ../VersionChange
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish App 'VersionChange' 40%" -PercentComplete 40
    . $bs build PublishApp
    cd ../WebFramework
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish Library 'WebFramework' 50%" -PercentComplete 50
    . $bs build PublishLib
    cd ../HighscoreApi
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish Library 'HighscoreApi' 70%" -PercentComplete 70
    . $bs build PublishLib
    cd ../HighscoreApi.Console
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish App 'HighscoreApi.Console' 80%" -PercentComplete 80
    . $bs build PublishApp
    cd ../CommandlineParser
    Write-Progress -Activity "BadScript2 Project Build" -Status "Publish Library 'CommandlineParser' 100%" -PercentComplete 100
    . $bs build PublishLib
    cd ..
}

if ($writeLog -eq $true) {
    Build-Projects
}
else {
    $null = Build-Projects
}

Write-Progress -Activity "BadScript2 Project Build" -Complete