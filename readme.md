# Twitch Chat Control
🤖 **A Twitch bot that consumes commands on chat to send keystrokes on the local machine.**

### Feeling generous?
[![Buy me a Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/brofar)

## Features
* Listens to Twitch chat using an account you specify.
* Sends keystrokes on your local machine in response to commands (e.g. have viewers control the game).

## To Do
- [x] Finish the damn bot.
- [x] Installation instructions.
- [x] Configuration/Setup instructions (Adding to Twitch).
- [x] Handle decimal numbers/symbols in input (reject them)
- [x] Handle multi key presses
- [x] Handle sequences of commands
- [x] Rewrite configuration instructions
- [ ] Explore targeted window for sendkeys
- [ ] Explore async

## Installation
See the [Releases page](https://github.com/brofar/TwitchChatControl/releases), download the latest file, extract, configure, run.
(No releases yet? It's probably because I'm a perfectionist and am still finding bugs or fine-tuning)

## Configuration
1. Extract the zip file from the Releases page.
1. Rename settings.sample.xml to settings.xml.
1. Fill in settings.xml per the table below.
1. If needed, create a key map and save it in the configs folder.

### settings.xml properties
|Key|Required?|Description|
|---|---------|-----------|
|`username`|☑|Your own or your bot's Twitch username|
|`token`|☑|To get this, go to https://twitchapps.com/tmi/, click Connect, sign in with the username you used in the username setting. Copy the oauth string that website generates as your token.|
|`channel`|☑|Your Twitch username.|

#### Example
```
<?xml version="1.0" encoding="utf-8" ?>
<TwitchBot>
  <username>bronear</username>
  <token>oauth:abcdefg123456789</token>
  <channel>brofar</channel>
</TwitchBot>
```

### Key Maps
Key maps follow this format: `<[chat message]>[key to press]</[chat message]>`

**Key Combinations**: To hold multiple keys at the same time use the `+` symbol. `<[chat message]>[key 1]+[key 2]</[chat message]>`

**Key Sequences**: To send multiple keys with a single chat message `-` symbol. `<[chat message]>[key 1]-[key 2]</[chat message]>`


#### Example
ffx.xml
```
<?xml version="1.0" encoding="utf-8" ?>
<keyMap>
  <!-- Base Movement -->
  <Up>Up</Up>
  <Down>Down</Down>
  <Left>Left</Left>
  <Right>Right</Right>

  <!-- Diagonal Movement -->
  <UR>UP+RIGHT</UR>
  <UL>UP+LEFT</UL>
  <DR>DOWN+RIGHT</DR>
  <DL>DOWN+LEFT</DL>
  <LR>LEFT+RIGHT</LR>

  <!-- Jecht Shot Movement -->
  <UX>UP+C</UX>
  <DX>DOWN+C</DX>
  <LX>LEFT+C</LX>
  <RX>RIGHT+C</RX>

  <!-- Main Buttons -->
  <X>C</X>
  <O>X</O>
  <S>Z</S>
  <T>V</T>

  <!-- Shoulder Buttons -->
  <L>S</L>
  <R>D</R>

  <!-- Other Buttons -->
  <char>A</char>
  <Start>B</Start>
  <Select>N</Select>

  <!-- "Cheats" -->
  <turbo>F1</turbo>
  <enc>F3</enc>
  <ab>F4</ab>

  <!-- Auron's Overdrives -->
  <df>DOWN-LEFT-UP-RIGHT-S-D-X-C</df>
  <ss>V-X-Z-C-LEFT-RIGHT-C</ss>
  <bb>UP-S-DOWN-D-RIGHT-LEFT-V</bb>
  <to>C-RIGHT-D-LEFT-S-V</to>

  <!-- Lulu's Overdrive -->
  <fury>LEFT-RIGHT</fury>
</keyMap>
```

## Future Improvements
* Hook for channel point redemptions