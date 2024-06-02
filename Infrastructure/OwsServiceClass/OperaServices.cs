using BussinessLogic.Abstractions;
using Domain;
using Domain.Response;
using Domain.Responses;
using InformationService;
using Infrastructure.OwsServiceClass.OwsHelper;
using Infrastructure.ResponseDTO;
using ReservationService;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using static ReservationService.ReservationServiceSoapClient;

namespace Infrastructure.OwsHelper
{
    public class OperaServices: IOperaService
    {

        InfraConfigutations? config;
        public OperaServices(InfraConfigutations _config)
        {
            config = _config;
        }
        async Task<ServiceResponse<object?>> IOperaService.MakePayment(OwsRequestModel Request)
        {
            #region OperaCredential
            Request.LegNumber = config?.PaymentSettings.LegNumber;
            Request.Username = config?.PaymentSettings.Username;
            Request.Language = config?.PaymentSettings.Language;
            Request.ChainCode = config?.PaymentSettings.ChainCode;
            Request.DestinationEntityID = config?.PaymentSettings.DestinationEntityID;
            Request.DestinationSystemType = config?.PaymentSettings.DestinationSystemType;
            Request.HotelDomain = config?.PaymentSettings.HotelDomain;
            Request.KioskID = config?.PaymentSettings?.KioskID;
            Request.SystemType = config?.PaymentSettings?.SystemType;
            Request.Password = config?.PaymentSettings?.Password;

            #endregion
            DateTime? PostingDate = null;
            #region Getting BusinessDate

            #region Request Header
            string temp1 = Helper.Get8Digits();
            InformationService.OGHeader OG = new InformationService.OGHeader();
            OG.transactionID = temp1;
            OG.timeStamp = DateTime.Now;
            OG.primaryLangID = Request.Language; //English
            InformationService.EndPoint orginEndPnt = new InformationService.EndPoint();
            orginEndPnt.entityID = Request.KioskID; //Kiosk Identifier
            orginEndPnt.systemType = Request.SystemType;
            OG.Origin = orginEndPnt;
            InformationService.EndPoint destEndPnt = new InformationService.EndPoint();
            destEndPnt.entityID = Request.DestinationEntityID;
            destEndPnt.systemType = Request.DestinationSystemType;
            OG.Destination = destEndPnt;
            InformationService.OGHeaderAuthentication Authnt = new InformationService.OGHeaderAuthentication();
            InformationService.OGHeaderAuthenticationUserCredentials userCrdntls = new InformationService.OGHeaderAuthenticationUserCredentials();
            userCrdntls.UserName = Request.Username;
            userCrdntls.UserPassword = Request.Password;
            userCrdntls.Domain = Request.HotelDomain;
            Authnt.UserCredentials = userCrdntls;
            OG.Authentication = Authnt;
            #endregion

            #region Request Body
            InformationService.LovRequest LOVReq = new InformationService.LovRequest();
            InformationService.LovResponse LOVRes = new InformationService.LovResponse();

            InformationService.LovQueryType2 LOVQuery = new InformationService.LovQueryType2();
            LOVQuery.LovIdentifier = "BUSINESSDATE";
            LOVReq.Item = LOVQuery;
            #endregion

            InformationService.InformationSoapClient InfoPortClient = new InformationService.InformationSoapClient(InformationSoapClient.EndpointConfiguration.InformationSoap);
            bool? isOperaCloudEnabled1 = false;
            isOperaCloudEnabled1 = config?.PaymentSettings!.OperaCloudEnabled;
            if (isOperaCloudEnabled1 is not null && isOperaCloudEnabled1==true)
            {
                InfoPortClient.Endpoint.EndpointBehaviors.Add(new CustomEndpointBehaviour(config?.PaymentSettings!.WSSEUserName, config?.PaymentSettings!.WSSEPassword,
                                        Request.Username, Request.Password, Request.HotelDomain));
            }
            LOVRes = InfoPortClient.QueryLov(ref OG,LOVReq);
            if (LOVRes.Result.resultStatusFlag == InformationService.ResultStatusFlag.SUCCESS)
            {
                string date = null;
                string month = null;
                string year = null;
                foreach (InformationService.LovQueryResultType Value in LOVRes.LovQueryResult)
                {
                    if (!string.IsNullOrEmpty(Value.qualifierType) && Value.qualifierType.Equals("Day"))
                        date = Value.qualifierValue;
                    if (!string.IsNullOrEmpty(Value.secondaryQualifierType) && Value.secondaryQualifierType.Equals("Month"))
                        month = Value.secondaryQualifierValue;
                    if (!string.IsNullOrEmpty(Value.tertiaryQualifierType) && Value.tertiaryQualifierType.Equals("Year"))
                        year = Value.tertiaryQualifierValue;
                }

                if (!string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                {
                    PostingDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                }
            }



            #endregion
            ResponseDTO.ResponseDTO responseModel = new ResponseDTO.ResponseDTO();


            #region Request Header
            string temp = Helper.Get8Digits();
            ResAvancedService.OGHeader OGHeader = new ResAvancedService.OGHeader();
            OGHeader.transactionID = temp;
            OGHeader.timeStamp = DateTime.Now;
            OGHeader.primaryLangID = Request.Language; //English
            ResAvancedService.EndPoint orginEndPOint = new ResAvancedService.EndPoint();
            orginEndPOint.entityID = Request.KioskID; //Kiosk Identifier
            orginEndPOint.systemType = Request.SystemType;
            OGHeader.Origin = orginEndPOint;
            ResAvancedService.EndPoint destEndPOint = new ResAvancedService.EndPoint();
            destEndPOint.entityID = Request.DestinationEntityID;
            destEndPOint.systemType = Request.DestinationSystemType;
            OGHeader.Destination = destEndPOint;
            ResAvancedService.OGHeaderAuthentication Auth = new ResAvancedService.OGHeaderAuthentication();
            ResAvancedService.OGHeaderAuthenticationUserCredentials userCredentials = new ResAvancedService.OGHeaderAuthenticationUserCredentials();
            userCredentials.UserName = Request.Username;
            userCredentials.UserPassword = Request.Password;
            userCredentials.Domain = Request.HotelDomain;
            Auth.UserCredentials = userCredentials;
            OGHeader.Authentication = Auth;
            #endregion

            #region Request Body

            ResAvancedService.MakePaymentRequest MPRequest = new ResAvancedService.MakePaymentRequest();
            ResAvancedService.Posting PaymentPosting = new ResAvancedService.Posting();
            PaymentPosting.PostDate = PostingDate != null ? (DateTime)PostingDate : DateTime.Now;

            PaymentPosting.PostDateSpecified = true;
            PaymentPosting.PostTime = DateTime.Now;
            PaymentPosting.PostTimeSpecified = true;
            if (!string.IsNullOrEmpty(Request.MakePaymentRequest.supplementary))
            {
                PaymentPosting.LongInfo = Request.MakePaymentRequest.supplementary;
            }
            else
            {
                PaymentPosting.LongInfo = Request.MakePaymentRequest.PaymentInfo;
            }
            PaymentPosting.Charge = Request.MakePaymentRequest.Amount;
            PaymentPosting.ChargeSpecified = true;
            PaymentPosting.StationID = Request.MakePaymentRequest.StationID;

            PaymentPosting.UserID = !string.IsNullOrEmpty(Request.MakePaymentRequest.UserName) ? Request.MakePaymentRequest.UserName : Request.Username;
            PaymentPosting.FolioViewNo = Request.MakePaymentRequest.WindowNumber != null ? Request.MakePaymentRequest.WindowNumber.Value : 1;
            PaymentPosting.FolioViewNoSpecified = true;

            ResAvancedService.ReservationRequestBase RRBase = new ResAvancedService.ReservationRequestBase();
            ResAvancedService.UniqueID[] rUniqueIDList = new ResAvancedService.UniqueID[1];
            ResAvancedService.UniqueID uID = new ResAvancedService.UniqueID();
            uID.type = ResAvancedService.UniqueIDType.INTERNAL;
            uID.source = "RESV_NAME_ID";
            uID.Value = Request.MakePaymentRequest.ReservationNameID;
            rUniqueIDList[0] = uID;
            RRBase.ReservationID = rUniqueIDList;
            ResAvancedService.HotelReference HF = new ResAvancedService.HotelReference();
            HF.hotelCode = Request.HotelDomain;
            RRBase.HotelReference = HF;
            PaymentPosting.ReservationRequestBase = RRBase;
            MPRequest.Posting = PaymentPosting;

            ResAvancedService.CreditCardInfo CCInfo = new ResAvancedService.CreditCardInfo();

            ResAvancedService.CreditCard CC = new ResAvancedService.CreditCard();

            //ReservationAdvancedService.cred
            if (Request.MakePaymentRequest.PaymentTypeCode != "CA")
            {
                bool isOPIEnabled = false;
                CC.chipAndPin = false;
                CC.chipAndPinSpecified = true;
                CC.cardType = Request.MakePaymentRequest.PaymentTypeCode;//"WEB";

                if (isOPIEnabled)
                {
                    CC.Item = new ResAvancedService.VaultedCardType()
                    {
                    // valuted credit card.
                        vaultedCardID = Request.MakePaymentRequest.ApprovalCode,
                        lastFourDigits = Request.MakePaymentRequest.MaskedCardNumber.Substring(Request.MakePaymentRequest.MaskedCardNumber.Length - 4)

                    };
                }
                else
                {
                    Request.MakePaymentRequest.MaskedCardNumber = Regex.Replace(Request.MakePaymentRequest.MaskedCardNumber, @"[^\d]", "x");
                    CC.Item = Request.MakePaymentRequest.MaskedCardNumber;// "4687560100136162";
                    CC.cardCode = Request.MakePaymentRequest.PaymentTypeCode;
                }
                if (Request.MakePaymentRequest.ExpiryDate != null)
                {
                    CC.expirationDate = Request.MakePaymentRequest.ExpiryDate.Value;
                }
                else
                {
                    CC.expirationDate = DateTime.Now.AddYears(2);
                }
                //CC.expirationDateSpecified = true;
                CCInfo.Item = CC;
            }
            else
            {
                CC.cardType = Request.MakePaymentRequest.PaymentTypeCode;
                CCInfo.Item = CC;
            }
            MPRequest.Reference = Request.MakePaymentRequest.PaymentRefernce;
            MPRequest.CreditCardInfo = CCInfo;

            ResAvancedService.ResvAdvancedServiceSoapClient ResAdvPortClient = new ResAvancedService.ResvAdvancedServiceSoapClient(ResAvancedService.ResvAdvancedServiceSoapClient.EndpointConfiguration.ResvAdvancedServiceSoap);
            bool? isOperaCloudEnabled = false;
            isOperaCloudEnabled = config?.PaymentSettings!.OperaCloudEnabled;
            if (isOperaCloudEnabled is not null && isOperaCloudEnabled == true)
            {
                InfoPortClient.Endpoint.EndpointBehaviors.Add(new CustomEndpointBehaviour(config?.PaymentSettings!.WSSEUserName, config?.PaymentSettings!.WSSEPassword,
                                        Request.Username, Request.Password, Request.HotelDomain));
            }
            ResAvancedService.MakePaymentResponse RSResponse = new ResAvancedService.MakePaymentResponse();
            #endregion


            RSResponse = ResAdvPortClient.MakePayment(ref OGHeader, MPRequest);



            if (RSResponse.Result.resultStatusFlag == ResAvancedService.ResultStatusFlag.SUCCESS)
            {
                return await new ServiceResult().GetServiceResponseAsync<object>(null, "Success", ApiResponseCodes.SUCCESS, 200, null);

            }
            else
            {
              
                return await new ServiceResult().GetServiceResponseAsync<object>(null, RSResponse.Result != null ? RSResponse.Result.Text != null ? string.Join(" ", RSResponse.Result.Text.Select(x => x.Value).ToArray()) : "Failled" : "Failled", ApiResponseCodes.FAILURE, 400, null);
            }
        }

        async Task<ServiceResponse<object?>> IOperaService.ModifyReservation(OwsRequestModel modifyReservation)
        {

            #region OperaCredential
            modifyReservation.LegNumber = config?.PaymentSettings.LegNumber;
            modifyReservation.Username = config?.PaymentSettings.Username;
            modifyReservation.Language = config?.PaymentSettings.Language;
            modifyReservation.ChainCode = config?.PaymentSettings.ChainCode;
            modifyReservation.DestinationEntityID = config?.PaymentSettings.DestinationEntityID;
            modifyReservation.DestinationSystemType = config?.PaymentSettings.DestinationSystemType;
            modifyReservation.HotelDomain = config?.PaymentSettings.HotelDomain;
            modifyReservation.KioskID = config?.PaymentSettings?.KioskID;
            modifyReservation.SystemType = config?.PaymentSettings?.SystemType;
            modifyReservation.Password = config?.PaymentSettings?.Password;
            modifyReservation.modifyBookingRequest.GarunteeTypeCode = config?.PaymentSettings.GuranteeTypeCode;
            #endregion
            ReservationService.ModifyBookingRequest modifyBookingReq = new ReservationService.ModifyBookingRequest();
            ReservationService.ModifyBookingResponse modifyBookingRes = new ReservationService.ModifyBookingResponse();

            #region Request Header

            string temp = Helper.Get8Digits();
            ReservationService.OGHeader OGHeader = new ReservationService.OGHeader();
            OGHeader.transactionID = temp;
            OGHeader.timeStamp = DateTime.Now;
            OGHeader.primaryLangID = modifyReservation.Language; //English
            ReservationService.EndPoint orginEndPOint = new ReservationService.EndPoint();
            orginEndPOint.entityID = modifyReservation.KioskID; //Kiosk Identifier
            orginEndPOint.systemType = modifyReservation.SystemType;//"KIOSK";
            OGHeader.Origin = orginEndPOint;
            ReservationService.EndPoint destEndPOint = new ReservationService.EndPoint();
            destEndPOint.entityID = modifyReservation.DestinationEntityID;
            destEndPOint.systemType = modifyReservation.DestinationSystemType;
            OGHeader.Destination = destEndPOint;
            ReservationService.OGHeaderAuthentication Auth = new ReservationService.OGHeaderAuthentication();
            ReservationService.OGHeaderAuthenticationUserCredentials userCredentials = new ReservationService.OGHeaderAuthenticationUserCredentials();
            userCredentials.UserName = modifyReservation.Username;
            userCredentials.UserPassword = modifyReservation.Password;
            userCredentials.Domain = modifyReservation.HotelDomain;
            Auth.UserCredentials = userCredentials;
            OGHeader.Authentication = Auth;
            #endregion

            ReservationService.ReservationServiceSoapClient ResSoapCLient = new ReservationService.ReservationServiceSoapClient(EndpointConfiguration.ReservationServiceSoap);
            bool isOperaCloudEnabled = false;
            if (isOperaCloudEnabled)
            {
                ResSoapCLient.Endpoint.EndpointBehaviors.Add(new CustomEndpointBehaviour("WSSEUserName", "WSSEPassword",
                                        modifyReservation.Username, modifyReservation.Password, modifyReservation.HotelDomain));
            }

            ReservationService.HotelReservation hReservation = new ReservationService.HotelReservation();

            ReservationService.UniqueID[] rUniqueIDList = new ReservationService.UniqueID[2];
            ReservationService.UniqueID uID = new ReservationService.UniqueID();
            uID.type = ReservationService.UniqueIDType.INTERNAL;
            uID.source = "RESERVATIONNUMBER";
            uID.Value = modifyReservation.modifyBookingRequest.ReservationNumber;
            rUniqueIDList[0] = uID;
            uID = new ReservationService.UniqueID();
            uID.type = ReservationService.UniqueIDType.INTERNAL;
            uID.source = "LEGNUMBER";
            uID.Value = modifyReservation.LegNumber;
            rUniqueIDList[1] = uID;
            hReservation.UniqueIDList = rUniqueIDList;
            ReservationService.RoomStay Rstay = new ReservationService.RoomStay();

            #region Update UDF Fields
            if (modifyReservation.modifyBookingRequest.isUDFFieldSpecified != null && modifyReservation.modifyBookingRequest.isUDFFieldSpecified.Value)
            {
                ReservationService.UserDefinedValue[] UDFFields = new ReservationService.UserDefinedValue[modifyReservation.modifyBookingRequest.uDFFields.Count];
                int x = 0;
                foreach (UDFField uDFField in modifyReservation.modifyBookingRequest.uDFFields)
                {
                    ReservationService.UserDefinedValue UDF = new ReservationService.UserDefinedValue();
                    if (uDFField.FieldName == "PreAuthUDF")
                        uDFField.FieldName = config?.PaymentSettings.PreAuthUDF;

                    else if (uDFField.FieldName == "PreAuthAmntUDF")
                        uDFField.FieldName = config?.PaymentSettings.PreAuthAmntUDF;
                    else
                        uDFField.FieldName = uDFField.FieldName;

                    UDF.valueName = uDFField.FieldName;
                    UDF.Item = uDFField.FieldValue;
                    UDFFields[x] = UDF;
                    x++;
                }
                hReservation.UserDefinedValues = UDFFields;
                ReservationService.HotelReference HF = new ReservationService.HotelReference();
                HF.hotelCode = modifyReservation.HotelDomain;
                Rstay.HotelReference = HF;
                ReservationService.RoomStay[] ArrayRstay = { Rstay };
                hReservation.RoomStays = ArrayRstay;


                modifyBookingReq.HotelReservation = hReservation;
                //ResSoapCLient.Endpoint.Behaviors.Add(new Helper.CustomEndpointBehaviour("Test USE", "Request.WSSEPassword", "Request.KioskUserName", "Request.KioskPassword", "Request.HotelDomain"));
                modifyBookingRes = ResSoapCLient.ModifyBooking(ref OGHeader,modifyBookingReq);
                ReservationService.GDSResultStatus status = modifyBookingRes.Result;

              
                if (status.resultStatusFlag.Equals(ReservationService.ResultStatusFlag.SUCCESS))
                {

                    if (modifyBookingRes.HotelReservation is not null)
                    {
                        bool foundFlag = true;

                        if (foundFlag)
                        {
                          
                            return await new ServiceResult().GetServiceResponseAsync<object>(null, "Success", ApiResponseCodes.SUCCESS, 200, null);
                        }
                        else
                        {
                           
                            return await new ServiceResult().GetServiceResponseAsync<object>(null, "Failled to Update UDF please check the request", ApiResponseCodes.FAILURE, 400, null);

                        }
                    }
                    else
                    {
                        return await new ServiceResult().GetServiceResponseAsync<object>(null, "Failled to Update UDF please check the request", ApiResponseCodes.FAILURE, 400, null);
                    }
                }
                else
                {
                   
                    return await new ServiceResult().GetServiceResponseAsync<object>(null, status.OperaErrorCode != null ? status.OperaErrorCode : "", ApiResponseCodes.FAILURE, 400, null);

                }
             

            }
            #endregion

            #region UpdatePaymentDetails

            else if (modifyReservation.modifyBookingRequest.updateCreditCardDetails != null && modifyReservation.modifyBookingRequest.updateCreditCardDetails.Value && modifyReservation.modifyBookingRequest.PaymentMethod != null && !string.IsNullOrEmpty(modifyReservation.modifyBookingRequest.PaymentMethod.MaskedCardNumber))
            {

                ReservationService.Guarantee Gurantee = new ReservationService.Guarantee();
                Gurantee.guaranteeType = modifyReservation.modifyBookingRequest.GarunteeTypeCode;

                ReservationService.GuaranteeAccepted GuranteeAccptd = new ReservationService.GuaranteeAccepted();

                ReservationService.CreditCard CC = new ReservationService.CreditCard();
                CC.cardCode = modifyReservation.modifyBookingRequest.PaymentMethod.PaymentType;
                CC.chipAndPin = false;
                CC.chipAndPinSpecified = true;
                CC.cardType = modifyReservation.modifyBookingRequest.PaymentMethod.PaymentType;//"WEB";

                bool isOPIEnabled = false;

                if (isOPIEnabled)
                {
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                    modifyReservation.modifyBookingRequest.PaymentMethod.MaskedCardNumber = rgx.Replace(modifyReservation.modifyBookingRequest.PaymentMethod.MaskedCardNumber, "");
                    CC.Item = modifyReservation.modifyBookingRequest.PaymentMethod.MaskedCardNumber;

                }
                else
                {
                    CC.Item = modifyReservation.modifyBookingRequest.PaymentMethod.MaskedCardNumber.ToLower();// "4687560100136162";
                }
                CC.expirationDate = !string.IsNullOrEmpty(modifyReservation.modifyBookingRequest.PaymentMethod.ExpiryDate) ?
                    DateTime.ParseExact(modifyReservation.modifyBookingRequest.PaymentMethod.ExpiryDate, "d/M/yyyy", CultureInfo.InvariantCulture,
DateTimeStyles.None) : DateTime.Now.AddYears(2);
                CC.expirationDateSpecified = true;
                GuranteeAccptd.GuaranteeCreditCard = CC;
                ReservationService.GuaranteeAccepted[] ArrayGA = { GuranteeAccptd };

                Gurantee.GuaranteesAccepted = ArrayGA;
                Rstay.Guarantee = Gurantee;
                ReservationService.HotelReference HF = new ReservationService.HotelReference();
                HF.hotelCode = modifyReservation.HotelDomain;
                HF.chainCode = modifyReservation.ChainCode;
                Rstay.HotelReference = HF;

                ReservationService.RoomStay[] ArrayRstay = { Rstay };
                hReservation.RoomStays = ArrayRstay;

                modifyBookingReq.HotelReservation = hReservation;
                //ResSoapCLient.Endpoint.Behaviors.Add(new Helper.CustomEndpointBehaviour("Test USE", "Request.WSSEPassword", "Request.KioskUserName", "Request.KioskPassword", "Request.HotelDomain"));
                modifyBookingRes = ResSoapCLient.ModifyBooking(ref OGHeader,modifyBookingReq);
                ReservationService.GDSResultStatus status = modifyBookingRes.Result;
                if (status.resultStatusFlag.Equals(ReservationService.ResultStatusFlag.SUCCESS))
                {
                    return await new ServiceResult().GetServiceResponseAsync<object>(null, "Success", ApiResponseCodes.SUCCESS, 200, null);

                }
                else
                {

                    return await new ServiceResult().GetServiceResponseAsync<object>(null, string.IsNullOrEmpty(status.OperaErrorCode) ? (status.GDSError != null ? status.GDSError.Value : "Opera error") : status.OperaErrorCode, ApiResponseCodes.FAILURE, 400, null);
                }

            }
            #endregion
            return await new ServiceResult().GetServiceResponseAsync<object>(null,ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);


        }
    }
}
