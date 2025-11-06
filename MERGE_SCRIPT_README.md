# Auto Merge Script - Claude Branch to Master

এই script গুলো automatically আপনার claude branch থেকে local master এ merge করে এবং remote master এ push করে দেয়।

## 📋 কি করে এই Script?

1. ✅ Current branch save করে
2. ✅ Master branch এ switch করে
3. ✅ Remote master থেকে latest changes pull করে
4. ✅ Claude branch fetch করে
5. ✅ Claude branch কে master এ merge করে
6. ✅ Master branch remote এ push করে
7. ✅ আগের branch এ ফিরে যায়

## 🚀 কিভাবে Use করবেন

### Linux/Mac (Bash):

```bash
# Script কে executable করুন (শুধু প্রথমবার)
chmod +x merge-to-master.sh

# Script run করুন
./merge-to-master.sh
```

### Windows (PowerShell):

```powershell
# PowerShell script run করুন
.\merge-to-master.ps1
```

অথবা Git Bash থেকে:

```bash
bash merge-to-master.sh
```

## ⚠️ Important Notes

### Script চালানোর আগে:

1. **সব changes commit করে নিন** - কোন uncommitted changes থাকলে script error দেবে
2. **Internet connection চেক করুন** - Remote থেকে pull/push করতে হবে
3. **Conflicts থাকলে** - Script বলে দেবে কি করতে হবে

### Script Run করার পর:

- Master branch এ সব changes merge হয়ে যাবে
- Remote master এও push হয়ে যাবে
- আপনি যে branch এ ছিলেন সেখানেই ফিরে আসবেন

## 🎯 Use Case Examples

### Scenario 1: Claude branch এ কাজ করছেন, master এ merge করতে চান

```bash
# Claude branch এ আছেন
git checkout claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn

# Changes commit করুন
git add .
git commit -m "feat: New feature added"
git push

# Script run করুন (automatically master এ merge হবে)
./merge-to-master.sh

# আবার claude branch এ ফিরে আসবেন automatically
```

### Scenario 2: অন্য কোন branch থেকে master update করতে চান

```bash
# যেকোন branch থেকে
./merge-to-master.sh

# Script automatically:
# 1. Master এ যাবে
# 2. Claude branch merge করবে
# 3. Push করবে
# 4. আপনার branch এ ফিরে আসবে
```

## 🔧 Troubleshooting

### Error: "You have uncommitted changes"

**সমাধান:**
```bash
# Option 1: Changes commit করুন
git add .
git commit -m "Your message"

# Option 2: Changes stash করুন
git stash
./merge-to-master.sh
git stash pop
```

### Error: "Merge conflict detected"

**সমাধান:**
```bash
# Conflicts manually resolve করুন
git status  # Conflicted files দেখুন

# Files edit করে conflicts fix করুন
# তারপর:
git add .
git commit -m "Merge claude branch"
git push origin master
```

### Error: "Failed to push to origin/master"

**সমাধান:**
```bash
# Remote এ নতুন changes থাকলে
git pull origin master --no-rebase
git push origin master
```

## 📝 Script Customization

যদি claude branch এর নাম পরিবর্তন হয়, script edit করুন:

**Bash script (merge-to-master.sh):**
```bash
# Line 11 এ branch name পরিবর্তন করুন
CLAUDE_BRANCH="your-new-branch-name"
```

**PowerShell script (merge-to-master.ps1):**
```powershell
# Line 5 এ branch name পরিবর্তন করুন
$CLAUDE_BRANCH = "your-new-branch-name"
```

## 🎉 Success Output Example

```
=== Starting Auto Merge Process ===

Current branch: claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn

[1/6] Switching to master branch...
Switched to branch 'master'

[2/6] Pulling latest changes from origin/master...
Already up to date.

[3/6] Fetching latest from claude branch...
From http://...
 * branch            claude/... -> FETCH_HEAD

[4/6] Merging origin/claude/... into master...
Updating abc123..def456
Fast-forward
 3 files changed, 150 insertions(+), 10 deletions(-)

[5/6] Changes to be pushed:
def456 feat: Add new feature
abc123 fix: Bug fix

[6/6] Pushing to origin/master...
To http://...
   abc123..def456  master -> master

Returning to original branch: claude/...

=== ✅ Successfully merged and pushed to master! ===

All done! 🎉
```

## 💡 Tips

1. **Regular basis এ run করুন** - Master branch up-to-date রাখতে
2. **Git GUI tools এর সাথে use করতে পারেন** - SourceTree, GitKraken, etc.
3. **CI/CD pipeline এ add করতে পারেন** - Automated deployment এর জন্য

---

**Created by:** Claude Code
**Date:** November 5, 2025
**Version:** 1.0
