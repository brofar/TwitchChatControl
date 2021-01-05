# Twitch Chat Control
🤖 **A Twitch bot that consumes commands on chat to send keystrokes on the local machine.**

### Feeling generous?
[![Buy me a Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/brofar)

## Features
* Listens to Twitch chat using an account you specify.
* Sends keystrokes on your local machine in response to commands (e.g. have viewers control the game).

## To Do
* ~~Finish the damn bot.~~
* ~~Installation instructions.~~
* ~~Configuration/Setup instructions (Adding to Twitch).~~
* ~~Handle decimal numbers/symbols in input (reject them)~~
* ~~Handle multi key presses~~
* Handle sequences of commands
* Rewrite configuration instructions
* Explore targeted window for sendkeys
* Explore async

## Installation
See the [Releases page](https://github.com/brofar/TwitchChatControl/releases), download the latest file, extract, configure, run.

## Configuration
1. Extract the zip file from the Releases page.
1. Rename app-sample.config to app.config.
1. Configure app.config per the table below.
1. If needed, write and save your key map to the configs folder.

### app.config properties
|Key|Required?|Description|
|---|---------|-----------|
|`username`|☑|Your own or your bot's Twitch username|
|`token`|☑|To get this, go to https://twitchapps.com/tmi/, click Connect, sign in with the username you used in the username setting. Copy the oauth string that website generates as your token.|
|`channel`|☑|The Twitch channel the bot should join (99.9% of the time this should be your own channel).|

### Key Maps
To specify a key map for the bot to use, create an XML file in the following format:
```
<?xml version="1.0" encoding="utf-8" ?>
<keyMap>
    <[chat message]>[bot key]</[chat message]>
</keyMap>
```

#### Example
ffx.xml
```
<?xml version="1.0" encoding="utf-8" ?>
<keyMap>
  <Up>Up</Up>
  <Down>Down</Down>
  <Left>Left</Left>
  <Right>Right</Right>
  <X>C</X>
  <O>X</O>
  <S>Z</S>
  <T>V</T>
  <L>S</L>
  <R>D</R>
  <char>A</char>
  <Start>B</Start>
  <Select>N</Select>
  <turbo>F1</turbo>
  <enc>F3</enc>
  <ab>F4</ab>
</keyMap>
```

## Future Improvements
* Hook for channel point redemptions
