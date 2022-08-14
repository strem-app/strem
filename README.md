# Strem
A free and open source automation tool geared for assisting streamers.

![strem image](docs/images/strem.png)

> Currently this is the most ALPHA of all alphas, so by all means use it, but just keep in mind this is SUPER early days.


## What Does It Do?

It lets you build complex logic `flows` from `triggers` and `tasks`.

For example you may want to play a meme on OBS every time chat contains "lollerskates", so you would make a twitch chat trigger, and have a task to show an obs source.

## How Do I Run It?
Just go to releases, download the latest zip file and put it in a directory somewhere and run `strem.exe`

The only integrations supported out of the box currently are `Twitch` and `OBS`, however we only have a small subset of available triggers/tasks available for the moment to assist with testing, these will rapidly get fleshed out as more time is spent..

> For the moment the docs are sparse, feel free to ask questions in issues though and more docs will be provided in the `docs` folder as time goes by.

## I Want To Add To This Project

Thats great, for the moment everything is VERY fluid so while im open to taking PRs and FRs just keep in mind I want to stabilize stuff a bit further before I open the flood gates a fully.

To build it locally you will to:

- Make sure you have `.net 6.0`
- Pull > Nuget Restore > Run

> Once loaded if you go into `Logs` you will be able to see where your data source files live and information around whats happening at runtime.

