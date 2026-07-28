using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BOG.SwissArmyKnife;

// Copyright (c) 2009-, John J Schultz, all rights reserved.
// Jan 2022: Upgrade to .NET Framework 4.8

namespace BOG.BulkFileRename
{
	public partial class AboutBox : Form
	{
		WinAppInfo x = new WinAppInfo();

		public AboutBox()
		{
			InitializeComponent();
			var av = new AssemblyVersion(SwissArmyKnife.AssemblyVersion.AssemblySource.Entry);

			x.AssemblyVersion = av.Version;
			x.BuildDate = av.BuildDate.ToShortDateString();
			x.Copyright = AssemblyCopyright;
			x.Description = AssemblyDescription;
			x.FullPath = av.Filename;
			x.Name = av.Name;
			x.Processor = RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD) ? "FreeBSD" : (
						RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" : (
						RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" : "OSX"));
			x.ProductName = AssemblyProduct;
			x.Title = AssemblyTitle;
			x.Version = av.Version;

			this.pgInfo.SelectedObject = x;
		}


		#region Assembly Attribute Accessors

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}
		#endregion

		private void okButton_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void linkHomePage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.linkHomePage.LinkVisited = true;
			var helpfile = (string)this.linkHomePage.Tag;
			try
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					// Windows requires UseShellExecute to launch URIs directly
					Process.Start(new ProcessStartInfo(helpfile) { UseShellExecute = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					// Linux relies on xdg-open to handle default URI schemes
					Process.Start("xdg-open", helpfile);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
				{
					// FreeBSD relies on xdg-open to handle default URI schemes
					Process.Start("xdg-open", helpfile);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					// macOS uses the open command for URIs
					Process.Start("open", helpfile);
				}
				else throw new Exception("Operating system not recognized.");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Unable to open the link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void okButton_Click_1(object sender, EventArgs e)
		{
			this.Close();
		}
	}

	[DefaultPropertyAttribute("Description")]
	class WinAppInfo
	{
		private string _Title;
		private string _ProductName;
		private string _AssemblyVersion;
		private string _Copyright;
		private string _Description;

		private string _BuildDate;
		private string _FullPath;
		private string _Name;
		private string _Processor;
		private string _Version;

		[CategoryAttribute("Admin"), DisplayNameAttribute("Title"), DescriptionAttribute("The title assigned to the project"), ReadOnly(true)]
		public string Title
		{
			get { return _Title; }
			set { _Title = value; }
		}

		[CategoryAttribute("Admin"), DisplayNameAttribute("Product Name"), DescriptionAttribute("The name assigned to the project"), ReadOnly(true)]
		public string ProductName
		{
			get { return _ProductName; }
			set { _ProductName = value; }
		}

		[CategoryAttribute("Admin"), DisplayNameAttribute("Assembly Version"), DescriptionAttribute("The specific build number."), ReadOnly(true)]
		public string AssemblyVersion
		{
			get { return _AssemblyVersion; }
			set { _AssemblyVersion = value; }
		}

		[CategoryAttribute("Admin"), DisplayNameAttribute("Copyright"), DescriptionAttribute(""), ReadOnly(true)]
		public string Copyright
		{
			get { return _Copyright; }
			set { _Copyright = value; }
		}

		[CategoryAttribute("Admin"), DisplayNameAttribute("Description"), DescriptionAttribute(""), ReadOnly(true)]
		public string Description
		{
			get { return _Description; }
			set { _Description = value; }
		}

		[CategoryAttribute("Technical"), DisplayNameAttribute("Build Date"), DescriptionAttribute("Date this version was constructed."), ReadOnly(true)]
		public string BuildDate
		{
			get { return _BuildDate; }
			set { _BuildDate = value; }
		}

		[CategoryAttribute("Technical"), DisplayNameAttribute("Application Location"), DescriptionAttribute("The location of the specific executable running."), ReadOnly(true)]
		public string FullPath
		{
			get { return _FullPath; }
			set { _FullPath = value; }
		}

		[CategoryAttribute("Technical"), DisplayNameAttribute("Application Name"), DescriptionAttribute("The name stored within the executable itself."), ReadOnly(true)]
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		[CategoryAttribute("Technical"), DisplayNameAttribute("Processor"), DescriptionAttribute("Targeted operating platform"), ReadOnly(true)]
		public string Processor
		{
			get { return _Processor; }
			set { _Processor = value; }
		}

		[CategoryAttribute("Technical"), DisplayNameAttribute("Version"), DescriptionAttribute("The file version of the executable."), ReadOnly(true)]
		public string Version
		{
			get { return _Version; }
			set { _Version = value; }
		}
	}
}
