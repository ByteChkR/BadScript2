. ./build.ps1 -publishProjects -config Release
cd build
. ./BSShell.ps1
cd ../src
. bs vchange "(~){yyyy}.(~){MM}.(~){dd}.+"
cd ..