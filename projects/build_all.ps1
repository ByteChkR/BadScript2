
$bs = "../../src/BadScript2.Console/bin/Release/net6.0/bs.exe"

cd Assert
. $bs run -f build.bs -a install
cd ../Commandline
. $bs run -f build.bs -a install
cd ../Debugger
. $bs run -f build.bs -a install
cd ../Enumerables
. $bs run -f build.bs -a install
cd ../Events
. $bs run -f build.bs -a install
cd ../Linq
. $bs run -f build.bs -a install
cd ../Logging
. $bs run -f build.bs -a install
cd ../PackageHandler
. $bs run -f build.bs -a install
cd ../Project
. $bs run -f build.bs -a install
cd ../ProjectUtils
. $bs run -f build.bs -a install
cd ../SourceReader
. $bs run -f build.bs -a install
cd ..