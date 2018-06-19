using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSOutlook = Microsoft.Office.Interop.Outlook;

namespace UnderConstruction
{
    class UTools
    {
        public static void SendEmail(string receiver, string subject, string content)
        {
            MSOutlook.Application olApp = new MSOutlook.Application();
            MSOutlook.MailItem mailItem = olApp.CreateItem(MSOutlook.OlItemType.olMailItem);
            mailItem.Subject = subject;
            mailItem.To = receiver;
            mailItem.Body = content;
            mailItem.Display(false);
            mailItem.Send();
        }
    }
}
