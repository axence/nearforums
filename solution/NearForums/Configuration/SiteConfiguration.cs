﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using NearForums.Configuration.Notifications;
using System.IO;
using System.Xml;

namespace NearForums.Configuration
{
	public class SiteConfiguration : ConfigurationSection
	{
		#region Current
		private static object lockCurrentLoad = new object();
		private static SiteConfiguration config;
		/// <summary>
		/// Gets the site configuration from the configuration section "site". If applies overrides configuration with admin settings.
		/// </summary>
		public static SiteConfiguration Current
		{
			get
			{
				if (config == null)
				{
					lock (lockCurrentLoad)
					{
						config = (SiteConfiguration)ConfigurationManager.GetSection("site");
						if (config == null)
						{
							throw new System.Configuration.ConfigurationErrorsException("Siteconfiguration not set.");
						}
						if (config.UseSettings)
						{
							config.LoadSettings();
						}
					}
				}
				return config;
			}
		}
		#endregion

		[ConfigurationProperty("ui", IsRequired = true)]
		public UIElement UI
		{
			get
			{
				return (UIElement)this["ui"];
			}
			set
			{
				this["ui"] = value;
			}
		}

		[ConfigurationProperty("authorizationProviders", IsRequired = true)]
		public AuthorizationProvidersElement AuthorizationProviders
		{
			get
			{
				return (AuthorizationProvidersElement)this["authorizationProviders"];
			}
			set
			{
				this["authorizationProviders"] = value;
			}
		}

		[ConfigurationProperty("replacements", IsRequired = false)]
		public ReplacementCollection Replacements
		{
			get
			{
				return (ReplacementCollection)this["replacements"];
			}
			set
			{
				this["replacements"] = value;
			}
		}

		[ConfigurationProperty("dataAccess", IsRequired = false)]
		public DataAccessElement DataAccess
		{
			get
			{
				return (DataAccessElement)this["dataAccess"];
			}
			set
			{
				this["dataAccess"] = value;
			}
		}

		[ConfigurationProperty("notifications", IsRequired = false)]
		public NotificationsContainerElement Notifications
		{
			get
			{
				return (NotificationsContainerElement)this["notifications"];
			}
			set
			{
				this["notifications"] = value;
			}
		}

		[ConfigurationProperty("spamPrevention", IsRequired = true)]
		public SpamPreventionElement SpamPrevention
		{
			get
			{
				return (SpamPreventionElement)this["spamPrevention"];
			}
			set
			{
				this["spamPrevention"] = value;
			}
		}

		#region Timezone
		[ConfigurationProperty("timeZoneOffset", IsRequired = false)]
		private decimal? TimeZoneOffsetHours
		{
			get
			{
				return (decimal?)this["timeZoneOffset"];
			}
			set
			{
				this["timeZoneOffset"] = value;
			}
		}

		public TimeSpan? TimeZoneOffset
		{
			get
			{
				if (this.TimeZoneOffsetHours == null)
				{
					return null;
				}
				return new TimeSpan(0, Convert.ToInt32(this.TimeZoneOffsetHours * 60), 0);
			}
		} 
		#endregion

		/// <summary>
		/// Determines if the application should store and retrieve admin settings, apart from the site configuration
		/// </summary>
		[ConfigurationProperty("useSettings", IsRequired = false, DefaultValue=true)]
		private bool UseSettings
		{
			get
			{
				return (bool)this["useSettings"];
			}
			set
			{
				this["useSettings"] = value;
			}
		}

		[ConfigurationProperty("settingsSource", IsRequired = false, DefaultValue = @"content\settings.xml")]
		public string SettingsSource
		{
			get
			{
				return (string)this["settingsSource"];
			}
			set
			{
				this["settingsSource"] = value;
			}
		}

		/// <summary>
		/// Determines the path of localization files, relative to executing path. Example: content\localization\
		/// </summary>
		[ConfigurationProperty("localizationFolder", IsRequired = false, DefaultValue = @"content\localization\")]
		public string LocalizationFolder
		{
			get
			{
				return (string)this["localizationFolder"];
			}
			set
			{
				this["localizationFolder"] = value;
			}
		}

		/// <summary>
		/// Gets the path on disk of the localization files. Example: c:\whatever\content\localization\
		/// </summary>
		public string LocalizationFullPath
		{
			get
			{
				return CombinePath(LocalizationFolder);
			}
		}

		/// <summary>
		/// Determines the path of localization files, relative to executing path. Example: content\localization\
		/// </summary>
		[ConfigurationProperty("cultureName", IsRequired = false, DefaultValue = "en-US")]
		public string CultureName
		{
			get
			{
				var cultureName = (string)this["cultureName"];
				if (String.IsNullOrWhiteSpace(cultureName))
				{
					cultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
				}
				return cultureName;
			}
			set
			{
				this["cultureName"] = value;
			}
		}

		public override bool IsReadOnly()
		{
			return false;
		}

		protected override void PostDeserialize()
		{
			base.PostDeserialize();

			ProcessMissingElements(this);
		}

		#region Load settings
		/// <summary>
		/// Loads settings by overriding configuration with admin settings.
		/// </summary>
		protected virtual void LoadSettings()
		{
			var settingsFileName = CombinePath(SettingsSource);
			try
			{
				using (var settingsFile = File.Open(settingsFileName, FileMode.Open, FileAccess.Read))
				{
					using (var reader = new XmlTextReader(settingsFile))
					{
						reader.Read();
						DeserializeElement(reader, false);
					}
				}
			}
			catch (DirectoryNotFoundException) { }
			catch (FileNotFoundException) { }
			//Its OK if there isn't settings
		}

		/// <summary>
		/// Combines the current application configuration path with the fileName given
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public string CombinePath(string fileName)
		{
			return Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile), fileName);
		} 
		#endregion

		#region Save settings
		/// <summary>
		/// Saves current settings.
		/// </summary>
		public void SaveSettings()
		{ 
			var settingsFileName = CombinePath(SettingsSource);
			var writer = XmlTextWriter.Create(settingsFileName, new XmlWriterSettings() 
			{ 
				ConformanceLevel = ConformanceLevel.Fragment, 
				Encoding = Encoding.UTF8,
				Indent = true
			});
			writer.WriteStartElement("site");
			try
			{
				SerializeElement(writer, false);
				writer.WriteEndElement();
			}
			finally
			{
				writer.Close();
			}
		}
		#endregion

		#region Handle missing elements
		private void ProcessMissingElements(ConfigurationElement element)
		{

			foreach (PropertyInformation propertyInformation in element.ElementInformation.Properties)
			{

				var complexProperty = propertyInformation.Value as ConfigurationElement;

				if (complexProperty != null)
				{
					if (propertyInformation.IsRequired && !complexProperty.ElementInformation.IsPresent)
					{
						throw new ConfigurationErrorsException("Configuration element: [" + propertyInformation.Name + "] is required but not present");
					}
					else
					{
						ProcessMissingElements(complexProperty);
					}
				}
			}
		} 
		#endregion
	}
}
