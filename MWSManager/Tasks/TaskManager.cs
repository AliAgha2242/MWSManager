using Application.ServiceContract;
using Application.ServiceImplement;
using Microsoft.Extensions.DependencyInjection;
using Model.ApplicationModels;
using Model.PresentationModels;
using Model.PublicModels;
using MWSMan.Core.Infrastructure.Extenssion;
using MWSMan.Core.Persistence.StaticConfiguration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MWSManager.Tasks
{
    public class TaskManager : BackgroundService
    {
        //public IRabbitTaskService rabbitTaskService { get; }
        public ISend SendService { get; set; }
        public IServiceProvider ServiceProvider { get; }

        private CancellationTokenSource cancellationTokenSource;
        //private IList<RabbitTasks> rabbitTasks;
        //private IList<RabbitTasks> newRabbitTasks;
        private readonly ConcurrentDictionary<int, RunningTaskInfo> _runningTasks = new();
        private readonly DockerDateTime dockerDateTime;
        private readonly IMinooConfiguration minooConfiguration;
        private readonly IGetConfiguration getConfiguration;
        private bool ChangeTaskList;

        public TaskManager(DockerDateTime dockerDateTime,IServiceProvider serviceProvider, IMinooConfiguration minooConfiguration,IGetConfiguration getConfiguration)
        {
            //this.rabbitTaskService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IRabbitTaskService>();
            SendService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ISend>();
            ChangeTaskList = false;
            this.dockerDateTime = dockerDateTime;
            ServiceProvider = serviceProvider;
            this.minooConfiguration = minooConfiguration;
            this.getConfiguration = getConfiguration;
            this.cancellationTokenSource = new CancellationTokenSource();

        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {



#if !DEBUG
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncTasks(stoppingToken);
                }
                catch (Exception ex)
                {
                    // log
                }

                await Task.Delay(
                    TimeSpan.FromHours(3),
                    stoppingToken);
            }
#endif


            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    await getConfiguration.GetConfigAsync("AppConfig.json");
            //    await getConfiguration.GetConfigAsync("AppConfig.json");
            //    await Task.Delay(TimeSpan.FromHours(1));
            //}
           
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var item in _runningTasks.Values)
            {
                item.CancellationTokenSource.Cancel();
            }

            await Task.WhenAll(
                _runningTasks.Values
                             .Select(x => x.Task));

            await base.StopAsync(cancellationToken);
        }



        private async Task<IList<RabbitTasks>> GetRabbitTasks(IRabbitTaskService service)
        {
            var res = await service.ReadAllAsync(p => p.RabbitTaskServerss, p => p.rabbitConsumerRequestHub);
            if (!res.IsSuccess)
            {
                await StopAsync(cancellationTokenSource.Token);
                return null;
            }
            else
            {
                IList<RabbitTasks> ress = res.Data?.Where(p => !p.StopForce && DateTime.Now >= p.FromDateTime && dockerDateTime.NowDateTime <= p.ToDateTime).ToList() ?? new List<RabbitTasks>();
                return ress;
            }
        }
        private async Task SendTask(RabbitTasks rabbitTasks)
        {
            SendService = ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ISend>();
            await SendService.RunTask(rabbitTasks, false);
        }

        private async Task SyncTasks(CancellationToken stoppingToken)
        {
            using var scope = ServiceProvider.CreateScope();

            var rabbitTaskService =
                scope.ServiceProvider
                    .GetRequiredService<IRabbitTaskService>();

            var dbTasks = (await GetRabbitTasks(rabbitTaskService)).Where(p => p.StopForce == false);

            dbTasks ??= new List<RabbitTasks>();

            var dbTaskIds = dbTasks
                .Select(x => x.Id)
                .ToHashSet();

            foreach (var running in _runningTasks)
            {
                if (!dbTaskIds.Contains(running.Key))
                {
                    running.Value.CancellationTokenSource.Cancel();

                    _runningTasks.TryRemove(
                        running.Key,
                        out _);
                }
            }

            foreach (var dbTask in dbTasks)
            {
                if (!_runningTasks.TryGetValue(
                        dbTask.Id,
                        out var running))
                {
                    StartTask(dbTask);
                    continue;
                }

                if (NeedRestart(
                        running.Config,
                        dbTask))
                {
                    running.CancellationTokenSource.Cancel();

                    _runningTasks.TryRemove(
                        dbTask.Id,
                        out _);

                    StartTask(dbTask);
                }
            }
        }
        private void StartTask(RabbitTasks rabbitTask)
        {
            var cts = new CancellationTokenSource();

            var task = RunTask(
                rabbitTask,
                cts.Token);

            _runningTasks.TryAdd(
                rabbitTask.Id,
                new RunningTaskInfo
                {
                    Config = rabbitTask,
                    Task = task,
                    CancellationTokenSource = cts
                });
        }
        private async Task RunTask(RabbitTasks rabbitTask, CancellationToken token)
        {
            using var scope =
                ServiceProvider.CreateScope();

            var rabbitTaskServerService =
                scope.ServiceProvider
                     .GetRequiredService<
                         IRabbitTaskServerService>();



            DateTime? LastExecuteDate = rabbitTask.RabbitTaskServerss.FirstOrDefault(p => p.LastStartDate != null)?.LastStartDate ?? dockerDateTime.NowDateTime;
            int totalintervalMinuteDiff = rabbitTask.IntervalTotalMinute - (((dockerDateTime.NowDateTime - LastExecuteDate).Value.Days * 24 * 60) + ((dockerDateTime.NowDateTime - LastExecuteDate).Value.Hours * 60) + ((dockerDateTime.NowDateTime - LastExecuteDate).Value.Minutes));
            if (totalintervalMinuteDiff > 0)
            {
                await Task.Delay(TimeSpan.FromMinutes(totalintervalMinuteDiff));
            }


            while (!token.IsCancellationRequested)
            {
                try
                {

                    await SendTask(rabbitTask);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in task {rabbitTask.TaskName}: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(rabbitTask.IntervalTotalMinute), token);
            }
        }

        private bool NeedRestart(RabbitTasks oldTask, RabbitTasks newTask)
        {
            return
                oldTask.IntervalTotalMinute
                    != newTask.IntervalTotalMinute
                ||
                oldTask.StopForce
                    != newTask.StopForce
                ||
                oldTask.ToDateTime
                    != newTask.ToDateTime
                ||
                oldTask.FromDateTime
                    != newTask.FromDateTime;
        }

    }
}
