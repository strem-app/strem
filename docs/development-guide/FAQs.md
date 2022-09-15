# FAQs

### What is this made in?

It is made in `C#` using [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) for the front end with [Photino](https://docs.tryphotino.io/Photino-Blazor) hosting the web view, MAUI was going to be the hosting app but its still early days and Rider doesnt support it.

### Why have you made this with web tech?

A few reasons really:

- Html/Css tech is probably the easiest and most flexible UI solution available
- Photino Blazor lets us run C# like any application but making use of the vast html/css/js elements and widgets available
- It is really easy to iterate and prototype compared to other UI tech
- It is easy for other developers to join in and work with

Ultimately leveraging web tech makes things easier, some people don't like seeing web looking UIs in their native applications, but some of the most popular apps these days are using this sort of tech (mainly TS/Electron etc though).

### Why do we need this? there is already Touch Portal, Streamerbot etc

GREAT QUESTION!

I love both of these apps and have used them in the past, there are quite a few others as well but ultimately the majority are closed source so if you want to add your own tasks/triggers/integrations you are ultimately in a queue for the devs to look into and hopefully prioritise, with this you can just do it yourself.