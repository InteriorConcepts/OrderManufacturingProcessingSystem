
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OMPS
{

    public class TcpNetworkManager : IDisposable
    {
        private TcpListener _listener;
        private TcpClient _client;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly int _port;
        private bool _isRunning;

        public event EventHandler<string> MessageReceived;
        public event EventHandler<Exception> ErrorOccurred;
        public event EventHandler ClientConnected;
        public event EventHandler ClientDisconnected;

        public int Port => _port;
        public bool IsRunning => _isRunning;

        public TcpNetworkManager(int port = 8080)
        {
            _port = port;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartServerAsync()
        {
            if (_isRunning)
                return;

            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            _isRunning = true;

            _ = Task.Run(async () => await AcceptConnectionsAsync(_cancellationTokenSource.Token));
        }

        private async Task AcceptConnectionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync(cancellationToken);
                    _ = Task.Run(async () => await HandleClientAsync(client, cancellationToken));
                }
                catch (Exception ex)
                {
                    OnErrorOccurred(ex);
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            try
            {
                OnClientConnected();

                using var stream = client.GetStream();
                var buffer = new byte[4096];

                while (client.Connected && !cancellationToken.IsCancellationRequested)
                {
                    var bytesRead = await stream.ReadAsync(buffer, cancellationToken);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    OnMessageReceived(message);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
            finally
            {
                OnClientDisconnected();
                client.Dispose();
            }
        }

        public async Task<bool> ConnectToServerAsync(string serverIp, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(serverIp, port);
                _isRunning = true;

                // Start listening for messages
                _ = Task.Run(async () => await ListenForMessagesAsync(_cancellationTokenSource.Token));

                return true;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                return false;
            }
        }

        private async Task ListenForMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var stream = _client.GetStream();
                var buffer = new byte[4096];

                while (_client.Connected && !cancellationToken.IsCancellationRequested)
                {
                    var bytesRead = await stream.ReadAsync(buffer, cancellationToken);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    OnMessageReceived(message);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
            }
        }

        public async Task SendMessageAsync(string message, string? targetIp = null, int? targetPort = null)
        {
            try
            {
                if (targetIp != null && targetPort.HasValue)
                {
                    // Send to specific target
                    using var tempClient = new TcpClient();
                    await tempClient.ConnectAsync(targetIp, targetPort.Value);
                    var stream = tempClient.GetStream();
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(messageBytes);
                }
                else if (_client?.Connected == true)
                {
                    // Send through existing connection
                    var stream = _client.GetStream();
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(messageBytes);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred(ex);
                throw;
            }
        }

        public async Task BroadcastMessageAsync(string message, string[] targetIps, int port)
        {
            List<Task> tasks = [];

            foreach (var ip in targetIps)
            {
                tasks.Add(SendMessageAsync(message, ip, port));
            }

            await Task.WhenAll(tasks);
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }

        protected virtual void OnErrorOccurred(Exception exception)
        {
            ErrorOccurred?.Invoke(this, exception);
        }

        protected virtual void OnClientConnected()
        {
            ClientConnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnClientDisconnected()
        {
            ClientDisconnected?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _listener?.Stop();
            _client?.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }

    public class UdpNetworkManager : IDisposable
    {
        private UdpClient _udpClient;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly int _port;

        public event EventHandler<string>? MessageReceived;

        public UdpNetworkManager(int port = 8081)
        {
            _port = port;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartListeningAsync()
        {
            _udpClient = new UdpClient(_port);

            _ = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var result = await _udpClient.ReceiveAsync();
                        string message = Encoding.UTF8.GetString(result.Buffer);
                        OnMessageReceived(message);
                    }
                    catch (Exception)
                    {
                        // Handle error
                    }
                }
            });
        }

        public async Task BroadcastMessageAsync(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _udpClient.SendAsync(messageBytes, messageBytes.Length, "255.255.255.255", _port);
        }

        public async Task SendMessageAsync(string message, string targetIp)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await _udpClient.SendAsync(messageBytes, messageBytes.Length, targetIp, _port);
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _udpClient?.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }

    public class NetworkCommunicationManager : IDisposable
    {
        private readonly TcpNetworkManager _tcpManager;
        private readonly UdpNetworkManager _udpManager;
        private readonly string _localIp;
        public string LocalIp { get => this._localIp; }
        private readonly string _user;
        public string User { get => this._user; }
        private readonly string _machine;
        public string Machine { get => this._machine; }
        private readonly List<string> _knownPeers = [];
        private readonly Dictionary<string, (string machine, string user)> _userLookup = [];
        public Dictionary<string, (string machine, string user)> userLookup { get => this._userLookup; }

        public NetworkCommunicationManager(int tcpPort = 8080, int udpPort = 8081)
        {
            _tcpManager = new TcpNetworkManager(tcpPort);
            _udpManager = new UdpNetworkManager(udpPort);
            _localIp = GetLocalIpAddress();
            _machine = Environment.MachineName;
            _user = Environment.UserName;

            // Wire up events
            _tcpManager.MessageReceived += OnMessageReceived;
            _udpManager.MessageReceived += OnUdpMessageReceived;
        }

        public async Task InitializeAsync()
        {
            // Start TCP server
            await _tcpManager.StartServerAsync();

            // Start UDP listener for discovery
            await _udpManager.StartListeningAsync();

            // Broadcast presence
            await BroadcastPresence();
        }

        private async void OnUdpMessageReceived(object? sender, string message)
        {
            if (!message.StartsWith("DISCOVER:")) return;
            var parts = message.Split(':');
            if (parts.Length is not 4) return;
            var foundIp = parts[1];
            if (foundIp == this._localIp) return;
            var foundMachine = parts[2];
            var foundUser = parts[3];
            if (foundMachine == this._machine) return;
            if (foundUser == this._user) return;
            // New peer discovered
            if (!this._knownPeers.Contains(foundIp))
            {
                this._knownPeers.Add(foundIp);
                await ConnectToPeer(foundIp);
            }
            if (!this._userLookup.TryGetValue(foundIp, out (string machine, string user) existing) ||
                existing.machine != foundMachine ||
                existing.user != foundUser)
            {
                this._userLookup[foundIp] = (foundMachine, foundUser);
            }
        }

        private async Task BroadcastPresence()
        {
            while (true)
            {
                await _udpManager.BroadcastMessageAsync($"DISCOVER:{_localIp}:{_machine}:{_user}");
                await Task.Delay(5000); // Broadcast every 5 seconds
            }
        }

        private async Task ConnectToPeer(string ipAddress)
        {
            await _tcpManager.ConnectToServerAsync(ipAddress, _tcpManager.Port);
        }

        public async Task SendToAllPeers(string message)
        {
            foreach (var peer in _knownPeers)
            {
                await _tcpManager.SendMessageAsync($"MSG:{this._localIp}:{message}", peer, _tcpManager.Port);
            }
        }

        public async Task SendToPeer(string peer, string message)
        {
            await _tcpManager.SendMessageAsync($"MSG:{this._localIp}:{message}", peer, _tcpManager.Port);
        }

        public async Task SendCommandToAllPeers(string message)
        {
            foreach (var peer in _knownPeers)
            {
                await _tcpManager.SendMessageAsync($"CMD:{this._localIp}:{message}", peer, _tcpManager.Port);
            }
        }

        public async Task SendCommandToPeer(string peer, string message)
        {
            await _tcpManager.SendMessageAsync($"CMD:{this._localIp}:{message}", peer, _tcpManager.Port);
        }

        private void OnMessageReceived(object? sender, string message)
        {
            // Handle TCP messages
            var type = "";
            switch (message[..3].ToLower())
            {
                case "msg":
                    type = "msg";
                    break;
                case "cmd":
                    type = "cmd";
                    break;
                default:
                    break;
            }
            if (type is "") return;
            var parts = message.Split(':');
            if (parts.Length < 2) return;
            var ip = parts[1];
            string machine, user;
            if (this._userLookup.TryGetValue(ip, out (string machine, string user) value))
            {
                (machine, user) = value;
            }
            else
            {
                machine = "?";
                user = "?";
            }
            MessageReceievedEventArgs e = new()
            {
                ip = ip,
                machine = machine,
                user = user,
                message = string.Join(":", parts[2..])
            };

            switch (type)
            {
                case "msg":
                    this.MessageReceived?.Invoke(this, e);
                    break;
                case "cmd":
                    this.CommandReveived?.Invoke(this, e);
                    break;
                default:
                    break;
            }
        }

        public class MessageReceievedEventArgs
        {
            public string? message;
            public string? ip;
            public string? machine;
            public string? user;
        }

        public event EventHandler<MessageReceievedEventArgs> MessageReceived;
        public event EventHandler<MessageReceievedEventArgs> CommandReveived;

        private string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        public void Dispose()
        {
            _tcpManager?.Dispose();
            _udpManager?.Dispose();
        }
    }
}
