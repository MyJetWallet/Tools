Write-Host "Congratulations! Your first script executed successfully"

$dir = Get-Location;

$list = Get-ChildItem -Path ..\ -Filter "*.sln" -Recurse | %{$_.DirectoryName};

foreach ($sln in $list) {
    $sln;

    Set-Location $sln;

    if ($sln -eq "1111111-MyJetWallet.Domain") {
        continue;
    }

    git commit -a -m "fix update_nuget.yaml"

    git push -u origin master

    Start-Sleep -s 5
}

Set-Location @dir;


