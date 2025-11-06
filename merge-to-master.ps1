# PowerShell Script to Merge Claude Branch to Master

Write-Host "`n=== Starting Auto Merge Process ===`n" -ForegroundColor Yellow

# Get the claude branch name
$CLAUDE_BRANCH = "claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn"

# Save current branch
$CURRENT_BRANCH = git branch --show-current
Write-Host "Current branch: $CURRENT_BRANCH" -ForegroundColor Green

# Check if there are uncommitted changes
$status = git status -s
if ($status) {
    Write-Host "`nError: You have uncommitted changes. Please commit or stash them first." -ForegroundColor Red
    git status -s
    exit 1
}

# Step 1: Checkout master
Write-Host "`n[1/6] Switching to master branch..." -ForegroundColor Yellow
git checkout master
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to checkout master" -ForegroundColor Red
    exit 1
}

# Step 2: Pull latest from remote master
Write-Host "`n[2/6] Pulling latest changes from origin/master..." -ForegroundColor Yellow
git pull origin master --no-rebase
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to pull from origin/master" -ForegroundColor Red
    Write-Host "Aborting and returning to $CURRENT_BRANCH" -ForegroundColor Yellow
    git checkout $CURRENT_BRANCH
    exit 1
}

# Step 3: Fetch latest claude branch
Write-Host "`n[3/6] Fetching latest from claude branch..." -ForegroundColor Yellow
git fetch origin $CLAUDE_BRANCH
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to fetch claude branch" -ForegroundColor Red
    exit 1
}

# Step 4: Merge claude branch into master
Write-Host "`n[4/6] Merging origin/$CLAUDE_BRANCH into master..." -ForegroundColor Yellow
git merge origin/$CLAUDE_BRANCH --no-edit
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Merge conflict detected!" -ForegroundColor Red
    Write-Host "Please resolve conflicts manually and then run:" -ForegroundColor Yellow
    Write-Host "  git add ."
    Write-Host "  git commit -m 'Merge claude branch'"
    Write-Host "  git push origin master"
    exit 1
}

# Step 5: Show what will be pushed
Write-Host "`n[5/6] Changes to be pushed:" -ForegroundColor Yellow
git log origin/master..HEAD --oneline

# Step 6: Push to remote master
Write-Host "`n[6/6] Pushing to origin/master..." -ForegroundColor Yellow
git push origin master
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to push to origin/master" -ForegroundColor Red
    Write-Host "You may need to push manually or resolve conflicts" -ForegroundColor Yellow
    exit 1
}

# Return to original branch if it wasn't master
if ($CURRENT_BRANCH -ne "master") {
    Write-Host "`nReturning to original branch: $CURRENT_BRANCH" -ForegroundColor Yellow
    git checkout $CURRENT_BRANCH
}

Write-Host "`n=== ✅ Successfully merged and pushed to master! ===`n" -ForegroundColor Green

# Show final status
Write-Host "Final status:" -ForegroundColor Green
git log --oneline -3

Write-Host "`nAll done! 🎉" -ForegroundColor Green
