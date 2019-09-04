using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Kobe.Common.ViewModels
{
    public class ResponseViewModel
    {
        public ResponseViewModel()
        {
            Status = 0;
            Message = Constants.Error;
        }
        public string Message { get; set; }
        public object ResponseData { get; set; }
        public int Status { get; set; }
        public int Total { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Constants
    {
        public const string Error = "Some internal error occurred";
        public const string Success = "Data saved successfully";
        public const string Delete = "Data deleted successfully";
        public const string Warning = "Data is not in proper format";
        public const string Retreived = "Data retrieved successfully";
        public const string NotFound = "Data not found";
    }
}
