namespace MultiDialogsBot
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    [Serializable]
    public class SteakQuery
    {
        //6.1
        [Prompt("What kind of {&} do you want for your steak?")]
        public string Meat { get; set; }

        [Prompt("Do you want {&}?")]
        public string Spicy { get; set; }

        [Numeric(1, int.MaxValue)]
        [Prompt("How would you like it {&}?")]
        public string Cooked { get; set; }

    }
}