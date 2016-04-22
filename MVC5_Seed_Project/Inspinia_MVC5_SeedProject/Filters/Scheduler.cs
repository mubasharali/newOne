using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inspinia_MVC5_SeedProject.CodeTemplates;
namespace Inspinia_MVC5_SeedProject.Filters
{
    public class Scheduler
    {
        public void Index()
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("myJob", "group1")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("myTrigger", "group1")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInSeconds(86400)
                  .RepeatForever())
              .Build();

            sched.ScheduleJob(job, trigger);
        }
    }
    public class HelloJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ElectronicsController.sendEmail("irfanyusanif@gmail.com", "I am job scheduler", "I am running at" + DateTime.UtcNow);
            //throw new NotImplementedException();
        }
        
    }
}