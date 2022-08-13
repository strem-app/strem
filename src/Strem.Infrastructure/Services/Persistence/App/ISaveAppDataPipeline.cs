using Strem.Core.Variables;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.App;

public interface ISaveAppDataPipeline : ISaveDataPipeline<IVariables>
{
    
}