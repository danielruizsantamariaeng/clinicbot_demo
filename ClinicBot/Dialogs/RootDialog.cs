using ClinicBot.Common.Cards;
using ClinicBot.Data;
using ClinicBot.Dialogs.CreateAppointment;
using ClinicBot.Dialogs.Qualification;
using ClinicBot.Infrastructure.Luis;
using ClinicBot.Infrastructure.QnMakerAI;
using ClinicBot.Infrastructure.SendGridEmail;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicBot.Dialogs
{
    public class RootDialog: ComponentDialog
    {
        private readonly ILuisService _luisService;
        private readonly IDataBaseService _databaseService;
        private readonly ISendGridEmailService _sendGridService;
        private readonly IQnMakerAIService _qnAMakerAIService;

        public RootDialog(ILuisService luisService, IDataBaseService databaseService, UserState userState, ISendGridEmailService sendGridService, IQnMakerAIService qnAMakerAIService)
        {
            _luisService = luisService;
            _qnAMakerAIService = qnAMakerAIService;
            _databaseService = databaseService;
            _sendGridService = sendGridService;

            // Contiene un array de metodos que se ejecutan secuecialmente
            var watterfallSteps = new WaterfallStep[] { 
                InitialProcess,
                FinalProcess
            };
            AddDialog(new QualificationDialog(_databaseService));
            AddDialog(new CreateAppointmentDialog(_databaseService,userState, _sendGridService,_luisService));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), watterfallSteps));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> InitialProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var luisResult = await _luisService._luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            return await ManageIntentions(stepContext, luisResult, cancellationToken);
        }



        private async Task<DialogTurnResult> FinalProcess(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ManageIntentions(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var topIntent = luisResult.GetTopScoringIntent();
            var score = topIntent.score;
            {
                switch (topIntent.intent)
                {
                    case "Saludar":
                        await IntentSaludar(stepContext, luisResult, cancellationToken);
                        break;
                    case "Agradecer":
                        await IntentAgradecer(stepContext, luisResult, cancellationToken);
                        break;
                    case "Despedir":
                        await IntentDespedir(stepContext, luisResult, cancellationToken);
                        break;
                    case "VerOpciones":
                        await IntentVerOptiones(stepContext, luisResult, cancellationToken);
                        break;
                    case "VerCentroContacto":
                        await IntentVerCentroContacto(stepContext, luisResult, cancellationToken);
                        break;
                    case "Calificar":
                        return await IntentCalificar(stepContext, luisResult, cancellationToken);
                    case "CrearCita":
                        return await IntentCrearCita(stepContext, luisResult, cancellationToken);
                    case "VerCita":
                        await IntentVerCita(stepContext, luisResult, cancellationToken);
                        break;
                    case "None":
                        await IntentNone(stepContext, luisResult, cancellationToken);
                        break;
                    default:
                        await IntentNone(stepContext, luisResult, cancellationToken);
                        break;
                }
            }
            return await stepContext.NextAsync(null, cancellationToken);
        }


        #region Intents

        private async Task IntentVerCita(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Un momento por favor...",cancellationToken: cancellationToken);
            await Task.Delay(1000);

            string idUser = stepContext.Context.Activity.From.Id;
            var medicalData = _databaseService.MedicalAppointment.Where(x => x.idUser == idUser).ToList();
            if (medicalData.Count > 0)
            {
                var pending = medicalData.Where(p => p.date >= DateTime.Now.Date).ToList();

                if (pending.Count > 0)
                {
                    await stepContext.Context.SendActivityAsync("Estas son tus citas pendientes: ", cancellationToken: cancellationToken);

                    foreach (var item in pending)
                    {
                        await Task.Delay(1000);
                        if (item.date == DateTime.Now.Date && item.time < DateTime.Now.Hour)
                        {
                            continue;
                        }

                        string summaryMedical = $"📅 Fecha: {item.date.ToShortDateString()}" +
                            $"{Environment.NewLine} ⏰ Hora: {item.time}";
                        await stepContext.Context.SendActivityAsync(summaryMedical, cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Lo siento, pero no tienes citas pendientes", cancellationToken: cancellationToken);
                }
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Lo siento, pero no tienes citas pendientes", cancellationToken: cancellationToken);
            }

        }
        private async Task<DialogTurnResult> IntentCrearCita(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(CreateAppointmentDialog), null, cancellationToken);
        }
        private async Task<DialogTurnResult> IntentCalificar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(QualificationDialog), cancellationToken: cancellationToken);
        }

        private async Task IntentVerCentroContacto(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            string phoneDatail =  $"Nuestro números de atención son los siguientes:{Environment.NewLine}" +
                $"📞 +51 943662964{Environment.NewLine}📞 +51 933886728";
            string addressDetail = $"🏦 Estamos ubicados en {Environment.NewLine} Calle Isabel de Valois 44, Boadilla del Monte, Madrid";
            await stepContext.Context.SendActivityAsync(phoneDatail, cancellationToken: cancellationToken);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync(addressDetail, cancellationToken: cancellationToken);
            await Task.Delay(1000);
            await stepContext.Context.SendActivityAsync("¿En qué más te puedo ayudar?", cancellationToken: cancellationToken);
        }
        private async Task IntentVerOptiones(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync($"Estas son las opciones", cancellationToken: cancellationToken);
            await MainOptionsCard.ToShow(stepContext, cancellationToken);
        }

        private async Task IntentSaludar(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var userName = stepContext.Context.Activity.From.Name;
            await stepContext.Context.SendActivityAsync($"Hola {userName}, ¿cómo te puedo ayudar?", cancellationToken: cancellationToken);
        }
        private async Task IntentAgradecer(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync($"No te preocupes, me gusta ayudar", cancellationToken: cancellationToken);
        }
        private async Task IntentDespedir(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync($"Espero verte pronto.", cancellationToken: cancellationToken);
        }

        private async Task IntentNone(WaterfallStepContext stepContext, RecognizerResult luisResult, CancellationToken cancellationToken)
        {
            var resultQnA = await _qnAMakerAIService._qnaMakerResult.GetAnswersAsync(stepContext.Context);

            var score = resultQnA.FirstOrDefault()?.Score;
            string response = resultQnA.FirstOrDefault()?.Answer;

            if (score >= 0.5) {
                await stepContext.Context.SendActivityAsync(response, cancellationToken: cancellationToken);
            } else {
                await stepContext.Context.SendActivityAsync("No te entiendo colega.", cancellationToken: cancellationToken);
                await Task.Delay(1000);
                await IntentVerOptiones(stepContext, luisResult, cancellationToken);
            }
        }
        #endregion


    }
}
