﻿using Microsoft.AspNetCore.Mvc;
using PrefinalsWebSys1.Models;
using PrefinalsWebSys1.ViewModels;
using System.Collections.Concurrent;
using System.Data.Common;

namespace PrefinalsWebSys1.Controllers
{
    public class ConnectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Messages()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(MessageViewModel req)
        {
            var resp = new MessageViewModel();

            using (var db = new SmileBookDBContext())
            {
                var newMessage = new UserMessage() { 
                    FromUserID = req.SenderUserID,
                    MessageBody = req.SenderMessage,
                    MessageType = "NORMAL",
                    Priorty=0,
                    SentDate = DateTime.Now,
                    ReceivedDate = DateTime.Now,
                    ToUserID = req.ReceiverUserID,
                    IsDeleted = false,
                    CreatedBy="SYSTEM",
                    ModifiedBy = "SYSTEM",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                };

                db.UserMessages.Add(newMessage);
                db.SaveChanges();

                resp.SenderUserID = req.SenderUserID;
                resp.MessageID = newMessage.ID;
                resp.Status = "SENT";

            }

            return Json(resp);
        }

        public IActionResult GetMessages()
        {
            var mydata = System.IO.File.ReadAllText("App_Data/messages.txt").Replace("\\n","<br>").Replace("\\r", "<br>");

            return Content(mydata);
        }

        [HttpGet]
        public HttpResponseMessage HasChanges(int PortalUserID)//, DateTime LastSyncDT)
        {
            CancellationToken clientDisconnectToken = new CancellationToken();
            var response = new HttpResponseMessage();
            response.Content = new PushStreamContent(async (stream, httpContent, transportContext) =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    using (var consumer = new BlockingCollection<string>())
                    {
                        var eventGeneratorTask = EventGeneratorAsync(PortalUserID, consumer, clientDisconnectToken);
                        foreach (var @event in consumer.GetConsumingEnumerable(clientDisconnectToken))
                        {
                            await writer.WriteLineAsync("data: " + @event);
                            await writer.WriteLineAsync();
                            await writer.FlushAsync();
                        }
                        await eventGeneratorTask;
                    }
                }
            }, "text/event-stream");
            return response;
        }

        private async Task EventGeneratorAsync(int PortalUserID, BlockingCollection<string> producer, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    int newNotif = 0;
                    if (newNotif > 0)
                    {
                        //var evtNotif = new UserEventNotificationVM()
                        //{
                        //    PortalUserID = PortalUserID,
                        //    DateChecked = DateTime.UtcNow.ToString(),
                        //    NewNotificationCount = newNotif
                        //};
                        //var evtNotifJSON = ObjectMapper.ObjectToJson(evtNotif).Replace("\n", "").Replace("\r", "").Replace("\t", "");//clearing indention,new lines and carriage return
                        //producer.Add(evtNotifJSON);
                    }
                    else
                    {
                        //var sampleNotif = new UserEventNotificationVM()
                        //{
                        //    PortalUserID = PortalUserID,
                        //    DateChecked = DateTime.UtcNow.ToString(),
                        //    NewNotificationCount = 0
                        //};
                        //var evtNotifJSON = ObjectMapper.ObjectToJson(sampleNotif).Replace("\n", "").Replace("\r", "").Replace("\t", "");//clearing indention,new lines and carriage return
                        //producer.Add(evtNotifJSON);
                    }
                    await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                producer.CompleteAdding();
            }
        }

    }
}
