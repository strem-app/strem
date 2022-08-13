using Cake.Frosting;
using Strem.Build;

return new CakeHost()
    .UseContext<BuildContext>()
    .Run(args);