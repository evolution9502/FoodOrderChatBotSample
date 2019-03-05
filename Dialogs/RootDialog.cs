namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string SteaksOption = "Steaks";

        private const string PizzaOption = "Pizza";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //2.1
            var message = await result;

            if (message.Text.ToLower().Contains("help") || message.Text.ToLower().Contains("support") || message.Text.ToLower().Contains("problem") || message.Text.ToLower().Contains("how"))
            {
                //await context.Forward(new SupportDialog(), this.ResumeAfterSupportDialog, message, CancellationToken.None);
                context.Call(new SupportDialog(), this.ResumeAfterSupportDialog);
            }
            else
            {
                this.ShowOptions(context);
            }

        }

        private void ShowOptions(IDialogContext context)
        {
            //2.2
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { SteaksOption, PizzaOption }, "Welcome to Mediterranean restaurant, do you want a steak or pizza?", "Not a valid option", 3);

        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case SteaksOption:
                        context.Call(new SteaksDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case PizzaOption:
                        context.Call(new PizzaDialog(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            //2.3
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);

        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}
