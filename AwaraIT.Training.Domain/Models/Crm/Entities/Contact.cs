using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class Contact : BaseEntity
    {
        public Contact() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string Email = "fnt_email";
            public const string ContactId = "fnt_contactid";
            public const string FirstName = "fnt_first_name";
            public const string MiddleName = "fnt_middle_name";
            public const string Phone = "fnt_phone_number";
            public const string TerritoryReference = "fnt_territory";
            public const string LastName = "fnt_last_name";
        }

        public const string EntityLogicalName = "fnt_contact";


        public Guid ContactId
        {
            get { return GetAttributeValue<Guid>(Metadata.ContactId); }
            set { Attributes[Metadata.ContactId] = value; }
        }


        public string Email
        {
            get { return GetAttributeValue<string>(Metadata.Email); }
            set { Attributes[Metadata.Email] = value; }
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


        public string Phone
        {
            get { return GetAttributeValue<string>(Metadata.Phone); }
            set { Attributes[Metadata.Phone] = value; }
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
    }
}
