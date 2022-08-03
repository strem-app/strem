using Strem.Core.Flows;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.Flows;

public interface ILoadFlowStorePipeline : ILoadDataPipeline<FlowStore>
{
}