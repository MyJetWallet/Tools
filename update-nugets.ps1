Write-Host "Congratulations! Your first script executed successfully"

$dir = Get-Location;

$list = Get-ChildItem -Path ..\ -Filter "*.sln" -Recurse | %{$_.DirectoryName};

foreach ($sln in $list) {
    $sln;

    Set-Location $sln;

    git pull

    dotnet outdated -u

    git commit -a -m "update nugets"

    git push -u origin master
}

Set-Location @dir;


