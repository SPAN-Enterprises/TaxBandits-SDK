﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIClientTool.ViewModels.Form941SCHR
{
    public class Form941SchRReturnResponse
    {

        public Guid SubmissionId { get; set; }
        public FormRecords Form941SchRRecords { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string StatusName { get; set; }
        public List<Error> Errors { get; set; }
    }
}