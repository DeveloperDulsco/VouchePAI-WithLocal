using BussinessLogic.Abstractions;
using Domain;
using Domain.Response;
using Domain.Responses;
using Infrastructure.OwsServiceClass.OwsHelper;
using Infrastructure.ResponseDTO;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

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
            InformationService.QueryLovRequest LOVReq = new InformationService.QueryLovRequest();
            InformationService.QueryLovResponse LOVRes = new InformationService.QueryLovResponse();

            InformationService.LovQueryType2 LOVQuery = new InformationService.LovQueryType2();
            LOVQuery.LovIdentifier = "BUSINESSDATE";
            LOVReq.LovRequest.Item = LOVQuery;
            #endregion

            InformationService.InformationSoapClient InfoPortClient = new InformationService.InformationSoapClient();
            bool? isOperaCloudEnabled1 = false;
            isOperaCloudEnabled1 = config?.PaymentSettings!.OperaCloudEnabled;
            if (isOperaCloudEnabled1 is not null && isOperaCloudEnabled1==true)
            {
                InfoPortClient.Endpoint.EndpointBehaviors.Add(new CustomEndpointBehaviour(config?.PaymentSettings!.WSSEUserName, config?.PaymentSettings!.WSSEPassword,
                                        Request.Username, Request.Password, Request.HotelDomain));
            }
            LOVRes = InfoPortClient.QueryLov(LOVReq);
            if (LOVRes.LovResponse.Result.resultStatusFlag == InformationService.ResultStatusFlag.SUCCESS)
            {
                string date = null;
                string month = null;
                string year = null;
                foreach (InformationService.LovQueryResultType Value in LOVRes.LovResponse.LovQueryResult)
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
            ReservationAdvancedService.OGHeader OGHeader = new ReservationAdvancedService.OGHeader();
            OGHeader.transactionID = temp;
            OGHeader.timeStamp = DateTime.Now;
            OGHeader.primaryLangID = Request.Language; //English
            ReservationAdvancedService.EndPoint orginEndPOint = new ReservationAdvancedService.EndPoint();
            orginEndPOint.entityID = Request.KioskID; //Kiosk Identifier
            orginEndPOint.systemType = Request.SystemType;
            OGHeader.Origin = orginEndPOint;
            ReservationAdvancedService.EndPoint destEndPOint = new ReservationAdvancedService.EndPoint();
            destEndPOint.entityID = Request.DestinationEntityID;
            destEndPOint.systemType = Request.DestinationSystemType;
            OGHeader.Destination = destEndPOint;
            ReservationAdvancedService.OGHeaderAuthentication Auth = new ReservationAdvancedService.OGHeaderAuthentication();
            ReservationAdvancedService.OGHeaderAuthenticationUserCredentials userCredentials = new ReservationAdvancedService.OGHeaderAuthenticationUserCredentials();
            userCredentials.UserName = Request.Username;
            userCredentials.UserPassword = Request.Password;
            userCredentials.Domain = Request.HotelDomain;
            Auth.UserCredentials = userCredentials;
            OGHeader.Authentication = Auth;
            #endregion

            #region Request Body

            ReservationAdvancedService.MakePaymentRequest MPRequest = new ReservationAdvancedService.MakePaymentRequest();
            ReservationAdvancedService.Posting PaymentPosting = new ReservationAdvancedService.Posting();
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

            ReservationAdvancedService.ReservationRequestBase RRBase = new ReservationAdvancedService.ReservationRequestBase();
            ReservationAdvancedService.UniqueID[] rUniqueIDList = new ReservationAdvancedService.UniqueID[1];
            ReservationAdvancedService.UniqueID uID = new ReservationAdvancedService.UniqueID();
            uID.type = ReservationAdvancedService.UniqueIDType.INTERNAL;
            uID.source = "RESV_NAME_ID";
            uID.Value = Request.MakePaymentRequest.ReservationNameID;
            rUniqueIDList[0] = uID;
            RRBase.ReservationID = rUniqueIDList;
            ReservationAdvancedService.HotelReference HF = new ReservationAdvancedService.HotelReference();
            HF.hotelCode = Request.HotelDomain;
            RRBase.HotelReference = HF;
            PaymentPosting.ReservationRequestBase = RRBase;
            MPRequest.Posting = PaymentPosting;

            ReservationAdvancedService.CreditCardInfo CCInfo = new ReservationAdvancedService.CreditCardInfo();

            ReservationAdvancedService.CreditCard CC = new ReservationAdvancedService.CreditCard();

            //ReservationAdvancedService.cred
            if (Request.MakePaymentRequest.PaymentTypeCode != "CA")
            {
                bool isOPIEnabled = false;
                CC.chipAndPin = false;
                CC.chipAndPinSpecified = true;
                CC.cardType = Request.MakePaymentRequest.PaymentTypeCode;//"WEB";

                if (isOPIEnabled)
                {
                    CC.Item = new ReservationAdvancedService.VaultedCardType()
                    {
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
                CC.expirationDateSpecified = true;
                CCInfo.Item = CC;
            }
            else
            {
                CC.cardType = Request.MakePaymentRequest.PaymentTypeCode;
                CCInfo.Item = CC;
            }
            MPRequest.Reference = Request.MakePaymentRequest.PaymentRefernce;
            MPRequest.CreditCardInfo = CCInfo;

            ReservationAdvancedService.ResvAdvancedServiceSoapClient ResAdvPortClient = new ReservationAdvancedService.ResvAdvancedServiceSoapClient();
            bool? isOperaCloudEnabled = false;
            isOperaCloudEnabled = config?.PaymentSettings!.OperaCloudEnabled;
            if (isOperaCloudEnabled is not null && isOperaCloudEnabled == true)
            {
                InfoPortClient.Endpoint.EndpointBehaviors.Add(new CustomEndpointBehaviour(config?.PaymentSettings!.WSSEUserName, config?.PaymentSettings!.WSSEPassword,
                                        Request.Username, Request.Password, Request.HotelDomain));
            }
            ReservationAdvancedService.MakePaymentResponse RSResponse = new ReservationAdvancedService.MakePaymentResponse();
            #endregion


            RSResponse = await ResAdvPortClient.MakePaymentAsync(OGHeader, MPRequest);



            if (RSResponse.Result.resultStatusFlag == ReservationAdvancedService.ResultStatusFlag.SUCCESS)
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

            ReservationService.ReservationServiceSoapClient ResSoapCLient = new ReservationService.ReservationServiceSoapClient();
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
                modifyBookingRes = ResSoapCLient.ModifyBookingAsync(modifyBookingReq);
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
                modifyBookingRes = ResSoapCLient.ModifyBookingAsync(modifyBookingReq);
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
