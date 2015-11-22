using System;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ServiceStack.OrmLite;

namespace someWorld
{
	public class AccountManager
	{
		public ConcurrentDictionary<string, Account> OnlineUsers;

		public AccountManager ()
		{
			OnlineUsers = new ConcurrentDictionary<string, Account>();
		}

		public void Add(Account account)
		{
			OnlineUsers.TryAdd(account.GetUsername(), account);
		}

		public void Remove(Account account)
		{
			Account ignored;
			OnlineUsers.TryRemove(account.GetUsername(), out ignored);
		}

		public void Remove(string username)
		{
			Account ignored;
			OnlineUsers.TryRemove(username, out ignored);
		}

		public async Task<Account> GetAccount(string username)
		{
			Account account;
			if (IsUserOnline(username) && OnlineUsers.TryGetValue(username, out account))
				return account;

			return await Account.FromUsername(username);
		}

		public bool IsUserOnline(string username)
		{
			var acc = GetAccount(username);
			return OnlineUsers.ContainsKey(username);
		}
	}
}

