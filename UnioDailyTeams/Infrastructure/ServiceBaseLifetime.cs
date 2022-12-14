using FluentScheduler;
using System;
using System.Collections.Generic;
using Teams.Notifications;
using Topshelf;

namespace UnioDailyTeams.Infrastructure
{
    public class ServiceBaseLifetime : ServiceControl
    {
        TeamsNotificationClient _client = new TeamsNotificationClient("seu webhook aqui");
        public bool Start(HostControl hostControl)
        {

            JobManager.Initialize();
            JobManager.AddJob(

                () => EnviarMensagem(),
                s => s.NonReentrant()
                      .ToRunEvery(1)
                      .Days()
                      .At(8, 59)
            );

            return true;
        }


        public bool Stop(HostControl hostControl)
        {
            JobManager.Stop();
            return true;
        }

        public void EnviarMensagem()
        {
            var message = new MessageCard();
            message.Title = "Daily " + DateTime.Now.ToString("dd/MM/yyyy");
            message.Text = "Bom dia, procure ser objetivo em sua daily de hoje.";
            message.Color = "f0ad4e";
            message.Sections = new List<MessageSection>();
            message.Sections.Add(new MessageSection() { Title = "O que você fez ontem?" });
            message.Sections.Add(new MessageSection() { Title = "O que vai fazer hoje?" });
            message.Sections.Add(new MessageSection() { Title = "Tem algum impedimento?" });

            _client.PostMessage(message).GetAwaiter().GetResult();
        }
    }
}
