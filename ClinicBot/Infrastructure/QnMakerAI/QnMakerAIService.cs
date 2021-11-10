using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBot.Infrastructure.QnMakerAI
{
    public class QnMakerAIService : IQnMakerAIService
    {
        public QnAMaker _qnaMakerResult { get; set; }

        public QnMakerAIService(IConfiguration configuration)
        {
            _qnaMakerResult = new QnAMaker(new QnAMakerEndpoint { 
                KnowledgeBaseId = configuration["QnAMakerBaseId"],
                EndpointKey = configuration["QnAMakerKey"],
                Host = configuration["QnAMakerHostName"]
            });
        }
    }
}
