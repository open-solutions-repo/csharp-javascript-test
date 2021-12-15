using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CSharpJs.Test.Api.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class MessageHeader
    {
        [JsonProperty("MessageCategory")]
        public string MessageCategory { get; set; }

        [JsonProperty("MessageClass")]
        public string MessageClass { get; set; }

        [JsonProperty("MessageType")]
        public string MessageType { get; set; }

        [JsonProperty("POIID")]
        public string POIID { get; set; }

        [JsonProperty("ProtocolVersion")]
        public string ProtocolVersion { get; set; }

        [JsonProperty("SaleID")]
        public string SaleID { get; set; }

        [JsonProperty("ServiceID")]
        public string ServiceID { get; set; }
    }

    public class POITransactionID
    {
        [JsonProperty("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("TransactionID")]
        public string TransactionID { get; set; }
    }

    public class POIData
    {
        [JsonProperty("POIReconciliationID")]
        public string POIReconciliationID { get; set; }

        [JsonProperty("POITransactionID")]
        public POITransactionID POITransactionID { get; set; }
    }

    public class OutputText
    {
        [JsonProperty("CharacterStyle")]
        public string CharacterStyle { get; set; }

        [JsonProperty("EndOfLineFlag")]
        public bool EndOfLineFlag { get; set; }

        [JsonProperty("Text")]
        public string Text { get; set; }
    }

    public class OutputContent
    {
        [JsonProperty("OutputFormat")]
        public string OutputFormat { get; set; }

        [JsonProperty("OutputText")]
        public List<OutputText> OutputText { get; set; }
    }

    public class PaymentReceipt
    {
        [JsonProperty("DocumentQualifier")]
        public string DocumentQualifier { get; set; }

        [JsonProperty("OutputContent")]
        public OutputContent OutputContent { get; set; }

        [JsonProperty("RequiredSignatureFlag")]
        public bool RequiredSignatureFlag { get; set; }
    }

    public class AmountsResp
    {
        [JsonProperty("AuthorizedAmount")]
        public double AuthorizedAmount { get; set; }

        [JsonProperty("Currency")]
        public string Currency { get; set; }
    }

    public class AreaSize
    {
        [JsonProperty("X")]
        public string X { get; set; }

        [JsonProperty("Y")]
        public string Y { get; set; }
    }

    public class SignaturePoint
    {
        [JsonProperty("X")]
        public string X { get; set; }

        [JsonProperty("Y")]
        public string Y { get; set; }
    }

    public class CapturedSignature
    {
        [JsonProperty("AreaSize")]
        public AreaSize AreaSize { get; set; }

        [JsonProperty("SignaturePoint")]
        public List<SignaturePoint> SignaturePoint { get; set; }
    }

    public class Instalment
    {
        [JsonProperty("InstalmentType")]
        public string InstalmentType { get; set; }

        [JsonProperty("Period")]
        public int Period { get; set; }

        [JsonProperty("PeriodUnit")]
        public string PeriodUnit { get; set; }

        [JsonProperty("SequenceNumber")]
        public int SequenceNumber { get; set; }

        [JsonProperty("TotalNbOfPayments")]
        public int TotalNbOfPayments { get; set; }
    }

    public class AcquirerTransactionID
    {
        [JsonProperty("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("TransactionID")]
        public string TransactionID { get; set; }
    }

    public class PaymentAcquirerData
    {
        [JsonProperty("AcquirerPOIID")]
        public string AcquirerPOIID { get; set; }

        [JsonProperty("AcquirerTransactionID")]
        public AcquirerTransactionID AcquirerTransactionID { get; set; }

        [JsonProperty("ApprovalCode")]
        public string ApprovalCode { get; set; }

        [JsonProperty("MerchantID")]
        public string MerchantID { get; set; }
    }

    public class SensitiveCardData
    {
        [JsonProperty("ExpiryDate")]
        public string ExpiryDate { get; set; }
    }

    public class CardData
    {
        [JsonProperty("EntryMode")]
        public List<string> EntryMode { get; set; }

        [JsonProperty("MaskedPan")]
        public string MaskedPan { get; set; }

        [JsonProperty("PaymentBrand")]
        public string PaymentBrand { get; set; }

        [JsonProperty("SensitiveCardData")]
        public SensitiveCardData SensitiveCardData { get; set; }
    }

    public class PaymentInstrumentData
    {
        [JsonProperty("CardData")]
        public CardData CardData { get; set; }

        [JsonProperty("PaymentInstrumentType")]
        public string PaymentInstrumentType { get; set; }
    }

    public class PaymentResult
    {
        [JsonProperty("AmountsResp")]
        public AmountsResp AmountsResp { get; set; }

        [JsonProperty("AuthenticationMethod")]
        public List<string> AuthenticationMethod { get; set; }

        [JsonProperty("CapturedSignature")]
        public CapturedSignature CapturedSignature { get; set; }

        [JsonProperty("Instalment")]
        public Instalment Instalment { get; set; }

        [JsonProperty("OnlineFlag")]
        public bool OnlineFlag { get; set; }

        [JsonProperty("PaymentAcquirerData")]
        public PaymentAcquirerData PaymentAcquirerData { get; set; }

        [JsonProperty("PaymentInstrumentData")]
        public PaymentInstrumentData PaymentInstrumentData { get; set; }

        [JsonProperty("PaymentType")]
        public string PaymentType { get; set; }
    }

    public class Response
    {
        [JsonProperty("AdditionalResponse")]
        public string AdditionalResponse { get; set; }

        [JsonProperty("Result")]
        public string Result { get; set; }
    }

    public class SaleTransactionID
    {
        [JsonProperty("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("TransactionID")]
        public string TransactionID { get; set; }
    }

    public class SaleData
    {
        [JsonProperty("SaleTransactionID")]
        public SaleTransactionID SaleTransactionID { get; set; }
    }

    public class PaymentResponse
    {
        [JsonProperty("POIData")]
        public POIData POIData { get; set; }

        [JsonProperty("PaymentReceipt")]
        public List<PaymentReceipt> PaymentReceipt { get; set; }

        [JsonProperty("PaymentResult")]
        public PaymentResult PaymentResult { get; set; }

        [JsonProperty("Response")]
        public Response Response { get; set; }

        [JsonProperty("SaleData")]
        public SaleData SaleData { get; set; }
    }

    public class SaleToPOIResponse
    {
        [JsonProperty("MessageHeader")]
        public MessageHeader MessageHeader { get; set; }

        [JsonProperty("PaymentResponse")]
        public PaymentResponse PaymentResponse { get; set; }
    }

    public class Root
    {
        [JsonProperty("SaleToPOIResponse")]
        public SaleToPOIResponse SaleToPOIResponse { get; set; }
    }
}
