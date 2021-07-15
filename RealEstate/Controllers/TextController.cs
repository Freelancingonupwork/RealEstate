using Microsoft.AspNetCore.Mvc;
using RealEstate.Utills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace RealEstate.Controllers
{
    //[Route("api/[controller]")]
    //[Route("[controller]/[action]")]
    //[ApiController]
    public class TextController : TwilioController
    {
        //[HttpPost]
        //public async Task<IActionResult> Post()
        //{
        //    var from = Request.Form["From"];
        //    var body = Request.Form["Body"];
        //    var testContent = $@"<Response><Message>your number:{from} and your message: '{body}'</Message></Response>";
        //    return Content(testContent, "text/xml");
        //}

        public IActionResult ReceiveSms()
        {
            var response = new MessagingResponse();
            ErrorLog.log(response.ToString());
            response.Message("The Robots are coming! Head for the hills!");

            return TwiML(response);
        }
    }
}
