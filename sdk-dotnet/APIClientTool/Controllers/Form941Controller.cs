using APIClientTool.Utilities;
using APIClientTool.ViewModels;
using APIClientTool.ViewModels.BaseModels;
using APIClientTool.ViewModels.Form941;
using APIClientTool.ViewModels.Form941CoreModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace APIClientTool.Controllers
{
    public class Form941Controller : Controller
    {
        #region Form 941 View Get Method
        // GET: Form941
        [Route("form941")]
        public ActionResult Form941Return(bool? id)
        {
            Form941Data form941 = new Form941Data();
            bool _prePopulate = id ?? false;
            if (_prePopulate)
            {
                PrePopulate(form941);
            }
            return View(form941);
        }
        #endregion

        #region Pre Populate
        /// <summary>
        /// Pre Populate date
        /// </summary>
        /// <param name="form941"></param>
        private static void PrePopulate(Form941Data form941)
        {
            //Mapping Form 941 
            form941.Sequence = "Record1";
            form941.RecordId = null;
            //Mapping Return Header
            form941.ReturnHeader = new Form941ReturnHeader
            {
                ReturnType = null,
                Qtr = "Q1",
                TaxYr = "2018",
                Business = new Business
                {
                    BusinessId = null,
                    BusinessNm = "Test Business",
                    EINorSSN = "003333330",
                    IsEIN = true,
                    BusinessType = "ESTE",
                    ContactNm = "John Doe",
                    Email = "employer@company.com",
                    Fax = "1234567890",
                    TradeNm = "Kodak",
                    IsBusinessTerminated = false,
                    Phone = "1234566890",
                    PhoneExtn = "12345",
                    IsForeign = false,
                    USAddress = new USAddress
                    {
                        Address1 = "Address Line 1",
                        City = "Rockhill",
                        State = "SC",
                        ZipCd = "29730"
                    },
                    SigningAuthority = new APIClientTool.ViewModels.SigningAuthority
                    {
                        BusinessMemberType = "ADMINISTRATOR",
                        Phone = "1234564390",
                        Name = "John"
                    },
                    KindOfEmployer = null,
                    KindOfPayer = null,
                    ForeignAddress = null,
                },
                BusinessStatusDetails = new BusinessStatusDetails
                {
                    IsBusinessClosed = false,
                    IsBusinessTransferred = false,
                    IsSeasonalEmployer = false,
                    BusinessClosedDetails = null,
                    BusinessTransferredDetails = null
                },
                IsThirdPartyDesignee = true,
                ThirdPartyDesignee = new ThirdPartyDesignee
                {
                    Name = "John Doe",
                    Phone = "1234567890",
                    PIN = "12345"
                },
                SignatureDetails = new SignatureDetails
                {
                    SignatureType = "REPORTING_AGENT",
                    ReportingAgentPIN = new ReportingAgentPIN { PIN = "12345" }
                }


            };

            //Mapping Return Data
            form941.ReturnData = new Form941ReturnData
            {
                DepositScheduleType = new DepositScheduleType
                {
                    DepositorType = DepositorType.MINTAXLIABILITY.ToString(),
                    TaxLiabilityTotalAmt = 200M
                },
                Form941 = new Form941Details
                {
                    EmployeeCnt = 3,
                    WagesAmt = 5750M,
                    FedIncomeTaxWHAmt = 130M,
                    WagesNotSubjToSSMedcrTaxInd = true,
                    Line5aInitialAmt = 564.51M,
                    Line5bInitialAmt = 0M,
                    Line5cInitialAmt = 0M,
                    Line5dInitialAmt = 0M,
                    Line5a = 70M,
                    Line5b = 0M,
                    Line5c = 0M,
                    Line5d = 0M,
                    Line5e = 70M,
                    TaxOnUnreportedTips3121qAmt = 0M,
                    CurrentQtrFractionsCentsAmt = 0M,
                    CurrentQuarterSickPaymentAmt = 0M,
                    CurrQtrTipGrpTermLifeInsAdjAmt = 0M,
                    Line12 = 200M,
                    Line11 = 0M,
                    Line14 = 0M,
                    Line15 = 0M,
                    Line6 = 200M,
                    Line10 = 200M,
                    TotTaxDepositAmt = 200M
                }
            };
        }
        #endregion

        #region Form 941 Create Return Response Status
        /// <summary>
        /// Function inputs Form 941 details, POST all those details to the API and returns the response.
        /// Successful response contains SubmissionId, StatusCode and RecordSuccessStatus details (Sequence, RecordId, RecordStatus etc)
        /// Error response contains StatusCode and RecordErrorStatus details (RecordId, Sequence and list of Error information such as Code, Name, Message and Type)
        /// </summary>
        /// <param name="form941">Form 941details passed through form941 parameter</param>
        /// <returns>Form941CreateReturnResponse</returns>
        [HttpPost]
        public ActionResult APIResponseStatus(Form941Data form941)
        {
            //Hardcoded values for Sequence
            var responseJson = string.Empty;

            form941.Sequence = "Record1";
            form941.RecordId = null;
            form941.ReturnHeader.ReturnType = null;
            form941.ReturnHeader.Business.IsEIN = true;
            form941.ReturnHeader.Business.IsForeign = false;

            if (form941?.ReturnHeader?.ThirdPartyDesignee != null && (!string.IsNullOrEmpty(form941.ReturnHeader.ThirdPartyDesignee.Name) || !string.IsNullOrEmpty(form941.ReturnHeader.ThirdPartyDesignee.Phone) || !string.IsNullOrEmpty(form941.ReturnHeader.ThirdPartyDesignee.PIN)))
            {
                form941.ReturnHeader.IsThirdPartyDesignee = true;
            }

            if (form941?.ReturnHeader?.BusinessStatusDetails != null)
            {
                if (form941.ReturnHeader.BusinessStatusDetails.IsBusinessClosed == false)
                {
                    form941.ReturnHeader.BusinessStatusDetails.BusinessClosedDetails = new BusinessClosedDetails();
                }
                if (form941.ReturnHeader.BusinessStatusDetails.IsBusinessTransferred == false)
                {
                    form941.ReturnHeader.BusinessStatusDetails.BusinessTransferredDetails = new BusinessTransferredDetails();
                }
            }

            var form941Response = new Form941CreateReturnResponse();
            var form941ReturnList = new Form941CreateReturnRequest { Form941Records = new List<Form941Data>() };
            form941ReturnList.Form941Records.Add(form941);

            // Generate JSON for Form 941
            var requestJson = JsonConvert.SerializeObject(form941ReturnList, Formatting.Indented);
            string authType = Utility.GetAppSettings("AuthenticationType");
            if (!string.IsNullOrWhiteSpace(authType) && authType.ToUpper()=="JWT")
            {
                //Get URLs from App.Config
                string oAuthApiUrl = Utility.GetAppSettings("OAuthApiUrl");
                string apiUrl = Utility.GetAppSettings("PublicAPIUrlWithJWT");

                //Call OAuth API 
                using (var oAuthClient = new HttpClient())
                {
                    string oAuthRequestUri = Utility.GetAppSettings("OAuthApiMethodRoute");
                    oAuthClient.BaseAddress = new Uri(oAuthApiUrl);

                    //Generate JWS and get access token (JWT)
                    OAuthGenerator.GenerateJWSAndGetAccessToken(oAuthClient);

                    //Read OAuth API response
                    var response = oAuthClient.GetAsync(oAuthRequestUri).Result;
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        var oauthApiResponse = response.Content.ReadAsAsync<AccessTokenResponse>().Result;
                        if (oauthApiResponse != null && oauthApiResponse.StatusCode == 200)
                        {
                            //Get Access token from OAuth API response
                            string accessToken = oauthApiResponse.AccessToken;
                            //Access token is valid for one hour. After that call OAuth API again & get new Access token.

                            if (!string.IsNullOrWhiteSpace(accessToken))
                            {
                                //Call TaxBandits API using the Access token
                                //Access token is valid for one hour. After that call OAuth API again & get new Access token.
                                using (var apiClient = new HttpClient())
                                {
                                    //API URL to Create Form 941 Return
                                    string requestUri = "Form941/Create";

                                    apiClient.BaseAddress = new Uri(apiUrl);

                                    //Construct HTTP headers
                                    //If Access token got expired, call OAuth API again & get new Access token.
                                    OAuthGenerator.ConstructHeadersWithAccessToken(apiClient, accessToken);

                                    //Get Response
                                    var apiResponse = apiClient.PostAsJsonAsync(requestUri, form941ReturnList).Result;
                                    if (apiResponse != null && response.IsSuccessStatusCode)
                                    {
                                        //Read Response
                                        var createResponse = apiResponse.Content.ReadAsAsync<Form941CreateReturnResponse>().Result;
                                        if (createResponse != null)
                                        {
                                            responseJson = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                            //Deserializing JSON (Success Response) to Form941CreateReturnResponse object
                                            form941Response = new JavaScriptSerializer().Deserialize<Form941CreateReturnResponse>(responseJson);
                                            if (form941Response.SubmissionId != null && form941Response.SubmissionId != Guid.Empty)
                                            {
                                                //Adding Form941CreateReturnResponse Response to Session
                                                APISession.AddForm941APIResponse(form941Response);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var createResponse = apiResponse.Content.ReadAsAsync<Object>().Result;
                                        responseJson = JsonConvert.SerializeObject(createResponse, Formatting.Indented);

                                        //Deserializing JSON (Error Response) to Form941CreateReturnResponse object
                                        form941Response = new JavaScriptSerializer().Deserialize<Form941CreateReturnResponse>(responseJson);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (var client = new PublicAPIClient())
                {
                    //API URL to Create Form 941 Return
                    string requestUri = "Form941/Create";

                    //POST
                    APIGenerateAuthHeader.GenerateAuthHeader(client, requestUri, HttpMethod.Post.ToString());

                    //Get Response
                    var response = client.PostAsJsonAsync(requestUri, form941ReturnList).Result;
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        //Read Response
                        var createResponse = response.Content.ReadAsAsync<Form941CreateReturnResponse>().Result;
                        if (createResponse != null)
                        {
                            responseJson = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                            //Deserializing JSON (Success Response) to Form941CreateReturnResponse object
                            form941Response = new JavaScriptSerializer().Deserialize<Form941CreateReturnResponse>(responseJson);
                            if (form941Response.SubmissionId != null && form941Response.SubmissionId != Guid.Empty)
                            {
                                //Adding Form941CreateReturnResponse Response to Session
                                APISession.AddForm941APIResponse(form941Response);
                            }
                        }
                    }
                    else
                    {
                        var createResponse = response.Content.ReadAsAsync<Object>().Result;
                        responseJson = JsonConvert.SerializeObject(createResponse, Formatting.Indented);

                        //Deserializing JSON (Error Response) to Form941CreateReturnResponse object
                        form941Response = new JavaScriptSerializer().Deserialize<Form941CreateReturnResponse>(responseJson);
                    }
                }
            }
          
            return PartialView(form941Response);
        }
        #endregion

        #region Transmit Return
        /// <summary>
        /// Function transmit the Form 941 Return to Efile
        /// </summary>
        /// <param name="submissionId">SubmissionId passed to transmit the 941 return</param>
        /// <returns>TransmitFormW2Response</returns>
        public ActionResult _TransmitReturn(Guid submissionId)
        {
            var transmitForm941 = new TransmitForm();
            var transmitForm941Response = new TransmitForm941Response();
            var transmitForm941ResponseJSON = string.Empty;
            if (submissionId != null && submissionId != Guid.Empty)
            {
                // Getting the RecordIds for SubmissionId
                transmitForm941 = APISession.GetForm941RecordIdsBySubmissionId(submissionId);

                // Generate JSON for TransmitForm 941
                var requestJson = JsonConvert.SerializeObject(transmitForm941, Formatting.Indented);

                if (transmitForm941 != null)
                {

                    string authType = Utility.GetAppSettings("AuthenticationType");
                    if (!string.IsNullOrWhiteSpace(authType) && authType.ToUpper() == "JWT")
                    {
                        //Get URLs from App.Config
                        string oAuthApiUrl = Utility.GetAppSettings("OAuthApiUrl");
                        string apiUrl = Utility.GetAppSettings("PublicAPIUrlWithJWT");

                        //Call OAuth API 
                        using (var oAuthClient = new HttpClient())
                        {
                            string oAuthRequestUri = Utility.GetAppSettings("OAuthApiMethodRoute");
                            oAuthClient.BaseAddress = new Uri(oAuthApiUrl);

                            //Generate JWS and get access token (JWT)
                            OAuthGenerator.GenerateJWSAndGetAccessToken(oAuthClient);

                            //Read OAuth API response
                            var response = oAuthClient.GetAsync(oAuthRequestUri).Result;
                            if (response != null && response.IsSuccessStatusCode)
                            {
                                var oauthApiResponse = response.Content.ReadAsAsync<AccessTokenResponse>().Result;
                                if (oauthApiResponse != null && oauthApiResponse.StatusCode == 200)
                                {
                                    //Get Access token from OAuth API response
                                    string accessToken = oauthApiResponse.AccessToken;
                                    //Access token is valid for one hour. After that call OAuth API again & get new Access token.

                                    if (!string.IsNullOrWhiteSpace(accessToken))
                                    {
                                        //Call TaxBandits API using the Access token
                                        //Access token is valid for one hour. After that call OAuth API again & get new Access token.
                                        using (var apiClient = new HttpClient())
                                        {
                                            //API URL to Transmit Form 941 Return
                                            string requestUri = "Form941/Transmit";

                                            apiClient.BaseAddress = new Uri(apiUrl);
                                            //Construct HTTP headers
                                            //If Access token got expired, call OAuth API again & get new Access token.
                                            OAuthGenerator.ConstructHeadersWithAccessToken(apiClient, accessToken);

                                            //Get Response
                                            var _response = apiClient.PostAsJsonAsync(requestUri, transmitForm941).Result;
                                            if (_response != null && _response.IsSuccessStatusCode)
                                            {
                                                //Read Response
                                                var createResponse = _response.Content.ReadAsAsync<TransmitForm941Response>().Result;
                                                if (createResponse != null)
                                                {
                                                    transmitForm941ResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                                    transmitForm941Response = new JavaScriptSerializer().Deserialize<TransmitForm941Response>(transmitForm941ResponseJSON);
                                                    if (transmitForm941Response.SubmissionId != null && transmitForm941Response.SubmissionId != Guid.Empty && transmitForm941Response.StatusCode == (int)StatusCode.Success)
                                                    {
                                                        //Updating Filing Status (Transmitted) for a specific SubmissionId in Session 
                                                        APISession.UpdateForm941ReturnFilingStatus(transmitForm941Response.SubmissionId);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var createResponse = _response.Content.ReadAsAsync<Object>().Result;
                                                transmitForm941ResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                                transmitForm941Response = new JavaScriptSerializer().Deserialize<TransmitForm941Response>(transmitForm941ResponseJSON);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var client = new PublicAPIClient())
                        {
                            //API URL to Transmit Form 941 Return
                            string requestUri = "Form941/Transmit";

                            //POST
                            APIGenerateAuthHeader.GenerateAuthHeader(client, requestUri, "POST");

                            //Get Response
                            var _response = client.PostAsJsonAsync(requestUri, transmitForm941).Result;
                            if (_response != null && _response.IsSuccessStatusCode)
                            {
                                //Read Response
                                var createResponse = _response.Content.ReadAsAsync<TransmitForm941Response>().Result;
                                if (createResponse != null)
                                {
                                    transmitForm941ResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                    transmitForm941Response = new JavaScriptSerializer().Deserialize<TransmitForm941Response>(transmitForm941ResponseJSON);
                                    if (transmitForm941Response.SubmissionId != null && transmitForm941Response.SubmissionId != Guid.Empty && transmitForm941Response.StatusCode == (int)StatusCode.Success)
                                    {
                                        //Updating Filing Status (Transmitted) for a specific SubmissionId in Session 
                                        APISession.UpdateForm941ReturnFilingStatus(transmitForm941Response.SubmissionId);
                                    }
                                }
                            }
                            else
                            {
                                var createResponse = _response.Content.ReadAsAsync<Object>().Result;
                                transmitForm941ResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                transmitForm941Response = new JavaScriptSerializer().Deserialize<TransmitForm941Response>(transmitForm941ResponseJSON);
                            }
                        }
                    }
                }
            }
            return PartialView(transmitForm941Response);
        }
        #endregion

        #region Get Efile Status
        /// <summary>
        /// Function returns the Efile status of Form 941
        /// </summary>
        /// <param name="submissionId">SubmissionId is passed to get the efile status</param>
        /// <returns>Form941StatusResponse</returns>
        public ActionResult _GetEfileStatusResponse(Guid submissionId)
        {
            Form941StatusResponse efileStatusResponse = new Form941StatusResponse();
            if (submissionId != null && submissionId != Guid.Empty)
            {
                var efileRequest = new EfileStatusGetRequest { SubmissionId = submissionId };
                var recordIds = APISession.GetForm941RecordIdsBySubmissionId(submissionId);
                if (recordIds != null && recordIds.RecordIds != null && recordIds.RecordIds.Count > 0)
                {
                    efileRequest.RecordIds = recordIds.RecordIds;
                }
                var transmitFormW2ResponseJSON = string.Empty;

                // Request JSON
                var requestJson = JsonConvert.SerializeObject(efileRequest, Formatting.Indented);

                if (submissionId != null && submissionId != Guid.Empty)
                {

                    string authType = Utility.GetAppSettings("AuthenticationType");
                    if (!string.IsNullOrWhiteSpace(authType) && authType.ToUpper() == "JWT")
                    {
                        //Get URLs from App.Config
                        string oAuthApiUrl = Utility.GetAppSettings("OAuthApiUrl");
                        string apiUrl = Utility.GetAppSettings("PublicAPIUrlWithJWT");

                        //Call OAuth API 
                        using (var oAuthClient = new HttpClient())
                        {
                            string oAuthRequestUri = Utility.GetAppSettings("OAuthApiMethodRoute");
                            oAuthClient.BaseAddress = new Uri(oAuthApiUrl);

                            //Generate JWS and get access token (JWT)
                            OAuthGenerator.GenerateJWSAndGetAccessToken(oAuthClient);

                            //Read OAuth API response
                            var response = oAuthClient.GetAsync(oAuthRequestUri).Result;
                            if (response != null && response.IsSuccessStatusCode)
                            {
                                var oauthApiResponse = response.Content.ReadAsAsync<AccessTokenResponse>().Result;
                                if (oauthApiResponse != null && oauthApiResponse.StatusCode == 200)
                                {
                                    //Get Access token from OAuth API response
                                    string accessToken = oauthApiResponse.AccessToken;
                                    //Access token is valid for one hour. After that call OAuth API again & get new Access token.

                                    if (!string.IsNullOrWhiteSpace(accessToken))
                                    {
                                        //Call TaxBandits API using the Access token
                                        //Access token is valid for one hour. After that call OAuth API again & get new Access token.
                                        using (var apiClient = new HttpClient())
                                        {
                                            //POST
                                            string requestUri = "Form941/Status";

                                            apiClient.BaseAddress = new Uri(apiUrl);
                                            //Construct HTTP headers
                                            //If Access token got expired, call OAuth API again & get new Access token.
                                            OAuthGenerator.ConstructHeadersWithAccessToken(apiClient, accessToken);

                                            //Read Response
                                            var _response = apiClient.PostAsJsonAsync(requestUri, efileRequest).Result;
                                            if (_response != null && _response.IsSuccessStatusCode)
                                            {
                                                var createResponse = _response.Content.ReadAsAsync<Form941StatusResponse>().Result;
                                                if (createResponse != null)
                                                {
                                                    transmitFormW2ResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                                    efileStatusResponse = new JavaScriptSerializer().Deserialize<Form941StatusResponse>(transmitFormW2ResponseJSON);
                                                }
                                            }
                                            else
                                            {
                                                var createResponse = _response.Content.ReadAsAsync<Object>().Result;
                                                transmitFormW2ResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                                efileStatusResponse = new JavaScriptSerializer().Deserialize<Form941StatusResponse>(transmitFormW2ResponseJSON);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var client = new PublicAPIClient())
                        {
                            //POST
                            string requestUri = "Form941/Status";

                            //Get Response
                            APIGenerateAuthHeader.GenerateAuthHeader(client, requestUri, "POST");

                            //Read Response
                            var _response = client.PostAsJsonAsync(requestUri, efileRequest).Result;
                            if (_response != null && _response.IsSuccessStatusCode)
                            {
                                var createResponse = _response.Content.ReadAsAsync<Form941StatusResponse>().Result;
                                if (createResponse != null)
                                {
                                    transmitFormW2ResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                    efileStatusResponse = new JavaScriptSerializer().Deserialize<Form941StatusResponse>(transmitFormW2ResponseJSON);
                                }
                            }
                            else
                            {
                                var createResponse = _response.Content.ReadAsAsync<Object>().Result;
                                transmitFormW2ResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                efileStatusResponse = new JavaScriptSerializer().Deserialize<Form941StatusResponse>(transmitFormW2ResponseJSON);
                            }
                        }
                    }
                }
            }
            return PartialView(efileStatusResponse);
        }
        #endregion

        #region  Business Members List
        [HttpGet]
        public ActionResult BusinessMembersList(string id)
        {
            var businessType = id;
            if (!string.IsNullOrWhiteSpace(businessType))
            {
                var businessMembersList = new List<SelectListItem>();
                if (businessType == BusinessType.ESTE.ToString())
                {
                    businessMembersList = Enum.GetValues(typeof(EstateBusinessMembers)).Cast<EstateBusinessMembers>().Select(x => new SelectListItem { Text = Utility.GetEnumDisplayName(x), Value = (x).ToString() }).ToList();
                }
                else if (businessType == BusinessType.PART.ToString())
                {
                    businessMembersList = Enum.GetValues(typeof(PartnershipBusinessMembers)).Cast<PartnershipBusinessMembers>().Select(x => new SelectListItem { Text = Utility.GetEnumDisplayName(x), Value = (x).ToString() }).ToList();
                }
                else if (businessType == BusinessType.CORP.ToString())
                {
                    businessMembersList = Enum.GetValues(typeof(CorporationBusinessMembers)).Cast<CorporationBusinessMembers>().Select(x => new SelectListItem { Text = Utility.GetEnumDisplayName(x), Value = (x).ToString() }).ToList();
                }
                else if (businessType == BusinessType.EORG.ToString())
                {
                    businessMembersList = Enum.GetValues(typeof(EstateBusinessMembers)).Cast<EstateBusinessMembers>().Select(x => new SelectListItem { Text = Utility.GetEnumDisplayName(x), Value = (x).ToString() }).ToList();
                }
                else if (businessType == BusinessType.SPRO.ToString())
                {
                    businessMembersList = Enum.GetValues(typeof(SoleProprietorshipBusinessMembers)).Cast<SoleProprietorshipBusinessMembers>().Select(x => new SelectListItem { Text = Utility.GetEnumDisplayName(x), Value = (x).ToString() }).ToList();
                }
                return Json(businessMembersList, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Form 941
        /// <summary>
        /// Function get the Form 941 Return to Efile
        /// </summary>
        /// <param name="submissionId">SubmissionId passed to get the 941 return</param>
        /// <returns>Form941GetReturnResponse</returns>
        public ActionResult GetForm941(Guid submissionId)
        {
            var getReturnResponse = new Form941GetReturnResponse();
            var getReturnResponseJSON = string.Empty;
            if (submissionId != null && submissionId != Guid.Empty)
            {

                string authType = Utility.GetAppSettings("AuthenticationType");
                if (!string.IsNullOrWhiteSpace(authType) && authType.ToUpper() == "JWT")
                {
                    //Get URLs from App.Config
                    string oAuthApiUrl = Utility.GetAppSettings("OAuthApiUrl");
                    string apiUrl = Utility.GetAppSettings("PublicAPIUrlWithJWT");

                    //Call OAuth API 
                    using (var oAuthClient = new HttpClient())
                    {
                        string oAuthRequestUri = Utility.GetAppSettings("OAuthApiMethodRoute");
                        oAuthClient.BaseAddress = new Uri(oAuthApiUrl);

                        //Generate JWS and get access token (JWT)
                        OAuthGenerator.GenerateJWSAndGetAccessToken(oAuthClient);

                        //Read OAuth API response
                        var response = oAuthClient.GetAsync(oAuthRequestUri).Result;
                        if (response != null && response.IsSuccessStatusCode)
                        {
                            var oauthApiResponse = response.Content.ReadAsAsync<AccessTokenResponse>().Result;
                            if (oauthApiResponse != null && oauthApiResponse.StatusCode == 200)
                            {
                                //Get Access token from OAuth API response
                                string accessToken = oauthApiResponse.AccessToken;
                                //Access token is valid for one hour. After that call OAuth API again & get new Access token.

                                if (!string.IsNullOrWhiteSpace(accessToken))
                                {
                                    //Call TaxBandits API using the Access token
                                    //Access token is valid for one hour. After that call OAuth API again & get new Access token.
                                    using (var apiClient = new HttpClient())
                                    {
                                        //API URL to Get Form 941 Return
                                        string requestUri = "Form941/Get?submissionId=" + submissionId;

                                        apiClient.BaseAddress = new Uri(apiUrl);
                                        //Construct HTTP headers
                                        //If Access token got expired, call OAuth API again & get new Access token.
                                        OAuthGenerator.ConstructHeadersWithAccessToken(apiClient, accessToken);

                                        //Get Response
                                        var _response = apiClient.GetAsync(requestUri).Result;
                                        if (_response != null && _response.IsSuccessStatusCode)
                                        {
                                            //Read Response
                                            var createResponse = _response.Content.ReadAsAsync<Form941GetReturnResponse>().Result;
                                            if (createResponse != null)
                                            {
                                                getReturnResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                                getReturnResponse = new JavaScriptSerializer().Deserialize<Form941GetReturnResponse>(getReturnResponseJSON);
                                                if (getReturnResponse != null && getReturnResponse.StatusCode == (int)StatusCode.Success)
                                                {
                                                    ViewData["GetResponseJSON"] = getReturnResponseJSON;
                                                    return PartialView();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var createResponse = _response.Content.ReadAsAsync<Object>().Result;
                                            getReturnResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                            getReturnResponse = new JavaScriptSerializer().Deserialize<Form941GetReturnResponse>(getReturnResponseJSON);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (var client = new PublicAPIClient())
                    {
                        //API URL to Get Form 941 Return
                        string requestUri = "Form941/Get?submissionId=" + submissionId;

                        //Get
                        APIGenerateAuthHeader.GenerateAuthHeader(client, requestUri, "GET");

                        //Get Response
                        var _response = client.GetAsync(requestUri).Result;
                        if (_response != null && _response.IsSuccessStatusCode)
                        {
                            //Read Response
                            var createResponse = _response.Content.ReadAsAsync<Form941GetReturnResponse>().Result;
                            if (createResponse != null)
                            {
                                getReturnResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                getReturnResponse = new JavaScriptSerializer().Deserialize<Form941GetReturnResponse>(getReturnResponseJSON);
                                if (getReturnResponse != null && getReturnResponse.StatusCode == (int)StatusCode.Success)
                                {
                                    ViewData["GetResponseJSON"] = getReturnResponseJSON;
                                    return PartialView();
                                }
                            }
                        }
                        else
                        {
                            var createResponse = _response.Content.ReadAsAsync<Object>().Result;
                            getReturnResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                            getReturnResponse = new JavaScriptSerializer().Deserialize<Form941GetReturnResponse>(getReturnResponseJSON);
                        }
                    }
                }



                
            }
            return PartialView(getReturnResponse);
        }
        #endregion

        #region Delete Return
        /// <summary>
        /// Function delete the Form 941 Return to Efile
        /// </summary>
        /// <param name="submissionId">SubmissionId passed to delete the 941 return</param>
        /// <returns>DeleteReturnResponse</returns>
        public ActionResult Delete(Guid submissionId)
        {
            var deleteReturnRequest = new DeleteReturnRequest();
            var deleteReturnResponse = new DeleteReturnResponse();
            var deleteReturnResponseJSON = string.Empty;
            if (submissionId != null && submissionId != Guid.Empty)
            {
                deleteReturnRequest.SubmissionId = submissionId;
                // Getting the RecordIds for SubmissionId
                var recordIdsFromSession = APISession.GetForm941RecordIdsBySubmissionId(submissionId);
                deleteReturnRequest.RecordIds = recordIdsFromSession != null ? recordIdsFromSession.RecordIds : null;
                if (deleteReturnRequest.RecordIds != null && deleteReturnRequest.RecordIds.Count > 0)
                {
                    string authType = Utility.GetAppSettings("AuthenticationType");
                    if (!string.IsNullOrWhiteSpace(authType) && authType.ToUpper() == "JWT")
                    {
                        //Get URLs from App.Config
                        string oAuthApiUrl = Utility.GetAppSettings("OAuthApiUrl");
                        string apiUrl = Utility.GetAppSettings("PublicAPIUrlWithJWT");

                        //Call OAuth API 
                        using (var oAuthClient = new HttpClient())
                        {
                            string oAuthRequestUri = Utility.GetAppSettings("OAuthApiMethodRoute");
                            oAuthClient.BaseAddress = new Uri(oAuthApiUrl);

                            //Generate JWS and get access token (JWT)
                            OAuthGenerator.GenerateJWSAndGetAccessToken(oAuthClient);

                            //Read OAuth API response
                            var response = oAuthClient.GetAsync(oAuthRequestUri).Result;
                            if (response != null && response.IsSuccessStatusCode)
                            {
                                var oauthApiResponse = response.Content.ReadAsAsync<AccessTokenResponse>().Result;
                                if (oauthApiResponse != null && oauthApiResponse.StatusCode == 200)
                                {
                                    //Get Access token from OAuth API response
                                    string accessToken = oauthApiResponse.AccessToken;
                                    //Access token is valid for one hour. After that call OAuth API again & get new Access token.

                                    if (!string.IsNullOrWhiteSpace(accessToken))
                                    {
                                        //Call TaxBandits API using the Access token
                                        //Access token is valid for one hour. After that call OAuth API again & get new Access token.
                                        using (var apiClient = new HttpClient())
                                        {
                                            //API URL to Transmit Form 941 Return
                                            string requestUri = "Form941/Delete";

                                            apiClient.BaseAddress = new Uri(apiUrl);
                                            //Construct HTTP headers
                                            //If Access token got expired, call OAuth API again & get new Access token.
                                            OAuthGenerator.ConstructHeadersWithAccessToken(apiClient, accessToken);

                                            //Get Response
                                            var _response = apiClient.PostAsJsonAsync(requestUri, deleteReturnRequest).Result;
                                            if (_response != null && _response.IsSuccessStatusCode)
                                            {
                                                //Read Response
                                                var createResponse = _response.Content.ReadAsAsync<DeleteReturnResponse>().Result;
                                                if (createResponse != null)
                                                {
                                                    deleteReturnResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                                    deleteReturnResponse = new JavaScriptSerializer().Deserialize<DeleteReturnResponse>(deleteReturnResponseJSON);
                                                    if (deleteReturnResponse != null && deleteReturnResponse.StatusCode == (int)StatusCode.Success)
                                                    {
                                                        //Remove Submission and RecordId from session
                                                        APISession.DeleteForm941APIResponse(submissionId);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var createResponse = _response.Content.ReadAsAsync<Object>().Result;
                                                deleteReturnResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                                deleteReturnResponse = new JavaScriptSerializer().Deserialize<DeleteReturnResponse>(deleteReturnResponseJSON);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        using (var client = new PublicAPIClient())
                        {
                            //API URL to Transmit Form 941 Return
                            string requestUri = "Form941/Delete";

                            //POST
                            APIGenerateAuthHeader.GenerateAuthHeader(client, requestUri, "POST");

                            //Get Response
                            var _response = client.PostAsJsonAsync(requestUri, deleteReturnRequest).Result;
                            if (_response != null && _response.IsSuccessStatusCode)
                            {
                                //Read Response
                                var createResponse = _response.Content.ReadAsAsync<DeleteReturnResponse>().Result;
                                if (createResponse != null)
                                {
                                    deleteReturnResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                    deleteReturnResponse = new JavaScriptSerializer().Deserialize<DeleteReturnResponse>(deleteReturnResponseJSON);
                                    if (deleteReturnResponse != null && deleteReturnResponse.StatusCode == (int)StatusCode.Success)
                                    {
                                        //Remove Submission and RecordId from session
                                        APISession.DeleteForm941APIResponse(submissionId);
                                    }
                                }
                            }
                            else
                            {
                                var createResponse = _response.Content.ReadAsAsync<Object>().Result;
                                deleteReturnResponseJSON = JsonConvert.SerializeObject(createResponse, Formatting.Indented);
                                deleteReturnResponse = new JavaScriptSerializer().Deserialize<DeleteReturnResponse>(deleteReturnResponseJSON);
                            }
                        }
                    }
                }
            }
            return PartialView(deleteReturnResponse);
        }
        #endregion

    }
}