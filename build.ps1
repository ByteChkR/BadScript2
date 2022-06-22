echo "Building Language.."
cd src
dotnet publish -o ../build -c Debug
cd ..

cp _copy_to_build/* build/
cd build

. ./BSShell.ps1
cd ..

cd projects
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