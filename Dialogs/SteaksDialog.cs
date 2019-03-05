namespace MultiDialogsBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class SteaksDialog : IDialog<object>
    {
        private string Meat = "none";
        private string Cooked = "none";

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome to the Steak Chooser!");
            this.MeatOptions(context);
        }

        private void MeatOptions(IDialogContext context)
        {
            //2.2
            PromptDialog.Choice(context, this.OnMeatOptionSelected, new List<string>() { "Pork", "Steak", "Chicken" }, "What kind of meat do you want?", "Sorry, we dont provide this...", 4);

        }

        private void CookedOptions(IDialogContext context)
        {
            //2.2
            PromptDialog.Choice(context, this.OnCookedOptionSelected, new List<string>() { "Medium-Rare", "Medium", "Medium-Well", "Well-Done" }, "How would you like it cooked?", "Sorry, we dont provide this...", 5);

        }

        private async Task OnMeatOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                Meat = await result;
                this.CookedOptions(context);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task OnCookedOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                Cooked = await result;

                if (Meat.Equals("Pork") || Meat.Equals("Chicken"))
                {
                    if (!(Cooked.Equals("Well-Done")))
                    {
                        await context.PostAsync("Sorry, we only provide well-done cooked pork and chicken!");
                        //this.CookedOptions(context);
                    }
                }
                else
                {
                    var hotels = await this.GetSteaksAsync(Meat, Cooked);

                    await context.PostAsync($"I found in total {hotels.Count()} steak for your choice:");

                    var resultMessage = context.MakeMessage();
                    resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    resultMessage.Attachments = new List<Attachment>();


                    foreach (var hotel in hotels)
                    {
                        HeroCard heroCard = new HeroCard()
                        {
                            Title = hotel.Name,
                            Subtitle = $"{hotel.Rating} starts. {hotel.NumberOfReviews} reviews. From ${hotel.PriceStarting} per portion.",
                            Images = new List<CardImage>()
                        {
                            new CardImage() { Url = hotel.Image }
                        },
                            Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "Order this!",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.teststrips4money.com/wp-content/uploads/2015/09/order-confirmed-test-strips-4-money.jpg"
                            },
                            new CardAction()
                            {
                                Title = "More details",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/search?q=hotels+in+" + HttpUtility.UrlEncode(hotel.Location)
                            }
                        }
                        };

                        resultMessage.Attachments.Add(heroCard.ToAttachment());
                    }

                    await context.PostAsync(resultMessage);
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
            finally
            {
                context.Done<object>(null);
            }
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
                this.MeatOptions(context);
            }

        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            //2.3
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);

        }

        private async Task<IEnumerable<Hotel>> GetSteaksAsync(string meat, string cooked)
        {
            var hotels = new List<Hotel>();
            string img;
            if (meat.Equals("Beef"))
            {
                img = "https://food.fnr.sndimg.com/content/dam/images/food/fullset/2007/1/2/0/valentines_steak.jpg.rend.hgtvcom.826.620.suffix/1385127027449.jpeg";
            }
            else if (meat.Equals("Pork"))
            {
                img = "https://food.fnr.sndimg.com/content/dam/images/food/fullset/2015/11/22/0/BXSP04H_Spiced-Roast-Pork_s4x3.jpg.rend.hgtvcom.826.620.suffix/1449598951075.jpeg";
            }
            else
            {
                img = "http://1.bp.blogspot.com/-pswdfKlOFQU/TZHu2tuzqkI/AAAAAAAABGQ/MHY7dpDRHjQ/s320/000_0019_%25E5%2589%25AF%25E6%259C%25AC.jpg";
            }

            // Filling the hotels results manually just for demo purposes
            for (int i = 1; i <= 5; i++)
            {
                var random = new Random(i);
                Hotel hotel = new Hotel()
                {
                    Name = $"Special {meat} {i}",
                    Location = cooked,
                    Rating = random.Next(1, 5),
                    NumberOfReviews = random.Next(0, 5000),
                    PriceStarting = random.Next(80, 450),
                    Image = $"{img}"
                };

                hotels.Add(hotel);
            }

            hotels.Sort((h1, h2) => h1.PriceStarting.CompareTo(h2.PriceStarting));

            return hotels;
        }
    }
}