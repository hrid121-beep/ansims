@echo off
REM Font Download Script for Ansar VDP IMS
REM Downloads Roboto and Noto Sans Bengali fonts locally

echo.
echo ========================================
echo   Font Download Helper
echo ========================================
echo.
echo This script will help you download fonts locally.
echo.
echo OPTION 1: Use System Fonts (Already Active)
echo   - No download needed
echo   - Uses Windows/Mac built-in fonts
echo   - Works offline
echo   - RECOMMENDED for most users
echo.
echo OPTION 2: Download Fonts Manually
echo   - Better looking
echo   - Consistent across all devices
echo   - Requires internet
echo.
echo.

choice /C 12 /M "Choose option (1=System Fonts, 2=Download)"

if errorlevel 2 goto download
if errorlevel 1 goto system

:system
echo.
echo [OK] System fonts are already active!
echo No action needed. Your app will use Windows/Mac fonts.
echo.
goto end

:download
echo.
echo ========================================
echo   Downloading Fonts...
echo ========================================
echo.
echo Opening Google Fonts download pages...
echo.
echo [1] Roboto Font:
start https://fonts.google.com/specimen/Roboto
echo    - Click "Download family" button
echo    - Extract to: IMS.Web\wwwroot\fonts\roboto\
echo.
echo [2] Noto Sans Bengali Font:
start https://fonts.google.com/specimen/Noto+Sans+Bengali
echo    - Click "Download family" button
echo    - Extract to: IMS.Web\wwwroot\fonts\noto-sans-bengali\
echo.
echo.
echo After downloading:
echo 1. Extract the .ttf or .woff2 files
echo 2. Put them in the folders mentioned above
echo 3. Edit: IMS.Web\wwwroot\css\custom-fonts.css
echo 4. Uncomment "OPTION 3: Self-Hosted Fonts" section
echo.
goto end

:end
echo.
echo ========================================
echo   Font Setup Instructions
echo ========================================
echo.
echo To activate custom-fonts.css:
echo 1. Open: IMS.Web\Views\Shared\_Layout.cshtml
echo 2. Add this line in the head section:
echo    ^<link rel="stylesheet" href="~/css/custom-fonts.css"^>
echo.
echo Current status: System fonts active (Option 1)
echo.
pause
