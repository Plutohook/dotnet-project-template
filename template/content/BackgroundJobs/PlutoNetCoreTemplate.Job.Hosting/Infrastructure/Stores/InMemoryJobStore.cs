﻿namespace PlutoNetCoreTemplate.Job.Hosting.Infrastructure
{
    using Models;

    using Quartz;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class InMemoryJobStore : IJobInfoStore
    {
        private static List<JobInfoModel> jobs = new();

        /// <inheritdoc />
        public Task<int> CountAsync()
        {
            return Task.FromResult(jobs.Count);
        }

        /// <inheritdoc />
        public async Task<List<JobInfoModel>> GetListAsync()
        {
            return await Task.FromResult(jobs);
        }

        /// <inheritdoc />
        public async Task<JobInfoModel> GetAsync(string id)
        {
            var model = jobs.FirstOrDefault(x => x.Id == id);
            return await Task.FromResult(model);
        }

        /// <inheritdoc />
        public async Task<JobInfoModel> GetAsync(JobKey job)
        {
            var model = jobs.FirstOrDefault(x => x.GroupName == job.Group && x.TaskName == job.Name);
            return await Task.FromResult(model);
        }

        /// <inheritdoc />
        public async Task AddAsync(JobInfoModel job)
        {
            if (jobs.Any(x => x.GroupName == job.GroupName && x.TaskName == job.TaskName))
            {
                throw new InvalidOperationException("任务已存在");
            }
            jobs.Add(job);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(JobInfoModel job)
        {
            if (!jobs.Any(x => x.GroupName == job.GroupName && x.TaskName == job.TaskName))
            {
                throw new InvalidOperationException("任务不存在");
            }
            var old = jobs.FirstOrDefault(x => x.GroupName == job.GroupName && x.TaskName == job.TaskName);
            jobs.Remove(old);
            jobs.Add(job);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task RemoveAsync(string groupName, string jobName)
        {
            var old = jobs.FirstOrDefault(x => x.GroupName == groupName && x.TaskName == jobName);
            jobs.Remove(old);
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task PauseAsync(string groupName, string jobName)
        {
            var old = jobs.FirstOrDefault(x => x.GroupName == groupName && x.TaskName == jobName);
            if (old == null)
            {
                throw new InvalidOperationException("任务不存在");
            }
            old.Status = EnumJobStates.Pause;
            var @new = old;
            jobs.Remove(old);
            jobs.Add(@new);
            await Task.CompletedTask;
        }
    }
}