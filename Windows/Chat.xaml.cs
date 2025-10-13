using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static OMPS.NetworkCommunicationManager;

namespace OMPS.Windows
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        public NetworkCommunicationManager NetworkManager { get; set; }
        public ObservableCollection<string> Messages { get; set; } = [];

        public Chat()
        {
            InitializeComponent();
            //
            this.InitializeNetwork();
        }

        private async void InitializeNetwork()
        {
            this.NetworkManager = new();
            this.NetworkManager.MessageReceived += this.OnNetworkMessageReceived;
            this.NetworkManager.CommandReveived += this.OnNetworkCommandReceived;
            this.NetworkManager.PeerFound += this.NetworkManager_PeerFound;
            await NetworkManager.InitializeAsync();

            this.Txtblk_Status.Text = "Network communication ready";
        }

        private void NetworkManager_PeerFound(object? sender, PeerDataEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Messages.Add($"[SYS]:  '{e.user}' connected");
            });
        }

        private void OnNetworkMessageReceived(object? sender, MessageReceievedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var displayName = e.user is null or "?" ? e.ip : e.user;
                Messages.Add($"[{e.user}]:  {e.message}");
            });
        }

        private void OnNetworkCommandReceived(object? sender, MessageReceievedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.message is null) return;
                var displayName = e.user is null or "?" ? e.ip : e.user;
                var split = e.message.Split(':');
                if (split.Length < 3) return;
                var cmd = split[0];
                var filter = split[1];
                var fitlers = filter.Split(',');
                var data = string.Join(":", split[2..]);
                switch (split[0].ToLower())
                {
                    case "notify":
                        if (!(filter is "*" || fitlers.Contains(this.NetworkManager.LocalIp))) return;
                        MessageBox.Show(data, $"Notification from [{displayName}]");
                        break;
                    default:
                        break;
                }
            });
        }

        private async void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is not Key.Enter) return;
            if ((TextBox)sender is not TextBox txt) return;
            if (string.IsNullOrWhiteSpace(txt.Text)) return;
            if (txt.Text.StartsWith("/msg "))
            {
                var split = txt.Text.Split(" ");
                if (this.NetworkManager.userLookup.FirstOrDefault(u => u.Value.user == split[1]) is KeyValuePair<string, (string machine, string user)> found && found.Key is not null)
                {
                    MessageBox.Show(found.GetType().Name);
                    await NetworkManager.SendToPeer(found.Key, string.Join(" ", split[2..]));
                    Messages.Add($"[You -> {found.Value.user}]:  {txt.Text}");
                } else
                {
                    Messages.Add($"[SYS]:  Couldn't find connected user with alias '{split[1]}'");
                }
            }
            else if (txt.Text.StartsWith("/cmd "))
            {
                await NetworkManager.SendCommandToPeer(NetworkManager.LocalIp, txt.Text[5..]);
            }
            else
            {
                await NetworkManager.SendToAllPeers(txt.Text);
                Messages.Add($"[You]:  {txt.Text}");
            }
            txt.Clear();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.NetworkManager?.Dispose();
            base.OnClosed(e);
        }
    }
}
