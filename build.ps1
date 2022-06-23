param ($config="Debug", [Switch] $writeLog=$false)

if ($IsWindows) {
    $os = "win"
} 
elseif ($IsLinux) {
    $os = "linux"
}
elseif ($IsMacOS) {
    $os = "osx"
}
else {
    write-host "Could not determine OS Version. Exiting.."
    exit
}

function Build-Language {
    cd src
    dotnet publish -o ../build -c $config --os $os
    cd ..
    cp _copy_to_build/* build/
}

function Build-Projects {
    cd build
    . ./BSShell.ps1
    cd ../projects
    cd Assert
    . bs run -f build.bs -a install
    cd ../Commandline
    . bs run -f build.bs -a install
    cd ../Debugger
    . bs run -f build.bs -a install
    cd ../Enumerables
    . bs run -f build.bs -a install
    cd ../Events
    . bs run -f build.bs -a install
    cd ../Linq
    . bs run -f build.bs -a install
    cd ../Logging
    . bs run -f build.bs -a install
    cd ../PackageHandler
    . bs run -f build.bs -a install
    cd ../Project
    . bs run -f build.bs -a install
    cd ../ProjectUtils
    . bs run -f build.bs -a install
    cd ../SourceReader
    . bs run -f build.bs -a install
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
