﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mediator.Net.Binding;
using Mediator.Net.Context;
using Mediator.Net.Contracts;

namespace Mediator.Net.Pipeline
{
    class RequestPipe<TContext> : IRequestReceivePipe<TContext>
        where TContext : IReceiveContext<IRequest>
    {
        private readonly IPipeSpecification<TContext> _specification;
        private readonly IDependancyScope _resolver;

        public RequestPipe(IPipeSpecification<TContext> specification, IPipe<TContext> next, IDependancyScope resolver)
        {
            Next = next;
            _specification = specification;
            _resolver = resolver;
        }

        public async Task<object> Connect(TContext context)
        {
            object result = null;
            try
            {
                await _specification.ExecuteBeforeConnect(context);
                result = await (Next?.Connect(context) ?? ConnectToHandler(context));
                await _specification.ExecuteAfterConnect(context);
            }
            catch (Exception e)
            {
                _specification.OnException(e, context);
            }
            return result;
        }

        private async Task<object> ConnectToHandler(TContext context)
        {
            var handlers =
                MessageHandlerRegistry.MessageBindings.Where(
                    x => x.MessageType == context.Message.GetType()).ToList();
            if (!handlers.Any())
                throw new NoHandlerFoundException(context.Message.GetType());

            if (handlers.Count() > 1)
            {
                throw new MoreThanOneHandlerException(context.Message.GetType());
            }

            var binding = handlers.Single();

            var handlerType = binding.HandlerType;
            var messageType = context.Message.GetType();

            var handleMethods = handlerType.GetRuntimeMethods().Where(m => m.Name == "Handle");
            var handleMethod = handleMethods.Single(y =>
            {
                var parameterTypeIsCorrect = y.GetParameters().Single()
                    .ParameterType.GenericTypeArguments.First()
                    .GetTypeInfo()
                    .IsAssignableFrom(messageType.GetTypeInfo());

                return parameterTypeIsCorrect
                       && y.IsPublic
                       && ((y.CallingConvention & CallingConventions.HasThis) != 0);
            });

            var handler = (_resolver == null) ? Activator.CreateInstance(handlerType) : _resolver.Resolve(handlerType);

            var task = (Task)handleMethod.Invoke(handler, new object[] { context });
            await task.ConfigureAwait(false);

            return task.GetType().GetTypeInfo().GetDeclaredProperty("Result").GetValue(task);
        }


        public IPipe<TContext> Next { get; }
    }
}