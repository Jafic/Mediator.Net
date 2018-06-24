﻿using System;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Mediator.Net.Test.TestUtils;

namespace Mediator.Net.Test.Middlewares
{
    static class ConsoleLog3
    {
        public static void UseConsoleLogger3<TContext>(this IPipeConfigurator<TContext> configurator)
            where TContext : IContext<IMessage>
        {
            configurator.AddPipeSpecification(new ConsoleLogSpecification3<TContext>());
        }
    }

    class ConsoleLogSpecification3<TContext> : IPipeSpecification<TContext> 
        where TContext : IContext<IMessage>
    {
        public bool ShouldExecute(TContext context)
        {
            return true;
        }

        public Task ExecuteBeforeConnect(TContext context)
        {
            return Task.FromResult(0);
        }

        public Task Execute(TContext context)
        {
            if (ShouldExecute(context))
                Console.WriteLine("Before 3");
            RubishBox.Rublish.Add(nameof(ConsoleLog3.UseConsoleLogger3));
            return Task.FromResult(0);
        }

        public Task ExecuteAfterConnect(TContext context)
        {
            if (ShouldExecute(context))
                Console.WriteLine("After 3");
            return Task.FromResult(0);
        }

        public void OnException(Exception ex, TContext context)
        {
            throw ex;
        }
    }
}
