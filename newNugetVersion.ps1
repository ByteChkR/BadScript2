param ($version="(~){yy}.(~){MM}.(~){dd}.+", $sources="./src", $postfix="")
$bsDir = "$pwd/build/"

if (-not(Test-Path $bsDir)) { #Only Build if the directory does not exist.
    . ./build.ps1 -noProjects -noTests -writeLog
}

if ($IsWindows -eq $true) {
    $bsFile = $bsDir + 'bs.exe'
}
else {
    $bsFile = $bsDir + 'bs'
}

echo "BadScript Runtime Located at $bsFile"
. $bsFile ./newNugetVersion.bs $version $sources $postfix
echo "Completed"
