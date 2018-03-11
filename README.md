# TransmissionRemoteBot
## Purpose
This is a C# bot that allows controlling transmission RPC using Telegram messenger. This is a WIP, right now only monitoring status and adding torrents from links (.torrent or magnet) is supported.
## Running
### Bot registration
Follow instructions at https://core.telegram.org/bots#creating-a-new-bot
### Running on Windows
* Copy appsettings.json into appsettings.local.json; Set telegram apiKey and transmission url/login/password there.
* Press F5
### Running on Debian
* From ConsoleRunner folder run ./publish-debian-x64.ps1
* If you are developing on Windows machine, copy files from \bin\release\netcoreapp2.0\debian-x64\publish\ to your linux box
* Execute dotnet ConsoleRunner.dll
### Installing as service on Debian
* Copy transmission-remote-bot.service from ConsoleRunner folder to /lib/systemd/system
* Set paths in ExecStart and WorkingDirectory to appropriate values
* systemctl daemon-reload
* systemctl enable transmission-remote-bot
* systemctl start transmission-remote-bot
* verify service started correctly using systemctl status transmission-remote-bot and journalctl --unit transmission-remote-bot
