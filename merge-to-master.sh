#!/bin/bash

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${YELLOW}=== Starting Auto Merge Process ===${NC}\n"

# Get the claude branch name
CLAUDE_BRANCH="claude/fix-store-details-date-binding-011CUpprfRX4axs5U6BtPUSn"

# Save current branch
CURRENT_BRANCH=$(git branch --show-current)
echo -e "${GREEN}Current branch: $CURRENT_BRANCH${NC}"

# Check if there are uncommitted changes
if [[ -n $(git status -s) ]]; then
    echo -e "${RED}Error: You have uncommitted changes. Please commit or stash them first.${NC}"
    git status -s
    exit 1
fi

# Step 1: Checkout master
echo -e "\n${YELLOW}[1/6] Switching to master branch...${NC}"
git checkout master
if [ $? -ne 0 ]; then
    echo -e "${RED}Error: Failed to checkout master${NC}"
    exit 1
fi

# Step 2: Pull latest from remote master
echo -e "\n${YELLOW}[2/6] Pulling latest changes from origin/master...${NC}"
git pull origin master --no-rebase
if [ $? -ne 0 ]; then
    echo -e "${RED}Error: Failed to pull from origin/master${NC}"
    echo -e "${YELLOW}Aborting and returning to $CURRENT_BRANCH${NC}"
    git checkout $CURRENT_BRANCH
    exit 1
fi

# Step 3: Fetch latest claude branch
echo -e "\n${YELLOW}[3/6] Fetching latest from claude branch...${NC}"
git fetch origin $CLAUDE_BRANCH
if [ $? -ne 0 ]; then
    echo -e "${RED}Error: Failed to fetch claude branch${NC}"
    exit 1
fi

# Step 4: Merge claude branch into master
echo -e "\n${YELLOW}[4/6] Merging origin/$CLAUDE_BRANCH into master...${NC}"
git merge origin/$CLAUDE_BRANCH --no-edit
if [ $? -ne 0 ]; then
    echo -e "${RED}Error: Merge conflict detected!${NC}"
    echo -e "${YELLOW}Please resolve conflicts manually and then run:${NC}"
    echo -e "  git add ."
    echo -e "  git commit -m 'Merge claude branch'"
    echo -e "  git push origin master"
    exit 1
fi

# Step 5: Show what will be pushed
echo -e "\n${YELLOW}[5/6] Changes to be pushed:${NC}"
git log origin/master..HEAD --oneline

# Step 6: Push to remote master
echo -e "\n${YELLOW}[6/6] Pushing to origin/master...${NC}"
git push origin master
if [ $? -ne 0 ]; then
    echo -e "${RED}Error: Failed to push to origin/master${NC}"
    echo -e "${YELLOW}You may need to push manually or resolve conflicts${NC}"
    exit 1
fi

# Return to original branch if it wasn't master
if [ "$CURRENT_BRANCH" != "master" ]; then
    echo -e "\n${YELLOW}Returning to original branch: $CURRENT_BRANCH${NC}"
    git checkout $CURRENT_BRANCH
fi

echo -e "\n${GREEN}=== ✅ Successfully merged and pushed to master! ===${NC}\n"

# Show final status
echo -e "${GREEN}Final status:${NC}"
git log --oneline -3

echo -e "\n${GREEN}All done! 🎉${NC}"
