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

namespace someWorld
{
	public static class Registrar
	{
		private static async Task<string> ReceiveValidUsername(Connection con)
		{
			await con.SendAsync("Please enter your desired username (a-Z, 0-9, _): ");
			var username = await con.ReadNextMsgAsync();

			if (await Account.IsUsernameTaken(username)) {
				await con.SendAsync("That name is already taken, please choose another one");
				return await ReceiveValidUsername(con);
			}

			return username;
		}

		private static async Task<string> ReceiveValidPassword(Connection con)
		{
			await con.SendAsync("Enter your desired password: ");
			var pw1 = await con.ReadNextMsgAsync();
			await con.SendAsync("Enter it again: ");
			var pw2 = await con.ReadNextMsgAsync();

			if (pw1 != pw2) {
				await con.SendAsync("Passwords don't match, try again ");
				return await ReceiveValidPassword(con);
			}

			return pw1;
		}

		public static async Task<Account> Register(Connection con)
		{
			var username = await ReceiveValidUsername(con);
			var password = await ReceiveValidPassword(con);

			var utf8 = new UTF8Encoding();
			return await Account.CreateAccount(username, utf8.GetBytes(password), PasswordSecurity.GenerateRandomSalt());
		}
	}
}

