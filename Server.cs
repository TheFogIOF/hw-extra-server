using HWExtra_Server.Models;
using HWExtra_Server.Requests;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace HWExtra_Server
{
    public class Server
    {
        private readonly HttpListener _listener;
        private readonly Dictionary<string, ClientModel> _clients = new();

        public Server(string prefix)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
        }

        public async Task Start()
        {
            _listener.Start();
            await Listen();
        }

        public void Stop() => _listener.Stop();

        private async Task Listen()
        {
            while (_listener.IsListening)
            {
                var context = await _listener.GetContextAsync();
                await HandleRequest(context);
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                string? path = context.Request.Url?.AbsolutePath;

                //ConsoleExt.WriteLine($"{path}", ConsoleColor.Green);

                if (context.Request.HttpMethod == "POST")
                {
                    using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
                    string body = await reader.ReadToEndAsync();

                    if (path == "/friends/join")
                    {
                        var request = JsonSerializer.Deserialize<JoinRequest>(body);

                        if (request?.Server == null || !request.Server.Contains("holyworld")) await SendResponse(context, 404, "{\"error\":\"Fake Request\"}");

                        if (request?.Username != null && request?.FriendNames != null)
                        {
                            lock (_clients)
                            {
                                _clients[request.Username] = new ClientModel
                                {
                                    Username = request.Username,
                                    Server = request.Server,
                                    FriendNames = new List<string>(request.FriendNames),
                                    LastSeen = DateTime.UtcNow
                                };
                            }

                            ConsoleExt.Write("[", ConsoleColor.DarkGray);
                            ConsoleExt.Write("+", ConsoleColor.Green);
                            ConsoleExt.Write("]", ConsoleColor.DarkGray);
                            Console.Write($" {request.Username} connected.");
                            Console.WriteLine();

                            await FriendUpdateResponse(context, request.Username);
                        }
                        else await SendResponse(context, 404, "{\"error\":\"Fake Request\"}");
                        
                    }
                    else if (path == "/friends/get")
                    {
                        var request = JsonSerializer.Deserialize<GetRequest>(body);

                        if (request != null && request.Username != null)
                        {
                            lock (_clients)
                            {
                                if (_clients.TryGetValue(request.Username, out var data))
                                {
                                    data.FriendNames = new List<string>(request.FriendNames);
                                    data.Server = request.Server;
                                    data.LastSeen = DateTime.UtcNow;
                                }
                                else
                                {
                                    _ = SendResponse(context, 404, "{\"error\":\"User Not Joined\"}");
                                }
                            }

                            await FriendUpdateResponse(context, request.Username);
                        }
                        else await SendResponse(context, 404, "{\"error\":\"Fake Request\"}");
                    }
                    else if (path == "/friends/leave")
                    {
                        var request = JsonSerializer.Deserialize<LeaveRequest>(body);

                        if (request != null && request.Username != null)
                        {
                            if (_clients.ContainsKey(request.Username))
                            {
                                lock (_clients)
                                {
                                    _clients.Remove(request.Username);
                                }

                                ConsoleExt.Write("[", ConsoleColor.DarkGray);
                                ConsoleExt.Write("-", ConsoleColor.Red);
                                ConsoleExt.Write("]", ConsoleColor.DarkGray);
                                Console.Write($" {request.Username} disconnected.");
                                Console.WriteLine();

                                await SendResponse(context, 200, "{\"info\":\"Ok\"}");
                            }
                            else await SendResponse(context, 404, "{\"error\":\"Fake Request\"}");
                        }
                        else await SendResponse(context, 404, "{\"error\":\"Fake Request\"}");
                    }
                    else
                    {
                        await SendResponse(context, 404, "{\"error\":\"Not Found\"}");
                    }
                }
                else
                {
                    await SendResponse(context, 405, "{\"error\":\"Method Not Allowed\"}");
                }
            }
            catch (Exception ex)
            {
                ConsoleExt.WriteLine(ex.ToString(), ConsoleColor.Red);
            }
        }

        private async Task FriendUpdateResponse(HttpListenerContext context, string username)
        {
            var onlineFriends = new List<object>();
            lock (_clients)
            {
                if (!_clients.TryGetValue(username, out var me)) return;
                foreach (var friendName in me.FriendNames)
                {
                    if (_clients.TryGetValue(friendName, out var friend) && friend.IsOnline() && friend.FriendNames.Contains(username))
                    {
                        onlineFriends.Add(new { name = friendName, online = true, server = friend.Server ?? "unknown" });
                    }
                    else
                    {
                        onlineFriends.Add(new { name = friendName, online = false, server = "unknown" });
                    }
                }
            }

            var response = new { type = "friend_update", friends = onlineFriends };
            string json = JsonSerializer.Serialize(response);
            await SendResponse(context, 200, json);
        }

        private async Task SendResponse(HttpListenerContext context, int status, string content)
        {
            context.Response.StatusCode = status;
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.ContentType = "application/json";
            await context.Response.OutputStream.WriteAsync(buffer);
            context.Response.Close();
        }
    }
}
