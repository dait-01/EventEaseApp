Step 2: Copilot made Date nullable so [Required] could work.
Step 3: the missing @using caused the blank /register/99 page, and the fix was adding it. Copilot-generated components need their namespace imported wherever they're used.
Step 4: the before and after DOM counts (about 5000 vs about 10-15) are strong evidence of the optimization.