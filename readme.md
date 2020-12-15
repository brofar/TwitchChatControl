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
* Configuration/Setup instructions (Adding to Twitch).
* ~~Handle decimal numbers/symbols in input (reject them)~~
* Handle sequences of commands

## Installation
See the [Releases page](https://github.com/brofar/TwitchChatControl/releases), download the latest file, extract, configure, run.

## Configuration
To specify a key map for the bot to use, create an XML file in the following format:
```
<?xml version="1.0" encoding="utf-8" ?>
<keyMap>
    <[chat message]>[bot key]</[chat message]>
</keyMap>
```

Example: ffx.xml
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
