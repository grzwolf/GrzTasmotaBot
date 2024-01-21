# GrzTasmotaBot
Control your home's Tasmota socket devices via Telegram messenger. 
GrzTasmotaBot is built for Windows x64 >= version 7. 

# Background Tasmota
One of the main goals of Tasmota devices is privacy, so no vendor cloud is needed.
A simple webbrowser is all you need to control your Tasmota devices locally.
There are scenarios, you want to access your Tasmota devices from the internet. 
While there are cloud based broker services available, there is a simple alternative. 

# How does GrzTasmotaBot work? 
The alternative is using the Telegram messenger app on your phone, which communicates with 
a Telegram bot running on a PC in your local network. 
Such Telegram bot acts as an interface between your phone and your Tasmota socket devices.
GrzTasmotaBot contains the described Telegram bot and takes care about the messages conversion quirks.

# GrzTasmotaBot Installation
- have a Windows PC running 24/7, even Atom Z8350 consuming <8 W (headless) is fine
- download & extract the latest ExecutableFilesFolder.zip from Releases -> tags
- simply copy ALL files from ExecutableFilesFolder.zip into a folder of your choice
- run GrzTasmotaBot.exe

# GrzTasmotaBot Configuration
- make sure, your Tasmota socket devices work in the same subnet as the PC running GrzTasmotaBot
- make sure, your Tasmota socket devices have unique names, like Tamota #1 ... Tamota #n 
- create a Telgram bot, follow the instructions on https://core.telegram.org/bots#creating-a-new-bot
- set GrzTasmotaBot -> Settings -> BotAuthenticationToken

# GrzTasmotaBot Configuration - optional whitelist
- send /help from you phone's Telegram messenger to the above created bot
- GrzTasmotaBot -> Messages Logger will show the sender (usually 9 numbers): TIMESTAMP RX '/help' from: SENDER
- set GrzTasmotaBot -> Settings -> 'Telegram whitelist' in the following format: name,SENDER
- Note: the COMMA between the name and the SENDER is essential !!
- you may add more whitelist members
- set GrzTasmotaBot -> Settings -> UseTelegramWhitelist to True
- GrzTasmotaBot -> System Menu --> "Send 'test' to 1st whitelist entry" to check if above format is ok
- Note: if UseTelegramWhitelist is enabled and the whitelist is empty or in wrong format, the bot won't work 

# Limitations (2024/01/19)
a) device types

Currently only Tasmota socket devices are supported by their basic commands: On, Off and Status.
The supported devices are listed in the file TasmotaSockets.txt, which could easily be amended.

b) NOUS A1T

Since I only own 'NOUS A1T' sockets, there was no further testing of other socket devices.

# Build yourself
Download sources, open project in Visual Studio, build --> profit.
