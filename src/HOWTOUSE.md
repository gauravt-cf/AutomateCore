# 🚀 AutomateCore Scheduler

**AutomateCore Scheduler** is a lightweight and extensible scheduling library for .NET applications. It allows you to execute background tasks at configurable intervals using a simple, configuration-driven approach.

Perfect for Windows Services, background jobs, or long-running worker processes.

---

## ✨ Features

- 🛠 Simple and intuitive interface
- ⚙️ Schedule tasks using `App.config` or `Web.config`
- 🔄 Lifecycle event hooks for logging or monitoring
- 🪶 Lightweight with minimal dependencies
- 🧱 Compatible with .NET Framework 4.0 and above
- ⚡ Support for asynchronous (`async/await`) task execution

---

## 📦 Installation

Add a reference to the `AutomateCore` assembly in your .NET project.

---

## ⚙️ Configuration

AutomateCore now supports **two types of scheduling configurations**:

- ✅ **Legacy (Single Schedule)** — for simple scheduling
- 🔁 **Multi-Task Scheduler** — supports multiple independently configured tasks

---

### ✅ Legacy: Single Schedule Setup

This format is used when the service runs on a single global schedule.

#### Example: `App.config` / `Web.config`

```xml
<configuration>
  <configSections>
    <section name="schedulerSettings" type="AutomateCore.Configuration.SchedulerConfigSection, AutomateCore" requirePermission="false" />
  </configSections>

  <schedulerSettings>
    <add scheduleType="Hourly" enabled="true" hour="0" minute="0" dayOfWeek="" dayOfMonth="0" />
  </schedulerSettings>
</configuration>
```

### 🔁 New: Multi-Task Scheduler

You can now define multiple scheduled tasks within a single configuration block.

#### Example: `Multi-Task Configuration`

```xml
<configuration>
  <configSections>
    <section name="schedulerSettings" type="AutomateCore.Configuration.SchedulerConfigSection, AutomateCore" requirePermission="false" />
  </configSections>

  <schedulerSettings>
    <!-- Optional global fallback -->
    <add scheduleType="Immediate" enabled="true" hour="0" minute="0" dayOfWeek="" dayOfMonth="0" />

    <tasks>
      <!-- Task 1: Immediate Execution -->
      <add name="Task1"
           enabled="true"
           scheduleType="Immediate"
           hour="0"
           minute="0"
           maxConcurrent="1"
           paused="false" />

      <!-- Task 2: Daily at 2:00 AM System -->
      <add name="Task2"
           enabled="true"
           scheduleType="Daily"
           dayOfWeek=""
           hour="02"
           minute="00"
           paused="false"
           maxConcurrent="1" />
    </tasks>
  </schedulerSettings>
</configuration>
```

---
## 🏁 Getting Started

Here's a minimal example to get you up and running:
---
### ✅ Single Task Example

```csharp
static void Main()
{
    var scheduler = new SchedulerEngine(() =>
    {
        Console.WriteLine("Scheduled task executed!");
    });

    scheduler.Start();
}
```
#### This is the simplest way to schedule a task using the default (single) configuration.
---

## 🔁 Multi-Task Scheduler Example
AutomateCore also supports registering multiple independent tasks with individual configurations:

```csharp
    public class MyClass
    {
        private MultiTaskScheduler taskSchedulerEngine;
        public MyClass(){
            taskSchedulerEngine = new MultiTaskScheduler();
        }
        static void Main()
        {
            //task registration
             taskSchedulerEngine.RegisterTask(new TaskExecutor("Task1", ct => StartDealerAPI()));
             taskSchedulerEngine.RegisterTask(new TaskExecutor("Task2", ct => StartOrderCommunication()));

              // Subscribe to events
            taskSchedulerEngine.TaskStarted += (name, time) => Console.WriteLine($"{name} started at {time}");
            taskSchedulerEngine.TaskCompleted += (name, time) => Console.WriteLine($"{name} completed at {time}");

            // Start all scheduled tasks
            await taskSchedulerEngine.StartTaskAsync("Task1");
            await taskSchedulerEngine.StartTaskAsync("Task2");
        }
    }
```
#### Each task uses the settings defined in your App.config under the <schedulerSettings><tasks>...</tasks> section.
### 📌 Note: `Task names passed to RegisterTask(name, action) must match those defined in your config file`:
```xml
    <tasks>
        <add name="Task1" ... />
        <add name="Task2" ... />
    </tasks>
```
---

## 🔍 Logging Integration

You can integrate any logging framework inside the lifecycle callbacks:

```csharp
void NotifyServiceFailed(Exception ex, DateTime timestamp)
{
    Log.Error(ex, $"Task failed at {timestamp}");
}
```
---
## 🧪 Debugging / Testing
For testing purposes, you can use `ScheduleType.Immediate` to run your task immediately on app start.
```xml
<add scheduleType="Immediate" enabled="true" />
```
---
## 🗓️ Schedule Configuration Options
| Attribute     | Description                                              |
|---------------|----------------------------------------------------------|
| `scheduleType` | The type of schedule to use (see supported types below).|
| `enabled`      | Set to `true` to enable this schedule                    |
| `hour`         | Hour at which the task should run (use when `scheduleType` is `Daily`)                        |
| `minute`       | Minute at which the task should run    (use when `scheduleType` is `Daily`/`Hourly`)                  |
| `dayOfWeek`    | *(Optional)* Day(s) of the week for weekly schedule      |
| `dayOfMonth`   | *(Optional)* Date of the month for monthly schedule      |

---

## 🧠 Supported `ScheduleType` Values

The following `ScheduleType` values are supported and can be used in the configuration:

### 🔁 Recurring Intervals

- `Immediate` — Executes as soon as the application starts.
- `EveryMinute` — Executes once every 1 minute.
- `Every2Minutes` — Executes once every 2 minutes.
- `Every5Minutes` — Executes once every 5 minutes.
- `Every10Minutes` — Executes once every 10 minutes.
- `Every15Minutes` — Executes once every 15 minutes.
- `Every30Minutes` — Executes once every 30 minutes.
- `Every2Hours` — Executes once every 2 hours.
- `Every4Hours` — Executes once every 4 hours.
- `Every6Hours` — Executes once every 6 hours.


### 📅 Time-Based Scheduling

- `Hourly` — Executes once every hour at the specified minute.
- `Daily` — Executes once a day at the specified time.
- `Weekly` — Executes once a week on the specified day and time.
- `Monthly` — Executes once a month on the specified date and time.

### 📌 Fixed Custom Schedules

- `Weekly_Monday` — Executes every Monday
- `Weekly_Friday` — Executes every Friday
- `Weekends` — Executes on Saturday and Sunday
- `TwiceDaily` — Executes twice a day (e.g., once in the morning and once in the evening)
- `Quarterly` — Executes every 3 months
- `SemiMonthly` — Executes on two fixed dates each month (e.g., 1st and 15th)
- `SemiAnnually` — Executes every 6 months (e.g., January and July)

## 🧑‍💻 Usage
### Declare a Scheduler Field
```csharp
private ISchedulerEngine _scheduler;
```
### 🧰 Initialize the Scheduler

Create and configure the scheduler by passing in your task logic along with optional lifecycle event handlers. These callbacks also provide useful context such as timestamps and error details.

```csharp
// Your main task to be executed on schedule
void RunTask()
{
    Console.WriteLine("Running scheduled task...");
}

// Lifecycle event handlers with context-aware parameters
void NotifyServiceStarted(DateTime timeStamp) =>
    Console.WriteLine($"✅ Task started at {timeStamp}");

void NotifyServiceCompleted(DateTime timeStamp) =>
    Console.WriteLine($"✅ Task completed at {timeStamp}");

void NotifyServiceSkipped(string reason, DateTime timestamp) =>
    Console.WriteLine($"⚠️ Task skipped at {timestamp} — Reason: {reason}");

void NotifyServiceFailed(Exception ex, DateTime timestamp) =>
    Console.WriteLine($"❌ Task failed at {timestamp} — Error: {ex.Message}");

// Initialize the scheduler with task and callbacks
_scheduler = new SchedulerEngine(
    RunTask,
    onTaskStarted: NotifyServiceStarted,
    onTaskCompleted: NotifyServiceCompleted,
    onTaskSkipped: NotifyServiceSkipped,
    onTaskFailed: NotifyServiceFailed
);
```
 ---
 ## 🚀 Scheduler Control Easily control the scheduler's lifecycle using the following methods:
 ### ▶️ Start the Scheduler 
 ```csharp 
 _scheduler.Start();
 ``` 
 > Use this when your task delegates are **not async**. 
 > If your tasks are async or use `await`, call the async version instead:

 #### ✅ Start Async (for async tasks) 

 ```csharp
 await _scheduler.StartAsync();
 ``` 

 ---
 ### ⏹️ Stop the Scheduler Gracefully stops the scheduler and waits for any running tasks to complete.
 ```csharp 
 _scheduler.Stop();
 ``` 
 --- 
 ### ⏸️ Pause the Scheduler Temporarily pauses all scheduled tasks without clearing them. You can resume later.
 ```csharp 
 _scheduler.Pause();
 ``` 
 --- 
 ### ▶️ Resume the Scheduler Resumes task execution after a pause. 
 ```csharp 
 _scheduler.Resume();
 ```
 --- 
## 🪝 Lifecycle Hooks

You can pass in optional delegates to handle different lifecycle events of the scheduled task:

- **`onTaskStarted`** — Called before the task starts.
- **`onTaskCompleted`** — Called after the task completes successfully.
- **`onTaskSkipped`** — Called if the task is skipped based on the schedule.
- **`onTaskFailed`** — Called if the task throws an exception.

---
## 📘 Example
```csharp
void RunTask()
{
    // Your background logic here
}

void NotifyServiceStarted(DateTime timeStamp) => Console.WriteLine("Task started");
void NotifyServiceCompleted(DateTime timeStamp) => Console.WriteLine("Task completed");
void NotifyServiceSkipped(string reason, DateTime timestamp) => Console.WriteLine("Task skipped");
void NotifyServiceFailed(Exception ex, DateTime timestamp) => Console.WriteLine($"Task failed: {ex.Message}");
```

---

## 🧩 Extensibility

AutomateCore Scheduler is designed to be easily extensible.

You can create your own custom schedule types by implementing the `IScheduleCalculator` interface. This allows you to define completely custom logic for determining the next run time of your task.

### Example:

```csharp
public class CustomScheduleCalculator : IScheduleCalculator
{
    public DateTime GetNextRunTime(DateTime lastRun)
    {
        // Custom logic to calculate next run time
        return lastRun.AddMinutes(7); // for example, every 7 minutes
    }
}
```
---
## 📝 Suggestions & Roadmap
### 🔖 Status Legend
- ✅ Completed – Feature is fully implemented and available.
- 🚧 In Progress – Actively being worked on.
- 📝 Planned – Feature is on the roadmap but work hasn't started yet.
- ❌ Dropped – Feature was considered but will not be implemented.
- 💡 Suggested – Proposed idea; feedback welcome.

| Feature / Suggestion                          | Status       | Notes                            |
|----------------------------------------------|--------------|----------------------------------|
| Async task support                            | ✅ Completed 💡 Suggested  | Available                      |
| Retry logic for failed tasks                  | ✅ Completed  | Completed with default 3 retries with 10 second delay |
| .NET Standard 2.0 / Core platform support     | ✅ Completed | Enables cross-platform compatibility |
| Pause/Resume scheduled tasks at runtime       | ✅ Completed | In-memory toggle without restarting service |
| Task execution history logging                | ✅ Completed | Useful for debugging and monitoring |
| Graceful shutdown handling                    | ✅ Completed | Ensures tasks finish before exit |

---

## 🤝 Contributing

This is a C# class library with the codebase hosted privately.

For suggestions, feature requests, or feedback, please use the public GitHub repository dedicated to discussions and issues:

- 📧 Email: mailto:gauravt@channel-fusion.com

We welcome your thoughts and ideas to help improve the library — even if the source code isn't public yet.

---
## 📬 Contact

Got questions? Reach out to **Gaurav Thakur** on [LinkedIn](https://www.linkedin.com/in/gaurav-thakur-714599239/), [GitHub](https://github.com/gauravt-cf), or [Email](mailto:gauravt@channel-fusion.com)!


