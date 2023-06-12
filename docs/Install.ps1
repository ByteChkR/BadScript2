
if ((Get-Command "git" -ErrorAction SilentlyContinue) -eq $null) 
{ 
   Write-Host "Unable to find 'git' in your PATH"
   Exit
}

if ((Get-Command "dotnet" -ErrorAction SilentlyContinue) -eq $null) 
{ 
   Write-Host "Unable to find 'git' in your PATH"
   Exit
}

if (Test-Path -Path "./BadScript2") {
    Write-Host "Unable to clone repository. Directory already exists!"
    Exit
}

Write-Host "Cloning Repository"
git clone https://github.com/ByteChkR/BadScript2.git

Write-Host "Building Projects"
cd BadScript2
. ./build.ps1
cd ..
Write-Host "Building Complete. Run 'BSShell.ps1' located in the 'build' directory to add the 'bs' command to a shell session."