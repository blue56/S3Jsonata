$date = Get-Date
$version = $date.ToString("yyyy-dd-M--HH-mm-ss")
$filename = "S3Jsonata" + $version + ".zip"
cd .\S3Jsonata\src\S3Jsonata
dotnet lambda package ..\..\..\Packages\$filename --configuration Release -frun dotnet6 -farch arm64
cd ..\..\..