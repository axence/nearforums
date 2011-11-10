﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NearForums.Configuration;

namespace NearForums.Localization
{
	public class Localizer
	{
		#region Static Methods
		private static object lockCurrentLoad = new object();
		private static Localizer _current;
		public static Localizer Current
		{
			get
			{
				if (_current == null)
				{
					lock (lockCurrentLoad)
					{
						var cultureName = SiteConfiguration.Current.CultureName;
						SetCulture(cultureName, SiteConfiguration.Current.LocalizationFullPath + cultureName + ".po");
					}
				}
				return _current;
			}
		}

		/// <summary>
		/// Loads the translations of a culture and sets as current
		/// </summary>
		/// <param name="cultureName"></param>
		public static void SetCulture(string cultureName, string filePath)
		{
			var manager = new Localizer(cultureName, filePath);
			manager.LoadCulture();

			_current = manager;
		}
		#endregion

		private Dictionary<string, string> _translations;

		/// <summary>
		/// Gets or sets the name of the culture.
		/// </summary>
		public string CultureName 
		{ 
			get; 
			set; 
		}

		public string FilePath
		{
			get;
			set;
		}

		public Localizer()
		{
			_translations = new Dictionary<string, string>();
		}

		public Localizer(string cultureName, string filePath) : this()
		{
			CultureName = cultureName;
			FilePath = filePath;
		}

		public void LoadCulture()
		{
			if (FilePath == null)
			{
				throw new NullReferenceException("FilePath can not be null");
			}
			_translations = LocalizationParser.ParseFile(FilePath);
		}

		public string Get(string neutralValue)
		{
			if (neutralValue == null)
			{
				throw new ArgumentNullException("neutralValue");
			}
			if (_translations.ContainsKey(neutralValue.ToLowerInvariant()))
			{
				return _translations[neutralValue.ToLowerInvariant()];
			}
			return neutralValue;
		}

		public string this[string neutralValue]
		{
			get
			{
				return Get(neutralValue);
			}
		}
	}
}
