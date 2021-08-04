Write-Host "Congratulations! Your first script executed successfully"

$dir = Get-Location;

$list = Get-ChildItem -Path ..\ -Filter "*.sln" -Recurse | %{$_.DirectoryName};

foreach ($sln in $list) {
    $sln;

    Set-Location $sln;

    git pull --progress -v --no-rebase "origin" master

}

Set-Location @dir;


Set-Location "../Flutter.BusinessLogicProvider";
git pull

Set-Location "../Flutter.JetWallet";
git pull

Set-Location "../Flutter.JetWallet.Android";
git pullcd 

Set-Location "../Flutter.JetWallet.Web";
git pull


Set-Location @dir;
