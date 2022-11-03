param ($config="Debug", [Switch] $writeLog=$false)


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

function Build-Language {
    cd src/BadScript2.Console
    dotnet publish -o ../../build -c $config --os $os
    cd ../..
    cp _copy_to_build/* build/
}

function Build-Projects {
    cd projects
    cd Debugger
    . $bs build ReleaseRun
    cd ../BuildSystem
    . $bs build ReleaseLib
    cd ../PackageHandler
    . $bs build ReleaseStartup
    cd ../System
    . $bs build ReleaseLib
    cd ../WebFramework
    . $bs build ReleaseLib
    cd ../..
}

if ($writeLog -eq $true) {
    echo "Building Language for OS: '$os' with Config: '$config'.."
    Build-Language
    echo "Building BadScript2 Projects"
    Build-Projects
}
else {
    echo "Building Language for OS: '$os' with Config: '$config'.."
    $null = Build-Language
    echo "Building BadScript2 Projects"
    $null = Build-Projects
}
