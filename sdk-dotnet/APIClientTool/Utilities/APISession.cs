﻿using APIClientTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIClientTool.Utilities
{
    public class APISession
    {
        #region Session Property for Return
        public static List<FormW2ReturnResponse> ReturnResponses
        {
            get
            {
                if(HttpContext.Current.Session["ReturnResponse"] != null)
                {
                    return (List<FormW2ReturnResponse>)HttpContext.Current.Session["ReturnResponse"];
                }
                else
                {
                    return new List<FormW2ReturnResponse>();
                }
            }
            set
            {
                if(value != null)
                {
                    HttpContext.Current.Session["ReturnResponse"] = value;
                }
            }
        }
        #endregion

        #region Add API Response
        /// <summary>
        /// Add API Response
        /// </summary>
        /// <param name="returnResponse"></param>
        public static void AddAPIResponse(FormW2ReturnResponse returnResponse)
        {           
            if (returnResponse != null && returnResponse.SubmissionId != Guid.Empty)
            {
                List<FormW2ReturnResponse> returnResponses = ReturnResponses;
                returnResponses.Add(returnResponse);
               ReturnResponses = returnResponses;
            }
        }
        #endregion

        #region Get API Response 
        /// <summary>
        ///  Get API Response 
        /// </summary>
        /// <returns></returns>
        public static List<EFileStatus> GetAPIResponse()
        {
            List<EFileStatus> formW2list = new List<EFileStatus>();
            List<FormW2ReturnResponse> returnResponses = ReturnResponses;
            if (returnResponses != null && returnResponses.Count > 0)
            {
                var apiResponse = returnResponses.Where(a => a.StatusCode == (int)StatusCode.Success && a.SubmissionId != Guid.Empty).ToList();
                foreach (var submission in apiResponse)
                {
                    var formW2 = new EFileStatus();
                    formW2.SubmissionId = submission.SubmissionId;
                    formW2.IsReturnTransmitted = submission.IsReturnTransmitted;
                    formW2list.Add(formW2);
                }
            }
            return formW2list;
        }
        #endregion

        #region Get RecordIDs by SubmissionId
        /// <summary>
        ///  Get RecordIDs by SubmissionId
        /// </summary>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        public static TransmitForm GetRecordIdsBySubmissionId(Guid submissionId)
        {
            TransmitForm transmitForm = new TransmitForm();
            List<FormW2ReturnResponse> returnResponses = ReturnResponses;
            if (submissionId != Guid.Empty)
            {
                transmitForm.SubmissionId = submissionId;
                if (returnResponses != null && returnResponses.Count > 0)
                {
                    var returnResponse = returnResponses.Where(r => r.SubmissionId == submissionId).SingleOrDefault();
                    if(returnResponse != null && returnResponse.FormW2Records != null 
                        && returnResponse.FormW2Records.SuccessRecords != null && returnResponse.FormW2Records.SuccessRecords.Count > 0)
                    {
                        var recordIds = returnResponse.FormW2Records.SuccessRecords.Select(r => r.RecordId).ToList();
                        if (recordIds != null && recordIds.Any())
                        {
                            transmitForm.RecordIds = new List<Guid>();
                            foreach (var recordId in recordIds)
                            {
                                transmitForm.RecordIds.Add(recordId ?? Guid.Empty);
                            }
                        }
                    }
                }
            }
            return transmitForm;
        }
        #endregion

        #region Update Filing Status
        /// <summary>
        /// Update Filing Status
        /// </summary>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        public static bool UpdateFilingStatus(Guid submissionId)
        {
            bool isUpdated = false;
            if (submissionId != Guid.Empty)
            {
                if (ReturnResponses != null && ReturnResponses.Count > 0)
                {
                    var apiResponse = ReturnResponses.Where(a => a.SubmissionId == submissionId).SingleOrDefault();
                    apiResponse.IsReturnTransmitted = true;
                    isUpdated = true;
                }
            }

            return isUpdated;
        }
        #endregion
    }
}