using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using System.Runtime.CompilerServices;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{



    [DataContract(Namespace = "AwaraIT.Training.Domain.Models.Crm.Entities")]
    [EntityLogicalName(EntityLogicalName)]
    public class PosibleDeal : BaseEntity
    {
        public PosibleDeal() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string Id = "fnt_posible_dealid";
            public const string ContactReference = "fnt_kontactid";
            public const string Status = "fnt_status";
            public const string BasePriceSum = "fnt_base_price_sum";
            public const string DiscountSum = "fnt_discount_sum";
            public const string PriceAfterDiscountSum = "fnt_price_after_discount";
            public const string TerritoryReference = "fnt_territoryid";
        }


        public enum PosibleDealStepStatus
        {
            Open = 797_720_000,
            InProgress = 797_720_001,
            Won = 797_720_002,

        }

        public const string EntityLogicalName = "fnt_posible_deal";

        [DataMember]
        public Guid PosibleDealId
        {
            get { return GetAttributeValue<Guid>(Metadata.Id); }
            set { Attributes[Metadata.Id] = value; }
        }
        [DataMember]
        public EntityReference ContactReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.ContactReference); }
            set { Attributes[Metadata.ContactReference] = value; }
        }

        [DataMember]
        public OptionSetValue Status
        {
            get { return GetAttributeValue<OptionSetValue>(Metadata.Status); }
            set { Attributes[Metadata.Status] = value; }
        }

        [DataMember]
        public decimal BasePriceSum
        {
            get { return GetAttributeValue<decimal>(Metadata.BasePriceSum); }
            set { Attributes[Metadata.BasePriceSum] = value; }
        }

        [DataMember]
        public decimal DiscountSum
        {
            get { return GetAttributeValue<decimal>(Metadata.DiscountSum); }
            set { Attributes[Metadata.DiscountSum] = value; }
        }

        [DataMember]
        public decimal PriceAfterDiscountSum
        {
            get { return GetAttributeValue<decimal>(Metadata.PriceAfterDiscountSum); }
            set { Attributes[Metadata.PriceAfterDiscountSum] = value; }
        }

        [DataMember]
        public EntityReference TerritoryReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.TerritoryReference); }
            set { Attributes[Metadata.TerritoryReference] = value; }
        }
        public PosibleDealStepStatus StatusEnum
        {
            get { return (PosibleDealStepStatus)Status?.Value; }
        }
    }
}
