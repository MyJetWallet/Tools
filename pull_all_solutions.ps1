Write-Host "Congratulations! Your first script executed successfully"

$dir = Get-Location;

$list = Get-ChildItem -Path ..\ -Filter "*.sln" -Recurse | %{$_.DirectoryName};

foreach ($sln in $list) {
    $sln;

    Set-Location $sln;

    git pull

}

Set-Location @dir;


