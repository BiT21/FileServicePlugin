Param([string] $version)
# Plugin
.\nuget.exe pack .\Plugin.nuspec -version 1.0.$version-alpha
move *.nupkg c:\code\NugetPackageSource

# .NetStandard
$p = Get-ChildItem -Path ..\ -Filter PublishOutput -Recurse
Get-ChildItem -Path $p.FullName -Filter *.nupkg | move -Destination c:\code\NugetPackageSource -Force
