# RestroPrint - C# Thermal Printer Listener App

**RestroPrint** is a Windows Forms application built with C# that listens for real-time print requests via [Pusher](https://pusher.com/) and sends them to a connected thermal printer (USB or LAN).

## 🔧 Features

- Connects to **Pusher Channels** for real-time printing.
- Accepts printer configuration via Pusher message (`text`, `printerType`, `ip`, `port`).
- Prints using USB printer or LAN printer over TCP.
- Prompts for **Pusher App Key** and **Cluster** on first run and stores them locally in registry.
- Simple logging interface for debugging incoming events and print status.

## 📦 Prerequisites

- Windows 10 or later
- .NET Framework 8
- A thermal printer connected via USB or available on the local network (LAN)
- Internet connection (for Pusher)

## 🚀 Getting Started

### 1. Clone this repository:

```bash
git clone git@github.com:pujan-thapa/restro-print-csharp.git
```

### 2. Build the project:

Open the solution in Visual Studio and build the solution.

### 3. Run the application:

On first launch, a modal window will prompt for:

- **Pusher App Key**
- **Pusher Cluster**

These values are saved locally for future launches.

### 4. Listening Details:

- **Channel Name**: `printer`
- **Event Name**: `App\Events\PrinterEvent`

### 5. Expected Payload from Server (Eg: via Laravel):

From your backend, send print requests using:

```php
$data = [
    'text' => $text,
    'printerType' => $printer->type, // 'usb' or 'lan'
    'ip' => $printer->ip,             // Required if 'lan'
    'printerPort' => $printer->port, // Default: 9100
];

$pusher->trigger('printer', 'App\\Events\\PrinterEvent', $data);
```

## 🖨️ Printer Types

- **USB**: Prints via default system printer.
- **LAN**: Sends raw text data over TCP/IP (e.g., to port `9100` of the printer IP).

## 📂 Local Settings

- First-time configuration is stored locally in the registry at `HKEY_CURRENT_USER\Software\RestroPrint`.
- You can update the Pusher App Key and Cluster by modifying the configuration file.

## 💬 Logging

The main form displays a real-time log of:

- Pusher connection status
- Incoming print messages
- Print results or errors

## 📄 License

MIT — feel free to use, modify, or contribute.
