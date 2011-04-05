﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NearForums.Configuration
{
	/// <summary>
	/// Represents a configuration element for Forms Authentication
	/// </summary>
	public class FormsAuthElement : ConfigurationElement, IOptionalElement
	{
		private bool _isDefined;

		[ConfigurationProperty("defined", IsRequired = true)]
		public bool Identifier
		{
			get
			{
				return (bool)this["defined"];
			}
			set
			{
				this["defined"] = value;
			}
		}

		/// <summary>
		/// Determines if the provider required data has been defined.
		/// </summary>
		public bool IsDefined
		{
			get
			{
				return this.Identifier && this.IsFormsAuthDefined;
			}
		}

		public bool IsFormsAuthDefined
		{
			get
			{
				return this._isDefined;
			}
			set
			{
				this._isDefined = value;
			}
		}
	}
}