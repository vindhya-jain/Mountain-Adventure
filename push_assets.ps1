$folders = Get-ChildItem -Directory -Path ".\Assets"
foreach ($folder in $folders) {
    Write-Host "Adding and committing $($folder.Name)"
    git add "Assets\$($folder.Name)"
    git commit -m "Add Assets/$($folder.Name)"
    git push origin main
}
