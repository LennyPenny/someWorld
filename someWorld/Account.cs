using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Linq;

using ServiceStack.DataAnnotations;
using ServiceStack.Logging;

using ServiceStack.OrmLite;

namespace someWorld
{
	public class AccountData
	{
		[AutoIncrement]
		public int ID { get; set;}

		[Index(Unique = true)]
		public string Username { get; private set;}
		public byte[] PasswordHash { get; private set;}
		public byte[] PasswordSalt { get; private set;}

		public static async Task<AccountData> FromUsername(string username)
		{
			AccountData account;
			using (var db = Program.DbFactory.Open()) {
				account = await db.SingleAsync<AccountData>($"Username = {username}");
			}

			return account;
		}

		public static async Task<AccountData> CreateAccountData(string username, byte[] passwordhash, byte[] passwordsalt)
		{
			var accountdata = new AccountData();
			accountdata.Username = username;
			accountdata.PasswordHash = passwordhash;
			accountdata.PasswordSalt = passwordsalt;

			using (var db = Program.DbFactory.Open()) {
				await db.SaveAsync<AccountData>(accountdata);
			}

			return accountdata;
		}
	}
	public class Account
	{
		private AccountData AccountData;

		private readonly object _sync = new object();
		private List<Connection> connections;

		private Account()
		{
			connections = new List<Connection>();
		}

		public static async Task<Account> FromUsername(string username)
		{
			var account = new Account();
			account.AccountData = await AccountData.FromUsername(username);

			return account;
		}

		public static async Task<Account> CreateAccount(string username, byte[] passwordhash, byte[] passwordsalt)
		{
			var account = new Account();
			account.AccountData = await AccountData.CreateAccountData(username, passwordhash, passwordsalt);

			return account;
		}

		public string GetUsername()
		{
			return AccountData.Username;
		}

		public byte[] GetPasswordHash()
		{
			return AccountData.PasswordHash;
		}

		public byte[] GetSalt()
		{
			return AccountData.PasswordSalt;
		}

		public void AddConnection(Connection con)
		{
			lock (_sync) {
				connections.Add(con);
				con.Account = this;
			}
		}

		public void RemoveConnection(Connection con)
		{
			lock (_sync) {
				connections.Remove(con);
			}
		}

		public bool IsOnline()
		{
			return connections.Any();
		}

		public ReadOnlyCollection<Connection> ReadGetConnections()
		{
			return connections.AsReadOnly();
		}

		public static async Task<bool> IsUsernameTaken(string username)
		{
			using (var db = Program.DbFactory.Open()) {
				return await db.ExistsAsync<AccountData>(accountdata => accountdata.Username == username);
			}
		}
	}
}

