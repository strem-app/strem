# Strem
A free and open source automation tool geared for assisting streamers.

![strem image](docs/images/strem.png)

> Currently this is the most ALPHA of all alphas, so by all means use it, but just keep in mind this is SUPER early days.

## What Does It Do?

It lets you build complex logic `flows` from `triggers` and `tasks`.

For example you may want to play a meme on OBS every time chat contains "lollerskates", so you would make a twitch chat trigger, and have a task to show an obs source.

> To run OBS you will need to go and install the [v4.*](https://github.com/obsproject/obs-websocket/releases/tag/4.9.1) [obs-websocket-remote plugin](https://obsproject.com/forum/resources/obs-websocket-remote-control-obs-studio-using-websockets.466/updates#resource-update-3807) which lets other applications communicate with OBS, version 5 of the plugin is not supported yet.

## How Do I Run It?
Just go to releases, download the latest zip file and put it in a directory somewhere and run `strem.exe`

> Currently only windows is supported, but only a few things realistically require windows so in the long run we may pull those dependencies out to support other platforms too (i.e file browsing, keyboard input simulation, tts etc).

The only integrations supported out of the box currently are `Twitch` and `OBS`, however we only have a small subset of available triggers/tasks available for the moment to assist with testing, these will rapidly get fleshed out as more time is spent..

> For the moment the docs are sparse, feel free to ask questions in issues though and more docs will be provided in the `docs` folder as time goes by.

## HALP!

Here are some of the existing tutorials for how to do basic use cases for the app:

### Creating A Death Counter (Per Game)
[![Watch the video](https://img.youtube.com/vi/Dg-VzJN4Mk4/default.jpg)](https://youtu.be/Dg-VzJN4Mk4)

### Create Todos From Commands (Or Anything)
[![Watch the video](https://img.youtube.com/vi/0XYkXwu0SBk/default.jpg)](https://youtu.be/0XYkXwu0SBk)

### How To Delete Flows
[![Watch the video](https://img.youtube.com/vi/TkI_oELPkys/default.jpg)](https://youtu.be/TkI_oELPkys)

## I Want To Add To This Project

Thats great, for the moment everything is VERY fluid so while im open to taking PRs and FRs just keep in mind I want to stabilize stuff a bit further before I open the flood gates a fully.

To build it locally you will to:

- Make sure you have `.net 6.0`
- Pull > Nuget Restore > Run
- Read Stuff In [docs](./docs) folder

> Once loaded if you go into `Logs` you will be able to see where your data source files live and information around whats happening at runtime.

