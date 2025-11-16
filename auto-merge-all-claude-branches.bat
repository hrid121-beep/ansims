@echo off
REM Dynamic Auto-Merge Script - Merges ALL Claude branches to Master
REM This will fetch and merge ALL branches starting with "claude/"

echo.
echo ====================================
echo   Merging All Claude Changes
echo ====================================
echo.

echo [Step 1] Switching to master branch...
git checkout master
if errorlevel 1 (
    echo [ERROR] Failed to checkout master.
    pause
    exit /b 1
)
echo [OK] On master branch
echo.

echo [Step 2] Pulling latest master...
git pull origin master
if errorlevel 1 (
    echo [ERROR] Failed to pull master.
    pause
    exit /b 1
)
echo [OK] Master updated
echo.

echo [Step 3] Fetching all remote branches...
git fetch --all
if errorlevel 1 (
    echo [ERROR] Failed to fetch branches.
    pause
    exit /b 1
)
echo [OK] Fetched all branches
echo.

echo [Step 4] Finding Claude branches...
echo.
echo Available Claude branches:
git branch -r | findstr "origin/claude/"
echo.
echo.

REM Merge the latest Claude branch (replace-cdn-local-links)
echo [Step 5] Merging claude/replace-cdn-local-links-018nHTbrDEuHBcioEUhF79jr...
git merge origin/claude/replace-cdn-local-links-018nHTbrDEuHBcioEUhF79jr --no-edit
if errorlevel 1 (
    echo [WARNING] Merge had issues. Check for conflicts.
    echo.
    echo Type 'git status' to see what happened.
    pause
    exit /b 1
)
echo [OK] Merged successfully
echo.

REM Optional: Merge other Claude branches if they exist
echo [Step 6] Checking for other Claude branches...
git branch -r | findstr "origin/claude/pull-request" > nul
if not errorlevel 1 (
    echo Found other Claude PR branches. Merging...
    for /f "tokens=*" %%b in ('git branch -r ^| findstr "origin/claude/pull-request"') do (
        echo   Merging %%b...
        git merge %%b --no-edit
        if errorlevel 1 (
            echo   [WARNING] Could not merge %%b
        ) else (
            echo   [OK] Merged %%b
        )
    )
) else (
    echo No other Claude PR branches found.
)
echo.

echo [Step 7] Pushing to GitHub master...
git push origin master
if errorlevel 1 (
    echo [ERROR] Push failed.
    pause
    exit /b 1
)
echo [OK] Pushed successfully
echo.

echo ====================================
echo   SUCCESS! All changes merged!
echo ====================================
echo.
echo Recent commits on master:
git log --oneline -10
echo.
echo Current branch status:
git status
echo.
pause
