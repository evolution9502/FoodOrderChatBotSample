namespace MultiDialogsBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class PizzaQuery
    {
        //6.1
        [Prompt("What kind of {&} do you want for your pizza?")]
        public string Meat { get; set; }

        [Prompt("Do you want {&}?")]
        public string Spicy { get; set; }

        [Numeric(1, int.MaxValue)]
        [Prompt("Do you want {&}?")]
        public string DoubleCheese { get; set; }

    }
}