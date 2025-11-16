# Fonts Setup Guide

## Current Configuration

The application currently uses **modern system fonts** for the best performance and offline capability.

## Font Options

### Option 1: System Fonts (✅ Currently Active)

**Pros:**
- No downloads required
- Works offline
- Fast loading
- Uses native OS fonts (Segoe UI on Windows, San Francisco on Mac)
- Bengali support via system fonts (Vrinda, Kalpurush)

**Cons:**
- May look slightly different on different devices

### Option 2: Google Fonts CDN

**Pros:**
- Consistent look across all devices
- Professional web fonts (Roboto, Noto Sans Bengali)
- Easy to enable

**Cons:**
- Requires internet connection
- External dependency

**To Enable:**
1. Open `wwwroot/css/custom-fonts.css`
2. Find section: `OPTION 2: Google Fonts`
3. Uncomment the `@import` line

### Option 3: Self-Hosted Fonts (Download Locally)

**Pros:**
- Best of both worlds
- Consistent look + offline capability
- No external dependencies
- Full control

**Cons:**
- Requires manual download

**To Enable:**
1. Download fonts from Google Fonts:
   - [Roboto](https://fonts.google.com/specimen/Roboto)
   - [Noto Sans Bengali](https://fonts.google.com/specimen/Noto+Sans+Bengali)

2. Extract font files to:
   ```
   wwwroot/fonts/roboto/roboto-regular.woff2
   wwwroot/fonts/roboto/roboto-bold.woff2
   wwwroot/fonts/noto-sans-bengali/noto-sans-bengali-regular.woff2
   wwwroot/fonts/noto-sans-bengali/noto-sans-bengali-bold.woff2
   ```

3. Open `wwwroot/css/custom-fonts.css`
4. Find section: `OPTION 3: Self-Hosted Fonts`
5. Uncomment the `@font-face` declarations and `:root` variables

## Quick Setup

Run the helper script:
```bash
download-fonts.bat
```

This will guide you through font setup options.

## Font Families Used

- **English:** Roboto, System UI fonts
- **Bengali:** Noto Sans Bengali, Kalpurush, Vrinda
- **Code:** Consolas, Monaco, Courier New

## File Structure

```
wwwroot/fonts/
├── README.md (this file)
├── roboto/
│   ├── roboto-regular.woff2
│   └── roboto-bold.woff2
├── noto-sans-bengali/
│   ├── noto-sans-bengali-regular.woff2
│   └── noto-sans-bengali-bold.woff2
└── source-sans-pro/
    └── (optional)
```

## CSS File

All font configurations are in:
```
wwwroot/css/custom-fonts.css
```

## Testing

After setup, verify fonts are loading:
1. Open browser DevTools (F12)
2. Go to Network tab
3. Filter by "Font" or "woff2"
4. Reload page
5. Check if fonts are loading

## Recommended Setup

For production: **Option 3 (Self-Hosted)**
For development: **Option 1 (System Fonts)** ✅ Current
For testing: **Option 2 (Google CDN)**
