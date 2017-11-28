﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIClientTool.ViewModels
{
    public enum StatusCode
    {
        Continue = 100,
        Success = 200,
        MultipleChoices = 300,
        BadRequest = 400,
        Unauthorized = 401,
        NotSufficient = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowsed = 405,
        RequestLimitExceeded = 429,
        InternalServerError = 500,
        NotAvailable = 503
    }

    public class EntityBase
    {
        public DateTime CreatedTimeStamp { get; set; }
        public DateTime UpdatedTimeStamp { get; set; }
        public bool IsDeleted { get; set; }
    }
}