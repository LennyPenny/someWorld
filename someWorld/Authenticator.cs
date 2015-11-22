using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using vtortola.WebSockets;
using RohBot;

using System.Text;

namespace someWorld
{
	public static class Authenticator
	{


		public static async Task<Account> Authenticate(Connection con)
		{
			await con.SendAsync("Please enter your username: (hit enter to register a new account)");
			var username = await con.ReadNextMsgAsync();

			Account account;
			if (username == "") {
				account = await Registrar.Register(con);;
			} else {
				var accounttask = Account.FromUsername(username);

				await con.SendAsync("Enter password: ");

				var pw = await con.ReadNextMsgAsync();

				account = await accounttask;
				var utf8 = new UTF8Encoding();
				if (account.GetPasswordHash() != PasswordSecurity.GenerateSaltedHash(utf8.GetBytes(pw), account.GetSalt())) {
					await con.SendAsync("Wrong password");
					return await Authenticate(con);
				}

			}
			return account;
		}
	}
}

