using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace app {
    public class SocketHandler {
        WebSocket _socket;
        SocketHandler(WebSocket socket)
        {
            _socket = socket;
        }
        void WriteJsonToStream(Stream stream, object data)
        {
            using (var streamWriter = new StreamWriter(stream))
            using (var writer = new JsonTextWriter(streamWriter))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, data);
            }
        }
        async Task SendTextJson(object data)
        {
            using (var stream = new MemoryStream())
            {
                WriteJsonToStream(stream, data);
                var outgoing = new ArraySegment<byte>(stream.ToArray());
                await _socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        async Task HandleMessage(string creditCardNbr, int attempts)
        {
            Console.Write(".");
            dynamic data = new { 
                creditCardNbr = creditCardNbr, 
                attempts = attempts,
            };
            await SendTextJson(data);
        }
        async Task WorkLoop()
        {
            Console.WriteLine("---> Starting WorkLoop`");

            var rand = new Random();
            while(_socket.State == WebSocketState.Open){
                var nbr = rand.Next(1, 10000);
                var creditCardNbr = String.Format( "a-{0:D6}-cc", nbr);
                var attempts = rand.Next(3,100);
                await HandleMessage(creditCardNbr, attempts);
                Thread.Sleep(rand.Next(10000));
            }
            
            Console.WriteLine("---> Ending WorkLoop`");
        }
        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;
            
            Console.WriteLine("---> Creating WebSocket");

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new SocketHandler(socket);
            await h.WorkLoop();
        }
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(SocketHandler.Acceptor);
        }
    }
}