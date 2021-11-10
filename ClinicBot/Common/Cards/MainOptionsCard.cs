using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClinicBot.Common.Cards
{
    public class MainOptionsCard
    {
        public static async Task ToShow(DialogContext stepContext, CancellationToken cancelationToken) {
            await stepContext.Context.SendActivityAsync(activity: CreateCarusel(), cancelationToken);
        }

        private static Activity CreateCarusel() {
            var cardCitasMedicas = new HeroCard
            { 
                Title = "Citas Medicas",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://clinicbotstoragedrs.blob.core.windows.net/images/menu_opt_01.jpg") },
                Buttons = new List<CardAction>() { 
                    new CardAction() { 
                        Title = "Crear cita médica",
                        Value = "Crear cita médica",
                        Type = ActionTypes.ImBack
                    },
                    new CardAction() {
                        Title = "Ver mi cita",
                        Value = "Ver mi cita",
                        Type = ActionTypes.ImBack
                    }
                }
            };

            var cardInformacionContacto = new HeroCard
            {
                Title = "Información de contacto",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://clinicbotstoragedrs.blob.core.windows.net/images/menu_opt_02.jpg") },
                Buttons = new List<CardAction>() {
                    new CardAction() {
                        Title = "Centro de contacto",
                        Value = "Centro de contacto",
                        Type = ActionTypes.ImBack
                    },
                    new CardAction() {
                        Title = "Sitio web",
                        Value = "https://www.sanitas.es/",
                        Type = ActionTypes.OpenUrl
                    }
                }
            };

            var cardSiguenosRedesSociales = new HeroCard
            {
                Title = "Siguenos en las redes",
                Subtitle = "Opciones",
                Images = new List<CardImage> { new CardImage("https://clinicbotstoragedrs.blob.core.windows.net/images/menu_opt_03.jpg") },
                Buttons = new List<CardAction>() {
                    new CardAction() {
                        Title = "Facebook",
                        Value = "https://www.facebook.com/YudnerParedes",
                        Type = ActionTypes.OpenUrl
                    },
                    new CardAction() {
                        Title = "Instagram",
                        Value = "https://www.instagram.com/yudner_paredes",
                        Type = ActionTypes.OpenUrl
                    },
                    new CardAction() {
                        Title = "Twitter",
                        Value = "https://www.twitter.com/YudnerParedes",
                        Type = ActionTypes.OpenUrl
                    }
                }
            };

            var cardCalificacion = new HeroCard
            {
                Title = "Calificación",
                Subtitle = "Calificación",
                Images = new List<CardImage> { new CardImage("https://clinicbotstoragedrs.blob.core.windows.net/images/menu_opt_04.jpg") },
                Buttons = new List<CardAction>() {
                    new CardAction() {
                        Title = "Calificar Bot",
                        Value = "Calificar Bot",
                        Type = ActionTypes.ImBack
                    }
                }
            };

            var optionsAttachments = new List<Attachment>() {
                cardCitasMedicas.ToAttachment(),
                cardInformacionContacto.ToAttachment(),
                cardSiguenosRedesSociales.ToAttachment(),
                cardCalificacion.ToAttachment()
            };

            var reply = MessageFactory.Attachment(optionsAttachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            return reply as Activity;
        }
    }
}
