using PusherClient;
using System;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace RestroPrint
{
    public class PrintData
    {
        public string? Text { get; set; }
        public string? PrinterType { get; set; }
        public string? Ip { get; set; }
        public string? PrinterPort { get; set; }
    }

    public class PusherService
    {
        private readonly Pusher _pusher;
        private Channel? _channel;

        private readonly string _channelName = "printer";
        private readonly string _eventName = "App\\Events\\PrinterEvent";

        private readonly Action<string, string?, int?, string> _onPrintReceived;

        // Now accepting appKey and cluster as constructor parameters
        public PusherService(string appKey, string cluster, Action<string, string?, int?, string?> onPrintReceived)
        {
            _onPrintReceived = onPrintReceived;

            // Initialize Pusher with the provided appKey and cluster
            _pusher = new Pusher(appKey, new PusherOptions
            {
                Cluster = cluster,
                Encrypted = true
            });

            // Handle connection state changes
            _pusher.ConnectionStateChanged += (sender, args) =>
            {
                LogHelper.Append(args.ToString());
            };

            // Handle any Pusher errors
            _pusher.Error += (sender, args) =>
            {
                LogHelper.Append($"Error: {args.Message}");
            };
        }

        // Connect to Pusher
        public async Task ConnectAsync()
        {
            try
            {
                await _pusher.ConnectAsync();
                _channel = await _pusher.SubscribeAsync(_channelName);

                // Bind to the event where print requests are received
                _channel.Bind(_eventName, (string rawJson) =>
                {
                    JObject rawObj = JObject.Parse(rawJson);
                    if (rawObj["data"]?.Type == JTokenType.String)
                    {
                        JObject nested = JObject.Parse(rawObj["data"]!.ToString());
                        PrintData? data = nested.ToObject<PrintData>();

                        if (data != null && !string.IsNullOrEmpty(data.Text))
                        {
                            if (data.PrinterType == "lan" && !string.IsNullOrEmpty(data.Ip) && int.TryParse(data.PrinterPort, out int port))
                            {
                                _onPrintReceived?.Invoke(data.Text, data.Ip, port, data.PrinterType);
                            }
                            else if (data.PrinterType == "usb")
                            {
                                _onPrintReceived?.Invoke(data.Text, null, null, data.PrinterType);
                            }
                        }
                        else
                        {
                            LogHelper.Append("No text received.");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.Append("Error while connecting to Server: " + ex.Message);
            }
        }
    }
}
