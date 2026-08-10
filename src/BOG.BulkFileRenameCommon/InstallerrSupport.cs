namespace BOG.BulkFileRename.Common
{
	public static class InstallerSupport
	{
		#region Registry Templates
		public static string GetRegistryTemplateForInstall()
		{
			return
		@"Windows Registry Editor Version 5.00

[HKEY_CLASSES_ROOT\Directory\shell\BOG.BulkFileRename]
@=""Bulk File Rename""

[HKEY_CLASSES_ROOT\Directory\shell\BOG.BulkFileRename\command]
@=""\""[{[PATH]}]\\BOG.BulkFileRename.exe\"" \""%1\""""

[-HKEY_CLASSES_ROOT\Directory\shell\Bulk_File_Rename]

";
		}

		public static string GetRegistryTemplateForRemoval()
		{
			return
		@"Windows Registry Editor Version 5.00

[-HKEY_CLASSES_ROOT\Directory\shell\BOG.BulkFileRename]

[-HKEY_CLASSES_ROOT\Directory\shell\Bulk_File_Rename]
";
		}


		#endregion
	}
}
