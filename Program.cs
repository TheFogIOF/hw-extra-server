using HWExtra_Server;

public class Program
{
    private static async Task Main(string[] args)
    {
        ConsoleExt.WriteLine("[HolyWorldExtra] Server build 0.2a", ConsoleColor.Cyan);

        String file = Path.Combine(Directory.GetCurrentDirectory(), "main.cfg");
        String serverLink = "http://localhost:5000/";

        if (File.Exists(file))
        {
            serverLink = File.ReadAllText(file);
        }
        else 
        {
            File.WriteAllText(file, serverLink);
        }

        Server server = new Server(serverLink);
        await server.Start();
    }
}