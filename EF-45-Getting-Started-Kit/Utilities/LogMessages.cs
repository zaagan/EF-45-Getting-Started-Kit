using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace App.Utilities
{
	class LogMessages
	{
		/// <summary>
		/// Enumerates icons for display alongside messages.
		/// </summary>
		public enum Icon
		{
			Information,
			Warning,
			Error
		}

		public LogMessages(ListView listView)
		{
			m_listView = listView;

			MaximizeMessageColumnWidth();
		}

		/// <summary>
		/// Maximum number of messages that can be displayed at once. Once this limit is reached,
		/// the oldest messages are deleted to make room for new ones.
		/// </summary>
		public int MaxMessages { get { return (m_maxMessages); } set { m_maxMessages = value; } }


		/// <summary>
		/// Add a message to the list.
		/// </summary>
		/// <param name="icon">Icon to display alongside message.</param>
		/// <param name="timeStamp">Time of message.</param>
		/// <param name="message">Message text.</param>
		public void AddMessage(Icon icon, DateTime timeStamp, String message)
		{
			AddMessage(icon, timeStamp, "Application", message);
		}

		/// <summary>
		/// Add a message to the list.
		/// </summary>
		/// <param name="icon">Icon to display alongside message.</param>
		/// <param name="timeStamp">Time of message.</param>
		/// <param name="source">Source of the message.</param>
		/// <param name="message">Message text.</param>
		public void AddMessage(Icon icon, DateTime timeStamp, String source, String message)
		{
			if (m_cacheMessages)
			{
				AddMessageToCache(icon, timeStamp, source, message);
			}
			else
			{
				AddMessageToList(icon, timeStamp, source, message);
			}
		}

		/// <summary>
		/// Clear the display of messages.
		/// </summary>
		public void ClearMessages()
		{
			m_listView.Items.Clear();
		}

		/// <summary>
		/// Cache the messages instead of displaying on the ListView control.
		/// </summary>
		public void CacheMessages()
		{
			if (m_listCache == null)
			{
				m_listCache = new Queue<ListViewItem>();
			}
			m_cacheMessages = true;
		}

		/// <summary>
		/// Clear all the cached messages and add them to the ListView control.
		/// </summary>
		public void FlushCachedMessages()
		{
			while (m_listCache.Count > 0)
			{
				// Remove messages from the display if the count
				// exceeds or is equal to the maximum.
				if (m_listView.Items.Count >= m_maxMessages)
				{
					m_listView.Items.RemoveAt(0);
				}
				m_listView.Items.Add(m_listCache.Dequeue());
			}

			EnsureLastMessageIsVisible();

			m_cacheMessages = false;
		}

		/// <summary>
		/// Add a message to the ListView control.
		/// </summary>
		private void AddMessageToList(Icon icon, DateTime timeStamp, String source, String message)
		{
			// If maximum messages reached, remove old messages to make room
			while (m_listView.Items.Count >= m_maxMessages)
			{
				m_listView.Items.RemoveAt(0);
			}

			// Add new list item to display the message
			ListViewItem messageItem =
				new ListViewItem(new String[] { null, timeStamp.ToString(TimestampFormat), source, message }, (int)icon);
			m_listView.Items.Add(messageItem);

			EnsureLastMessageIsVisible();
		}

		/// <summary>
		/// Ensure the last message is visible.
		/// </summary>
		private void EnsureLastMessageIsVisible()
		{
			// Scroll the display if necessary to ensure the new message is visible
			m_listView.EnsureVisible(m_listView.Items.Count - 1);
		}

		/// <summary>
		/// Add a message to the cache.
		/// </summary>
		private void AddMessageToCache(Icon icon, DateTime timeStamp, String source, String message)
		{
			// Add new list item to display the message
			ListViewItem messageItem =
				new ListViewItem(new String[] { null, timeStamp.ToString(TimestampFormat), source, message }, (int)icon);
			m_listCache.Enqueue(messageItem);
			// If the queue contains more than the maximum number of messages
			// displayed on the screen then remove the oldest message.
			if (m_listCache.Count > m_maxMessages)
			{
				m_listCache.Dequeue();
			}
		}

		private void m_listView_SizeChanged(object sender, EventArgs e)
		{
			MaximizeMessageColumnWidth();
		}

		private void MaximizeMessageColumnWidth()
		{
			m_listView.Columns[3].Width =
				m_listView.Width -
				(m_listView.Columns[0].Width + m_listView.Columns[1].Width + m_listView.Columns[2].Width + ScrollbarWidth);
		}

		private const int ScrollbarWidth = 20;
		private const string TimestampFormat = "T";

		private int m_maxMessages = 100;
		private bool m_cacheMessages = false;
		private Queue<ListViewItem> m_listCache = null;
		private ListView m_listView;
	}
}
