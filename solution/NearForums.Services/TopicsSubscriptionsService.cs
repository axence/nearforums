﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NearForums.DataAccess;
using NearForums.Configuration;

namespace NearForums.Services
{
	public class TopicsSubscriptionsService : ITopicsSubscriptionsService
	{
		/// <summary>
		/// Topic subscriptions repository
		/// </summary>
		private readonly ITopicsSubscriptionsDataAccess _dataAccess;

		/// <summary>
		/// Service to do the notifications
		/// </summary>
		private readonly INotificationsService _notificationService;

		public TopicsSubscriptionsService(ITopicsSubscriptionsDataAccess da, INotificationsService notifications)
		{
			_dataAccess = da;
			_notificationService = notifications;
		}

		public void Add(int topicId, int userId)
		{
			_dataAccess.Add(topicId, userId);
		}

		public int Remove(int topicId, int userId, Guid userGuid)
		{
			return _dataAccess.Remove(topicId, userId, userGuid);
		}

		public List<User> GetSubscribed(int topicId)
		{
			return _dataAccess.GetUsersByTopic(topicId);
		}

		public List<Topic> GetTopics(int userId)
		{
			return _dataAccess.GetTopicsByUser(userId);
		}

		public void SendNotifications(Topic topic, int userId, string url, string unsubscribeUrl)
		{
			if (SiteConfiguration.Current.Notifications != null && SiteConfiguration.Current.Notifications.Subscription != null)
			{
				string body = SiteConfiguration.Current.Notifications.Subscription.Body.ToString();
				var users = GetSubscribed(topic.Id);
				users.RemoveAll(x => x.Id == userId || String.IsNullOrEmpty(x.Email));

				var handler = new SendNotificationsHandler(_notificationService.SendToUsersSubscribed);
				handler.BeginInvoke(topic, users, body, url, unsubscribeUrl, true, null, null);
			}
		}
	}

	public delegate int SendNotificationsHandler(Topic topic, List<User> users, string body, string url, string unsubscribeUrl, bool handleExceptions);
}