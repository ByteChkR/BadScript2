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
        [pscustomobject]@{Name='BuildSystem'; Target=$config}
        [pscustomobject]@{Name='BuildSystem.Console'; Target=$config}
        [pscustomobject]@{Name='PackageHandler'; Target=$config}
        [pscustomobject]@{Name='Debugger'; Target=$config}
        [pscustomobject]@{Name='Assert'; Target=$config}
        [pscustomobject]@{Name='CommandlineParser'; Target=$config}
        [pscustomobject]@{Name='Enumerables'; Target=$config}
        [pscustomobject]@{Name='Event'; Target=$config}
        [pscustomobject]@{Name='Logging'; Target=$config}
        [pscustomobject]@{Name='SourceReader'; Target=$config}
        [pscustomobject]@{Name='VersionChange'; Target=$config}
        [pscustomobject]@{Name='NewProject'; Target=$config}
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
