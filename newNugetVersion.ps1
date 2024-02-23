param ($version="(~){yy}.(~){MM}.(~){dd}.+", $sources="./src", $postfix="")
. ./build.ps1 -noProjects -noTests

$bs = "$pwd/build/"

if ($IsWindows -eq $true) {
    $bsFile = $bs + 'bs.exe'
}
else {
    $bsFile = $bs + 'bs'
}

echo "BadScript Runtime Located at $bsFile"
. $bsFile ./newNugetVersion.bs $version $sources $postfix
echo "Completed"
