. ./build.ps1 -publishProjects -config Release
cd build
. ./BSShell.ps1
cd ../src
. bs pm update
. bs pm add package vchange
. bs vchange "(~){yyyy}.(~){MM}.(~){dd}.+"
cd ..