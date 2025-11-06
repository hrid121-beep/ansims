@echo off
REM Quick Commit and Push Script
REM When you make local changes, run this to commit and push
SETLOCAL ENABLEDELAYEDEXPANSION

echo.
echo ====================================
echo   Commit and Push Your Changes
echo ====================================
echo.

REM Ensure we're on master branch
echo [0/4] Checking current branch...
for /f "tokens=*" %%i in ('git branch --show-current') do set CURRENT_BRANCH=%%i

if not "%CURRENT_BRANCH%"=="master" (
    echo [INFO] Current branch: %CURRENT_BRANCH%
    echo [INFO] Switching to master branch...
    git checkout master
    if errorlevel 1 (
        echo [ERROR] Failed to switch to master
        echo [INFO] Please switch manually: git checkout master
        pause
        exit /b 1
    )
)
echo [OK] On master branch
echo.

REM Check if there are any changes
echo [1/4] Checking for changes...
git status --short > "%TEMP%\git_status_temp.txt" 2>&1
set CHANGE_COUNT=0
for /f %%i in ('type "%TEMP%\git_status_temp.txt" ^| find /c /v ""') do set CHANGE_COUNT=%%i
del "%TEMP%\git_status_temp.txt" 2>nul

if %CHANGE_COUNT% EQU 0 (
    echo [INFO] No changes to commit.
    echo [INFO] Everything is up to date!
    echo.
    pause
    exit /b 0
)

echo [OK] Found %CHANGE_COUNT% changed file(s)
echo.
echo Changes:
git status --short
echo.

REM Ask for commit message
set "COMMIT_MSG="
set /p "COMMIT_MSG=Enter commit message (or press Enter for default): "

if "!COMMIT_MSG!"=="" (
    set "COMMIT_MSG=chore: Update local changes"
)

echo.
echo [2/4] Adding all changes...
git add .
if errorlevel 1 (
    echo [ERROR] Failed to add changes
    pause
    exit /b 1
)
echo [OK] Changes staged
echo.

echo [3/4] Committing with message: "!COMMIT_MSG!"
git commit -m "!COMMIT_MSG!"
if errorlevel 1 (
    echo [ERROR] Commit failed
    pause
    exit /b 1
)
echo [OK] Committed successfully
echo.

echo [4/4] Pushing to GitHub master...
REM Try to push
git push origin master 2>&1 | find "rejected" >nul
if not errorlevel 1 (
    echo [WARNING] Push rejected. Remote has new changes.
    echo [INFO] Pulling latest changes first...
    git pull origin master --no-rebase
    if errorlevel 1 (
        echo [ERROR] Pull failed. May have conflicts.
        echo [INFO] Please resolve conflicts manually.
        pause
        exit /b 1
    )
    echo [INFO] Trying to push again...
    git push origin master
    if errorlevel 1 (
        echo [ERROR] Push still failed
        pause
        exit /b 1
    )
) else (
    git push origin master
    if errorlevel 1 (
        echo [ERROR] Push failed
        echo [INFO] Try: git pull origin master
        pause
        exit /b 1
    )
)
echo [OK] Pushed successfully
echo.

echo ====================================
echo   SUCCESS! Changes Pushed!
echo ====================================
echo.
echo Latest commits on master:
git log --oneline -5
echo.
echo Your changes are now on GitHub!
echo.
pause
ENDLOCAL
