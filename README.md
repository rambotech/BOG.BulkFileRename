# BOG.BulkFileRename

*Migrated to maintained frameworks*

A Windows Form utility app for enabling complex and bulk renaming within a folder.

A Windows Explorer folder extension (to open the app directly for the selected folder) 
can be added / removed using the menu items in the help dropdown.

### Version History
- v2.0.2 -- JJS -- 2026/07/27
  - Features:
      - Added Replace (REGEX) option... e.g.
          - For: ABCDEFG.txt
              - Find:    ABc(d)eFG\.t(.)t
              - Replace: ABc$1eFG.te$2t
              - Result:  ABcdeFG.text 
          - For: 20220913_This_is_my_file.txt.bak
              - Find:    (20[\d]{2})([\d]{2})([\d]{2})_This_is_my_file.txt(\.bak)
              - Replace: 20$1-$2-$3-This_is_my_file.txt
              - Result:  2022-09-13-This_is_my_file.txt
          * The ignore case checkbox will determine if the regex match is case sensitive.
          * Multiline mode is enabled by default, so ^ and $ will match the start and end of each line in the input string.
          Be sure to encode ^ and $ in any filename.
  - Other:
      - Remove registry file templates, replace with static string values.
      - Upgrade to .NET 10.0 and C# 14.0

- v2.0.1 -- JJS -- 2025/06/08
  - Change shell command key name from Bulk_File_Rename to BOG.BulkFileRename
  - NuGet package upgrade

- v2.0.0 -- JJS -- 2025
  - Refactored for VS2022 and .NET Framework 4.8.1
  - Added REG file generation and launch for adding / removing context menu extensions for folder in Windows Explorer.

- v1.0.0 -- JJS -- 2009
  - Original version
  
  