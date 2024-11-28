using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;


namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class Interest : BaseEntity
    {
        public Interest() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string Email = "fnt_email";
            public const string InterestId = "fnt_interestid";
            public const string FirstName = "fnt_first_name";
            public const string ContactReference = "fnt_kontid";
            public const string MiddleName = "fnt_middle_name";
            public const string Status = "fnt_status";
            public const string Phone = "fnt_phone";
            public const string TerritoryReference = "fnt_terrid";
            public const string LastName = "fnt_last_name";
            public const string OwnerId = "ownerid";
        }
        public enum InterestStepStatus
        {
            New = 797_720_001,
            InProgress = 797_720_002,
            Agreement = 797_720_003,
            Refusal = 797_720_004
        }

        public const string EntityLogicalName = "fnt_interest";

        public string Email
        {
            get { return GetAttributeValue<string>(Metadata.Email); }
            set { Attributes[Metadata.Email] = value; }
        }
        public Guid InterestId
        {
            get { return GetAttributeValue<Guid>(Metadata.InterestId); }
            set { Attributes[Metadata.InterestId] = value; }
        }
        public string FirstName
        {
            get { return GetAttributeValue<string>(Metadata.FirstName); }
            set { Attributes[Metadata.FirstName] = value; }
        }       
        public string MiddleName
        {
            get { return GetAttributeValue<string>(Metadata.MiddleName); }
            set { Attributes[Metadata.MiddleName] = value; }
        }
        public OptionSetValue Status
        {
            get { return GetAttributeValue<OptionSetValue>(Metadata.Status); }
            set { Attributes[Metadata.Status] = value; }
        }
        public string Phone
        {
            get { return GetAttributeValue<string>(Metadata.Phone); }
            set { Attributes[Metadata.Phone] = value; }
        }
        public EntityReference ContactReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.ContactReference); }
            set { Attributes[Metadata.ContactReference] = value; }
        }
        public EntityReference TerritoryReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.TerritoryReference); }
            set { Attributes[Metadata.TerritoryReference] = value; }
        }
        public string LastName
        {
            get { return GetAttributeValue<string>(Metadata.LastName); }
            set { Attributes[Metadata.LastName] = value; }
        }

        public InterestStepStatus StatusToEnum => (InterestStepStatus)Status?.Value;
        public EntityReference InterestOwnerId 
        {
            get { return GetAttributeValue<EntityReference>(Metadata.OwnerId); }
            set { Attributes[Metadata.OwnerId] = value; }
        }
    }
}
