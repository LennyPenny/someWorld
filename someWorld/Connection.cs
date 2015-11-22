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
	public class Connection : WebSocketClient
	{
		public Account Account { get; set;}

		protected override async void OnOpen()
		{
			try {
				var account = await Authenticator.Authenticate(this);
				account.AddConnection(this);

				while (IsConnected) {
					await OnMessage(await ReadNextMsgAsync());
				}
			}
			catch(InvalidOperationException e) {
				
			}
		}

		protected async Task OnMessage(string msg)
		{
			var username = Account.GetUsername();
			await SendAsync($"{username}: {msg}");
		}

		protected override void OnClose()
		{
			Account.RemoveConnection(this);
		}

		protected override void OnError(Exception e)
		{
			Console.WriteLine("error "+e.Message);
		}
	}
}

