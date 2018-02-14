Param([parameter(mandatory = $true)][string] $version)

dotnet pack ..\src\FileService\FileService.NetStandard20\FileService.NetStandard20.csproj -o  c:\code\NugetPackageSource -c Release /p:Version=2.0.$version-alpha