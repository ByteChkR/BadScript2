$bs = "$pwd/"



if ($IsWindows -eq $true) {
    $bsFile = $bs + 'bs.exe'
}
else {
    $bsFile = $bs + 'bs'
}

$bs = (Get-Item $bs).fullname

$Env:Path += ";$bs"
echo "Bad Script Executable Directory: $bs"
echo "Bad Script Executable: $bsFile"
echo "Run Bad Script: 'bs <script-path>'"