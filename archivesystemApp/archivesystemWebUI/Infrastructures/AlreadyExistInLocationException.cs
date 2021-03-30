using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Infrastructures
{
    public class AlreadyExistInLocationException:Exception
    {
            public AlreadyExistInLocationException()
            {
            }

            public AlreadyExistInLocationException(string message)
                : base(message)
            {
            }

            public AlreadyExistInLocationException(string message, Exception inner)
                : base(message, inner)
            {
            }
        
    }
}