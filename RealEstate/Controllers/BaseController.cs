using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Controllers
{
    public class BaseController : Controller
    {
        [NonAction]
        public void ShowErrorMessage(string message, bool persistMessage = false)
        {
            ShowMessage(message, "error", persistMessage);
        }

        [NonAction]
        public void ShowSuccessMessage(string message, bool persistMessage = false)
        {
            ShowMessage(message, "success", persistMessage);
        }

        [NonAction]
        public void ShowWarningMessage(string message, bool persistMessage = false)
        {
            ShowMessage(message, "warning", persistMessage);
        }

        private void ShowMessage(string message, string messageType, bool persistMessage = false)
        {
            if (persistMessage)
            {
                TempData["MessageType"] = messageType;
                TempData["Message"] = message;
            }
            else
            {
                ViewBag.MessageType = messageType;
                ViewBag.Message = message;
            }
        }

        [NonAction]
        public ActionResult ResourceNotFound()
        {
            return RedirectToAction("ResourceNotFound");
        }
    }
}
