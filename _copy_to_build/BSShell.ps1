$bs = "$pwd/"
$bsFile = $bs + 'bs.exe'

$bs = (Get-Item $bs).fullname

$Env:Path += ";$bs"
echo "Bad Script Executable Directory: $bs"
echo "Run Bad Script: 'bs <script-path>'"