
if ((Get-Command "git" -ErrorAction SilentlyContinue) -eq $null) 
{ 
   Write-Host "Unable to find 'git' in your PATH"
   Exit
}

if ((Get-Command "dotnet" -ErrorAction SilentlyContinue) -eq $null) 
{ 
   Write-Host "Unable to find 'dotnet' in your PATH"
   Exit
}

if (Test-Path -Path "./BadScript2") {
    Write-Host "Found Installation. Pulling new commits"
    cd BadScript2
    git pull --recurse-submodules
    cd ..
}
else {
   Write-Host "Cloning Repository"
   git clone https://github.com/ByteChkR/BadScript2.git
   cd BadScript2
   git submodule init
   git pull --recurse-submodules
   cd ..
}

Write-Host "Building Projects"
cd BadScript2
. ./build.ps1
cd ..

Write-Host "Building Complete. Run '$pwd/BadScript2/build/BSShell.ps1' located in the 'build' directory to add the 'bs' command to a shell session."

if ($IsWindows) {
    $os = "win"
    $bs = "$pwd/BadScript2/build/bs.exe"
} 
elseif ($IsLinux) {
    $os = "linux"
    $bs = "$pwd/BadScript2/build/bs"
}
elseif ($IsMacOS) {
    $os = "osx"
    $bs = "$pwd/BadScript2/build/bs"
}
else {
    write-output "Could not determine OS Version."
    write-output "BadScript2 Console Root Directory: $pwd/BadScript2/build"
    exit
}

Write-Host "BadScript2 Console Executable for '$os': $bs"
cd BadScript2/build
. ./BSShell.ps1
cd ../..