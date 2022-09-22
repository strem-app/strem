## Prerequisite Tech

If you want to work with this library, to either PR back into core, or to add to your own separate plugin it is recommended you know a bit about the following tech/patterns.

- **Blazor** - Its really mainly Html/Css/Js + C#
- **ASP MVC** - This is mainly used for side hosted content internally
- **Reactive Extensions** - We use `IObservable` quite a bit to handle events/triggers
- **Dependency Injection** - We HEAVILY use this to let stuff access dependencies
- **Unit Testing** - If you want to PR stuff ideally we need it to have unit tests to verify behaviour
- **Data/View Separation** - We heavily separate data, logic and view to make it easier to pass data around and test

For the most part we are not doing anything too crazy, and everything is written in a way that you can just follow the same conventions yourself (see other docs for more info).
