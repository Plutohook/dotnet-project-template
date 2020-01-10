﻿using MediatR;

namespace Pluto.netcoreTemplate.Domain.Events.UserEvents
{
    public class EnableUserEvent: INotification
    {

        public string Message { get; set; }


        public EnableUserEvent(string msg)
        {
            Message = msg;
        }
    }
}