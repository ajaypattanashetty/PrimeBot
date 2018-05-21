using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PrimeBot.Dialogs
{
    [Serializable]
    [LuisModel("c4da5f79-7d35-4f54-882e-f59b431da62b", "ff7dd279118d4ca0be98aa3fc982d473")]
    public class PrimeLUISDailog: LuisDialog<Object>
    {
        [LuisIntent("MovieRecommendations")]
        public async Task MovieRecommendations(IDialogContext context, LuisResult result)
        {
            if (result.TopScoringIntent.Score > 0.5)
            {
                await context.PostAsync(string.Format("Ok, Here are top 5 {0} movies for you", result.Entities[0].Entity));
            }
            else
            {
                await context.PostAsync("I can only suggest and play movies for you. Sorry if I disappointed you");
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("PlayMovie")]
        public async Task PlayMovie(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(string.Format("Cool playing movie {0} for you", result.Entities[0].Entity));
            context.Wait(MessageReceived);
        }

        [LuisIntent("ReportError")]
        public async Task ReportError(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(string.Format("Sorry for the {0}, will log a service ticket on behalf of you.", result.Entities[0].Type));
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry I am not trained to answer this");
            context.Wait(MessageReceived);
        }
    }
}