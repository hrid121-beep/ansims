@echo off
REM Quick Commit and Push Script
REM When you make local changes, run this to commit and push

echo.
echo ====================================
echo   Commit and Push Your Changes
echo ====================================
echo.

REM Check if there are any changes
git status -s > temp_status.txt
set /p HAS_CHANGES=<temp_status.txt
del temp_status.txt

if "%HAS_CHANGES%"=="" (
    echo [INFO] No changes to commit.
    echo [INFO] Everything is up to date!
    pause
    exit /b 0
)

echo [INFO] Changes detected:
git status -s
echo.

REM Ask for commit message
set /p COMMIT_MSG="Enter commit message (or press Enter for default): "

if "%COMMIT_MSG%"=="" (
    set COMMIT_MSG=Update: Local changes
)

echo.
echo [1/3] Adding all changes...
git add .
if errorlevel 1 (
    echo [ERROR] Failed to add changes
    pause
    exit /b 1
)
echo [OK] Changes added
echo.

echo [2/3] Committing with message: "%COMMIT_MSG%"
git commit -m "%COMMIT_MSG%"
if errorlevel 1 (
    echo [ERROR] Commit failed
    pause
    exit /b 1
)
echo [OK] Committed successfully
echo.

echo [3/3] Pushing to GitHub master...
git push origin master
if errorlevel 1 (
    echo [ERROR] Push failed. May need to pull first.
    echo [INFO] Try running: git pull origin master
    pause
    exit /b 1
)
echo [OK] Pushed successfully
echo.

echo ====================================
echo   SUCCESS! Changes Pushed!
echo ====================================
echo.
echo Latest commits:
git log --oneline -5
echo.
pause
