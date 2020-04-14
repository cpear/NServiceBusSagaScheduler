# NServiceBus Saga Scheduler
An example of using a Saga to run a scheduled job with NServiceBus

## Setup
This codebase assumes you have a named SQL Server instance called **SqlExpress** and have created a database called **NsbSamplesSql**.

The connection string looks like this:
`Data Source=.\\SqlExpress;Database=NsbSamplesSql;Integrated Security=True`

If you already have a version of SQLServer installed or wish to use a different database you will need to change the connection string in 
the `Program.cs` of both the **Sender** and **NServiceBus.SagaScheduler** projects.

## Endpoints
**NServiceBus.SagaScheduler** is both a Send and Receive endpoint. Upon startup it will send a message to itself to _start the scheduled job_. 
Each time it receives this _start scheduled job_ message it will make sure it hasn't been started previously to avoid mutiple versions of 
the same job from being scheduled. 

Actions related to the _start a scheduled job_ command will display as green text in the console window(s).

**Sender** is a send only endpoint. This endpoint allows _start scheduled job_ commands to be fired manually for the purpose of demonstrating that 
the job will not be scheduled multiple times.

## Instructions
We will begin by starting up multiple instance of the scheduler to demonstrate that the scheduled job can be scaled out without worrying about 
multiple instances of the job being created. Then we will start the send-only **Sender** endpoint to manually fire off more _start scheduled job commands_ to 
again demonstrate that only one instance of the job is running.

1. Run the **NServiceBus.SagaScheduler** from Visual Studio 2019
2. Wait for scheduler to begin running the job
3. Next start a second instance of **NServiceBus.SagaScheduler** endpoint by right-clicking the **NServiceBus.SagaScheduler** project in the solution explorer 
and selecting **Debug > Start New Instance**
4. Repeat step 3 if desired
5. Wait and watch the command consoles compete to pick up the job and begin running it
6. Next start an instance of the **Sender** project.
7. From the **Sender** console window hit the enter button to manually fire off a _start scheduled job_ command
