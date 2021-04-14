Write-Host "Congratulations! Your first script executed successfully"

$dir = Get-Location;

$list = Get-ChildItem -Path ..\ -Filter "*.sln" -Recurse | %{$_.DirectoryName};

foreach ($sln in $list) {
    $sln;

    Set-Location $sln;

    if ($sln -eq "MyJetWallet.Domain") {
        continue;
    }
    
    

    Copy-Item D:\SimplBit\git\JetWallet\MyJetWallet.Domain\.github\workflows\update_nuget.yaml .\.github\workflows\

    git commit -a -m "update notify_errors.yaml"

    git push -u origin master
}

Set-Location @dir;


