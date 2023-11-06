param ($version="(~){yyyy}.(~){MM}.(~){dd}.+", $sources="./src")
. ./build.ps1 -noProjects -noTests -writeLog
cd build
. ./BSShell.ps1
cd ..
. bs ./newNugetVersion.bs $version $sources
