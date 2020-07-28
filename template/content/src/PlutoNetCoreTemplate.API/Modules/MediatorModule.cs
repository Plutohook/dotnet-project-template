﻿using Autofac;

using MediatR;

using PlutoNetCoreTemplate.Application.Behaviors;
using PlutoNetCoreTemplate.Application.Commands;

using System.Reflection;
using PlutoNetCoreTemplate.Application.EventBus.Users;

namespace PlutoNetCoreTemplate.API.Modules
{
    public class MediatorModule : Autofac.Module
    {
        /*
         * https://github.com/jbogard/MediatR/wiki
         */
        protected override void Load(ContainerBuilder builder)
        {

            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();
            
            builder.RegisterAssemblyTypes(typeof(CreateUserCommand).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>)).InstancePerDependency();


            builder.RegisterAssemblyTypes(typeof(DisableUserEventHandler).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));


            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });


            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>)).InstancePerDependency(); ;
            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>)).InstancePerDependency(); ;
        }
    }
}