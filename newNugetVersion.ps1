param ($version="(~){yy}.(~){MM}.(~){dd}.+", $sources="./src", $postfix="")
$bs = "$pwd/build/"

if (-not(Test-Path $bs)) { #Only Build if the directory does not exist.
    . ./build.ps1 -noProjects -noTests -writeLog
}

if ($IsWindows -eq $true) {
    $bsFile = $bs + 'bs.exe'
}
else {
    $bsFile = $bs + 'bs'
}

echo "BadScript Runtime Located at $bsFile"
. $bsFile ./newNugetVersion.bs $version $sources $postfix
echo "Completed"
