﻿using System;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Pipeline;
using Mediator.Net.Test.TestUtils;

namespace Mediator.Net.Test.Middlewares
{
    static class ConsoleLog2
    {
        public static void UseConsoleLogger2<TContext>(this IPipeConfigurator<TContext> configurator)
            where TContext : IContext<IMessage>
        {
            configurator.AddPipeSpecification(new ConsoleLogSpecification2<TContext>());
        }
    }

    class ConsoleLogSpecification2<TContext> : IPipeSpecification<TContext> 
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
                Console.WriteLine("Before 2");
            RubishBox.Rublish.Add(nameof(ConsoleLog2.UseConsoleLogger2));
            return Task.FromResult(0);
        }

        public Task ExecuteAfterConnect(TContext context)
        {
            if (ShouldExecute(context))
                Console.WriteLine("After 2");
            return Task.FromResult(0);
        }

        public void OnException(Exception ex, TContext context)
        {
            RubishBox.Rublish.Add(ex);
            throw ex;
        }
    }
}
