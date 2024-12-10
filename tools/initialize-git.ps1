#! pwsh
if (!$IsWindows) {
    chmod +x ./run.ps1
}

if (!(Get-Item .git -Force)) {
    git init --initial-branch=master
    $commitMessage = "Initial commit"
} else {
    $commitMessage = "Template update"
}

git add .
git commit -m $commitMessage --quiet
