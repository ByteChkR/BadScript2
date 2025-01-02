$bs = "$pwd/build/"

if (-not(Test-Path $bs)) { #Only Build if the directory does not exist.
    . ./build.ps1 -noTests -writeLog
}

if ($IsWindows -eq $true) {
    $bsFile = $bs + 'bs.exe'
}
else {
    $bsFile = $bs + 'bs'
}

echo "BadScript Runtime Located at $bsFile"
cd docs
. $bsFile bsdoc
echo "Completed"
cd ..
