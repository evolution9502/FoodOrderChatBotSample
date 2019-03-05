namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class SupportDialog : IDialog<int>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //3.1
            await context.PostAsync($"<Hello dear customer, how can I help you?");
            context.Wait(this.MessageReceivedAsync);

        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //3.2
            var message = await result;

            var ticketNumber = new Random().Next(0, 20000);

            if (message.Text.ToLower().Contains("nothing") || message.Text.ToLower().Contains("thank") || message.Text.ToLower().Contains("back") || message.Text.ToLower().Contains("ok") || message.Text.ToLower().Contains("fine"))
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");
            }

            await context.PostAsync($"Your message '{message.Text}' was registered. Once we resolve it; we will get back to you.");

            context.Done(ticketNumber);

        }
    }
}
