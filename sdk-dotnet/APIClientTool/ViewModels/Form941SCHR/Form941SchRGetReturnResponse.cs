﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIClientTool.ViewModels.Form941SCHR
{
    public class Form941SchRGetReturnResponse
    {
        /// <summary>
        /// Submission Id
        /// </summary>
        public Guid SubmissionId { get; set; }
        /// <summary>
        /// A detailed information about the Business, Employees and Form 941 records
        /// </summary>
        public List<Form941SchRRecords> Form941SchRRecords { get; set; }
        /// <summary>
        /// It will show the detailed information about the error.
        /// </summary>
        public List<Error> Errors { get; set; }
        /// <summary>
        /// It will return the status codes like 200, 300, etc., Refer Status Codes.
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// It will return the name of status code.
        /// </summary>
        public string StatusMessage { get; set; }
        /// <summary>
        /// It will return the detailed message of status code.
        /// </summary>
        public string StatusName { get; set; }
    }
}