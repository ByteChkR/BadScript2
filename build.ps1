param ($config="Debug", [Switch] $writeLog=$false, [Switch] $noTests=$false, [Switch] $noProjects=$false, [Switch] $noBuild=$false)

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
    if ($noBuild -eq $false)
    {
        echo "Building Language for OS: '$os' with Config: '$config'.."
        Write-Progress -Activity "BadScript2 Build" -Status "Build BadScript2 Runtime "
        if (Test-Path "build") {
            Remove-Item "build" -Force -Recurse
        }
        cd src/BadScript2.Console/BadScript2.Console
        dotnet publish -o ../../../build -c $config --os $os
        cd ../../..
        cp _copy_to_build/* build/
    }
}
function Build-Project
{
    $name = $args[0]
    $target = $args[1]
    cd $name
    . $bs build $target
    cd ..
}

$projects = @(
        [pscustomobject]@{Name='BuildSystem'; Target='ReleaseLib'}
        [pscustomobject]@{Name='BuildSystem.Console'; Target='ReleaseApp'}
        [pscustomobject]@{Name='PackageHandler'; Target='ReleaseStartup'}
        [pscustomobject]@{Name='Debugger'; Target='ReleaseRun'}
        [pscustomobject]@{Name='Assert'; Target='ReleaseLib'}
        [pscustomobject]@{Name='CommandlineParser'; Target='ReleaseLib'}
        [pscustomobject]@{Name='Enumerables'; Target='ReleaseLib'}
        [pscustomobject]@{Name='Event'; Target='ReleaseLib'}
        [pscustomobject]@{Name='Logging'; Target='ReleaseLib'}
        [pscustomobject]@{Name='SourceReader'; Target='ReleaseLib'}
        [pscustomobject]@{Name='VersionChange'; Target='ReleaseApp'}
    )


function Build-Projects {
    if ($noProjects -eq $false)
    {
        echo "Building BadScript2 Projects"
        cd projects

        for (($i = 0); $i -lt $projects.Length; $i++)
        {
            $project = $projects[$i]
            Write-Progress -Activity "BadScript2 Build" -Status "Build Project '$($project.Name)' $i/$($projects.Length)" -PercentComplete $(($i / $projects.Length) * 100)
            Build-Project $project.Name $project.Target
        }

        cd ..
    }
}

if ($writeLog -eq $true) {
    Build-Language
    Build-Projects
}
else {
    $null = Build-Language
    $null = Build-Projects
}

Write-Progress -Activity "BadScript2 Build" -Complete

if ($noTests -eq $false)
{
    . $bs test
    Remove-Item "TestResult.xml"
}
