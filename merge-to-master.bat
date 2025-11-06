@echo off
REM Windows Batch Script to Merge Claude Branch to Master
REM Encoding: UTF-8

setlocal enabledelayedexpansion

echo.
echo === Starting Auto Merge Process ===
echo.

REM Set the claude branch name
set CLAUDE_BRANCH=claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn

REM Get current branch
for /f "tokens=*" %%i in ('git branch --show-current') do set CURRENT_BRANCH=%%i
echo [INFO] Current branch: %CURRENT_BRANCH%

REM Check for uncommitted changes
git status -s > nul 2>&1
for /f %%i in ('git status -s ^| find /c /v ""') do set CHANGES=%%i
if !CHANGES! gtr 0 (
    echo.
    echo [ERROR] You have uncommitted changes. Please commit or stash them first.
    git status -s
    exit /b 1
)

REM Step 1: Checkout master
echo.
echo [1/6] Switching to master branch...
git checkout master
if errorlevel 1 (
    echo [ERROR] Failed to checkout master
    exit /b 1
)

REM Step 2: Pull latest from remote master
echo.
echo [2/6] Pulling latest changes from origin/master...
git pull origin master --no-rebase
if errorlevel 1 (
    echo [ERROR] Failed to pull from origin/master
    echo [INFO] Aborting and returning to %CURRENT_BRANCH%
    git checkout %CURRENT_BRANCH%
    exit /b 1
)

REM Step 3: Fetch latest claude branch
echo.
echo [3/6] Fetching latest from claude branch...
git fetch origin %CLAUDE_BRANCH%
if errorlevel 1 (
    echo [ERROR] Failed to fetch claude branch
    exit /b 1
)

REM Step 4: Merge claude branch into master
echo.
echo [4/6] Merging origin/%CLAUDE_BRANCH% into master...
git merge origin/%CLAUDE_BRANCH% --no-edit
if errorlevel 1 (
    echo [ERROR] Merge conflict detected!
    echo [INFO] Please resolve conflicts manually and then run:
    echo   git add .
    echo   git commit -m "Merge claude branch"
    echo   git push origin master
    exit /b 1
)

REM Step 5: Show what will be pushed
echo.
echo [5/6] Changes to be pushed:
git log origin/master..HEAD --oneline

REM Step 6: Push to remote master
echo.
echo [6/6] Pushing to origin/master...
git push origin master
if errorlevel 1 (
    echo [ERROR] Failed to push to origin/master
    echo [INFO] You may need to push manually or resolve conflicts
    exit /b 1
)

REM Return to original branch if it wasn't master
if not "%CURRENT_BRANCH%"=="master" (
    echo.
    echo [INFO] Returning to original branch: %CURRENT_BRANCH%
    git checkout %CURRENT_BRANCH%
)

echo.
echo === Successfully merged and pushed to master! ===
echo.

REM Show final status
echo [INFO] Final status:
git log --oneline -3

echo.
echo All done! 🎉
echo.

endlocal
exit /b 0
