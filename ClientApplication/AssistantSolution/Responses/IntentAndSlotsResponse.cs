using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssistantSolution.Responses
{
    class IntentAndSlotsResponse
    {
        public string Intent;
        public List<string> Slots;
        
        public IntentAndSlotsResponse(string intent, List<string> slots)
        {
            Intent = intent;
            Slots = slots;
        }
        ~IntentAndSlotsResponse() { }
    }
}
