param ($version="(~){yy}.(~){MM}.(~){dd}.+", $sources="./src")
. ./build.ps1 -noProjects -noTests

$bs = "$pwd/build/"

if ($IsWindows -eq $true) {
    $bsFile = $bs + 'bs.exe'
}
else {
    $bsFile = $bs + 'bs'
}

echo "BadScript Runtime Located at $bsFile"
. $bsFile ./newNugetVersion.bs $version $sources
echo "Completed"
