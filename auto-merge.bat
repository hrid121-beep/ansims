@echo off
REM Simple Auto-Merge Script - Claude Changes to Master
REM Just double-click this file whenever Claude makes changes

echo.
echo ====================================
echo   Getting Claude's Latest Changes
echo ====================================
echo.

echo [1/3] Fetching latest changes from Claude branch...
git fetch origin claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn
if errorlevel 1 (
    echo [ERROR] Failed to fetch. Check internet connection.
    pause
    exit /b 1
)
echo [OK] Fetched successfully
echo.

echo [2/3] Merging changes into master...
git merge origin/claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn --no-edit
if errorlevel 1 (
    echo [ERROR] Merge failed. May have conflicts.
    pause
    exit /b 1
)
echo [OK] Merged successfully
echo.

echo [3/3] Pushing to GitHub master...
git push origin master
if errorlevel 1 (
    echo [ERROR] Push failed.
    pause
    exit /b 1
)
echo [OK] Pushed successfully
echo.

echo ====================================
echo   SUCCESS! All changes updated!
echo ====================================
echo.
echo Latest changes:
git log --oneline -5
echo.
pause
