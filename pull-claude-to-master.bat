@echo off
REM Super Simple Script - Claude Changes থেকে Master এ আনার জন্য
REM কোন branch switching নেই - শুধু master এই থাকবেন

echo.
echo ========================================
echo   Claude Changes to Master - Simple
echo ========================================
echo.

REM Step 1: নিশ্চিত করুন আপনি master branch এ আছেন
echo [Step 1/4] Checking if you are on master branch...
for /f "tokens=*" %%i in ('git branch --show-current') do set CURRENT_BRANCH=%%i

if not "%CURRENT_BRANCH%"=="master" (
    echo.
    echo [INFO] You are currently on: %CURRENT_BRANCH%
    echo [INFO] Switching to master branch...
    git checkout master
    if errorlevel 1 (
        echo [ERROR] Cannot switch to master. Please switch manually:
        echo   git checkout master
        pause
        exit /b 1
    )
)

echo [OK] You are on master branch
echo.

REM Step 2: Remote থেকে latest changes fetch করুন
echo [Step 2/4] Fetching latest changes from GitHub...
git fetch origin
if errorlevel 1 (
    echo [ERROR] Failed to fetch from GitHub
    echo [INFO] Check your internet connection
    pause
    exit /b 1
)
echo [OK] Fetched successfully
echo.

REM Step 3: Claude branch এর changes master এ merge করুন
echo [Step 3/4] Getting changes from claude branch...
git merge origin/claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn --no-edit
if errorlevel 1 (
    echo [ERROR] Merge failed - may have conflicts
    echo [INFO] Please resolve conflicts and run:
    echo   git add .
    echo   git commit -m "Merge claude changes"
    echo   git push origin master
    pause
    exit /b 1
)
echo [OK] Claude changes merged successfully
echo.

REM Step 4: GitHub master এ push করুন
echo [Step 4/4] Pushing to GitHub master...
git push origin master
if errorlevel 1 (
    echo [ERROR] Push failed
    echo [INFO] You may need to pull first:
    echo   git pull origin master
    pause
    exit /b 1
)
echo [OK] Pushed successfully
echo.

echo ========================================
echo   SUCCESS! All Done!
echo ========================================
echo.
echo Your local master now has all claude changes
echo and GitHub master is also updated!
echo.
echo Latest commits:
git log --oneline -5
echo.
echo Press any key to close...
pause > nul
