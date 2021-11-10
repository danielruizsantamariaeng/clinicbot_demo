using Microsoft.Bot.Builder.AI.QnA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBot.Infrastructure.QnMakerAI
{
    public interface IQnMakerAIService
    {
        public QnAMaker _qnaMakerResult { get; set; }
    }
}
