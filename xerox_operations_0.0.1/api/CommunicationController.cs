using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace xerox_operations_0._0._1.api
{
    public class CommunicationController : ApiController
    {
        [HttpGet]
        public string AddItem(String Item)
        {
            // CommonObjects.mainFormReference.addItemToMethod(Item);
            return "OK";
        }

        [HttpGet]
        public string DeleteItem(String Item)
        {
            
            return "OK";
        }

        [HttpGet]
        public string AddDriveProgress(String Item)
        {
            CommonObjects.mainFormReference.onDriveProgressBar(Int32.Parse(Item));
            return "OK";
        }

        [HttpGet]
        public string AddDriveFreeSpaceAvailable(String Item)
        {
            CommonObjects.mainFormReference.onDriveFreeSpaceAvailableLabelChanged(Int32.Parse(Item));
            return "OK";
        }
    }

    public class CommonObjects
    {
        public static MainForm mainFormReference;
    }
}
