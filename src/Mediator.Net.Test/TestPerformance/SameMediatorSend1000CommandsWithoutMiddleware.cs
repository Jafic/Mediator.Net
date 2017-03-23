﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Mediator.Net.Binding;
using Mediator.Net.Test.CommandHandlers;
using Mediator.Net.Test.Messages;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Mediator.Net.Test.TestPerformance
{
    public class SameMediatorSend1000CommandsWithoutMiddleware : TestBase
    {
        private IMediator _mediator;
        private long milliSeconds = 0;
        public void GivenAMediator()
        {
            ClearBinding();
            var builder = new MediatorBuilder();
            _mediator = builder.RegisterHandlers(() =>
            {
                var binding = new List<MessageBinding> { new MessageBinding(typeof(NoWorkCommand), typeof(NoWorkCommandHandler)) };
                return binding;
            })
            .Build();

        }

        public async Task When1000CommandIsSent()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                await _mediator.SendAsync(new NoWorkCommand());
            }
            sw.Stop();
            milliSeconds = sw.ElapsedMilliseconds;
            Console.WriteLine($"it took {milliSeconds} milliseconds");
        }

        public void ThenItShouldNotTakeMoreThan50MilliSeconds()
        {
            milliSeconds.ShouldBeLessThan(50);
        }

        [Fact]
        public void Run()
        {
            this.BDDfy();
        }
    }
}
