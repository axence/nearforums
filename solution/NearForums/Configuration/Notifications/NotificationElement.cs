﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NearForums.Configuration.Notifications
{
	public class NotificationElement : ConfigurationElement, IOptionalElement
	{
		[ConfigurationProperty("body", IsRequired = false)]
		public CDataElement Body
		{
			get
			{
				return (CDataElement)this["body"];
			}
			set
			{
				this["body"] = value;
			}
		}

		/// <summary>
		/// Determines if the provider required data has been defined.
		/// </summary>
		public bool IsDefined
		{
			get
			{
				return (!String.IsNullOrEmpty(this.Body.ToString()));
			}
		}
	}
}
