﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomateCore.Scheduler.Interfaces
{
    public interface IAsyncTaskExecutor : ITaskExecutor
    {
        new Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
