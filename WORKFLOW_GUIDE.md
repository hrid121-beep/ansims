# 📘 Complete Git Workflow Guide

এই guide এ ২টা scenario আছে:
1. **যখন Claude changes করবে** - আপনি কিভাবে পাবেন
2. **যখন আপনি changes করবেন** - কিভাবে push করবেন

---

## 🤖 Scenario 1: Claude যখন Changes করবে

### Claude বলবে:
```
"আমি নতুন feature add করেছি এবং claude branch এ push করেছি।
এখন auto-merge.bat run করুন।"
```

### আপনি করবেন:

**Option A: Double-Click (সবচেয়ে সহজ)**
```
📁 Project folder এ যান
📜 auto-merge.bat double-click করুন
✅ Done!
```

**Option B: PowerShell Command**
```powershell
cd "E:\Github Projects\zzzSir\ANSAR VDP\IMS"
git fetch origin claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn
git merge origin/claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn --no-edit
git push origin master
```

**Option C: এক লাইনে**
```powershell
git fetch origin claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn; git merge origin/claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn --no-edit; git push origin master
```

### Result:
- ✅ Claude এর সব changes আপনার local master এ আসবে
- ✅ Automatic GitHub master এও push হবে
- ✅ সব up-to-date!

---

## 👨‍💻 Scenario 2: আপনি যখন Local এ Changes করবেন

### Example: আপনি একটা file edit করলেন

**Option A: Batch File (সবচেয়ে সহজ)**
```
📜 commit-and-push.bat double-click করুন
⌨️ Commit message টাইপ করুন (অথবা Enter চাপুন default এর জন্য)
✅ Done! Automatic commit এবং push হবে
```

**Option B: Manual PowerShell Commands**
```powershell
# Step 1: দেখুন কি কি changes হয়েছে
git status

# Step 2: সব changes add করুন
git add .

# Step 3: Commit করুন
git commit -m "fix: Bug fix in Store controller"

# Step 4: GitHub master এ push করুন
git push origin master
```

### Commit Message Examples:
```
feat: Add new feature for inventory tracking
fix: Fix issue in stock calculation
update: Update validation logic
docs: Add documentation for API
chore: Clean up unused files
```

---

## 🔄 Complete Workflow Example

### সকালে Office এ আসার পর:

**1. Check if Claude made any changes:**
```powershell
# PowerShell এ
git fetch origin claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn
git log HEAD..origin/claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn --oneline
```

যদি কোন changes দেখায়:
```
auto-merge.bat run করুন
```

**2. আপনার কাজ করুন:**
```
- Files edit করুন
- Code লিখুন
- Test করুন
```

**3. কাজ শেষে commit এবং push:**
```
commit-and-push.bat run করুন
```

---

## 📊 Workflow Diagram

```
┌─────────────────────────────────────────────┐
│  Claude Makes Changes                       │
│  └─> Pushes to claude branch               │
└────────────────┬────────────────────────────┘
                 │
                 ▼
         ┌───────────────┐
         │  You Run:     │
         │ auto-merge.bat│
         └───────┬───────┘
                 │
                 ▼
┌────────────────────────────────────────────┐
│  Your Local Master Updated                 │
│  GitHub Master Updated                     │
└────────────────────────────────────────────┘


┌─────────────────────────────────────────────┐
│  You Make Changes Locally                   │
│  └─> Edit files, add features, fix bugs    │
└────────────────┬────────────────────────────┘
                 │
                 ▼
         ┌──────────────────┐
         │  You Run:        │
         │commit-and-push.bat│
         └────────┬──────────┘
                 │
                 ▼
┌────────────────────────────────────────────┐
│  Changes Committed to Local Master         │
│  Pushed to GitHub Master                   │
└────────────────────────────────────────────┘
```

---

## 🎯 Quick Reference Card

| Situation | What to Run | What Happens |
|-----------|-------------|--------------|
| Claude করেছে changes | `auto-merge.bat` | Claude এর changes আপনার কাছে আসবে + GitHub update |
| আপনি করেছেন changes | `commit-and-push.bat` | আপনার changes GitHub এ push হবে |
| দেখতে চান কি changes আছে | `git status` | Current changes দেখাবে |
| Latest commits দেখতে চান | `git log --oneline -10` | শেষ ১০টা commits দেখাবে |

---

## 🆘 Troubleshooting

### Problem 1: "Push failed" Error

**কারণ:** GitHub এ নতুন changes আছে যা আপনার local এ নেই

**সমাধান:**
```powershell
git pull origin master
git push origin master
```

### Problem 2: "Merge conflict" Error

**কারণ:** Same file এ আপনি এবং Claude উভয়ে changes করেছেন

**সমাধান:**
```powershell
# 1. Conflict আছে এমন files দেখুন
git status

# 2. সেই files manually edit করুন
# 3. Conflict markers (<<<<, ====, >>>>) remove করুন
# 4. তারপর:

git add .
git commit -m "Resolved merge conflicts"
git push origin master
```

### Problem 3: "Uncommitted changes" Error

**কারণ:** আপনার local এ uncommitted changes আছে

**সমাধান:**

**Option A: Changes commit করুন**
```powershell
git add .
git commit -m "WIP: Work in progress"
```

**Option B: Changes stash করুন (temporarily save)**
```powershell
git stash
# ... do your merge/pull
git stash pop  # পরে আবার changes ফিরে আনুন
```

---

## 💡 Pro Tips

### Tip 1: Regular Updates
প্রতিদিন office এ আসার পর `auto-merge.bat` run করুন - latest changes পাবেন

### Tip 2: Commit Often
ছোট ছোট changes করার পর commit করুন - easier to track

### Tip 3: Meaningful Messages
Commit message এ clear লিখুন কি করেছেন:
```
✅ Good: "fix: Fix null reference error in StockService"
❌ Bad:  "update"
```

### Tip 4: Check Before Push
Push করার আগে একবার `git status` দেখে নিন

### Tip 5: Backup Important Work
বড় changes করার আগে একটা commit করে রাখুন

---

## 🎓 Git Commands Cheat Sheet

```powershell
# See current status
git status

# See what changed in files
git diff

# See commit history
git log --oneline -10

# See which branch you're on
git branch

# Undo last commit (keep changes)
git reset --soft HEAD~1

# Discard all local changes (CAREFUL!)
git reset --hard HEAD

# Create a backup branch
git branch backup-$(date +%Y%m%d)
```

---

## 📞 Need Help?

যদি কোন সমস্যা হয়:

1. ✅ এই guide check করুন
2. ✅ Error message copy করুন
3. ✅ Claude কে জিজ্ঞেস করুন

---

**Remember:**
- 🤖 Claude এর changes = `auto-merge.bat`
- 👨‍💻 আপনার changes = `commit-and-push.bat`
- 📝 সব master branch এই হবে
- ✅ Simple and Easy!

---

**Created:** November 2025
**Last Updated:** November 6, 2025
